using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ArthSetuBackend.Data;
using ArthSetuBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace ArthSetuBackend.Services
{
    public class SourceSyncService
    {
        private readonly ApplicationDbContext _context;

        public SourceSyncService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<SourceSyncLog> RunSyncTransactionAsync(
            string sourceId,
            string connectorName,
            string finalUrl,
            string payload,
            int httpStatus,
            string contentType,
            List<SchemeStaging> stagingSchemes)
        {
            var syncLog = new SourceSyncLog
            {
                SourceId = sourceId,
                Connector = connectorName,
                StartedAt = DateTime.UtcNow,
                FetchStatus = httpStatus >= 200 && httpStatus < 300 ? "SUCCESS" : "FAILED",
                HttpStatus = httpStatus,
                RecordsDiscovered = stagingSchemes.Count,
                RecordsParsed = stagingSchemes.Count,
                RecordsImported = 0,
                RecordsUpdated = 0,
                RecordsUnchanged = 0,
                RecordsSkipped = 0,
                RecordsNeedingReview = 0,
                ConflictsCreated = 0
            };

            if (httpStatus < 200 || httpStatus >= 300)
            {
                syncLog.Status = "FAILED";
                syncLog.ErrorMessage = $"HTTP request failed with status {httpStatus}";
                syncLog.CompletedAt = DateTime.UtcNow;
                _context.SourceSyncLogs.Add(syncLog);
                await _context.SaveChangesAsync();
                return syncLog;
            }

            var hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(payload));
            string contentHash = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();

            // Check if snapshot exists and matches
            var lastSnapshot = await _context.SourceSnapshots
                .Where(s => s.SourceId == sourceId)
                .OrderByDescending(s => s.SnapshotDate)
                .FirstOrDefaultAsync();

            bool contentChanged = lastSnapshot == null || lastSnapshot.ContentHash != contentHash;
            syncLog.ContentChanged = contentChanged;

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Always create a new snapshot if content changed, or reference the old one if it didn't
                int snapshotId;
                if (contentChanged)
                {
                    var snapshot = new SourceSnapshot
                    {
                        SourceId = sourceId,
                        SnapshotDate = DateTime.UtcNow,
                        Payload = payload, // Store raw or reference
                        SourceUrl = finalUrl,
                        FinalUrl = finalUrl,
                        ContentType = contentType,
                        ContentHash = contentHash,
                        ConnectorVersion = "1.0",
                        VerificationStatus = "Verified"
                    };
                    _context.SourceSnapshots.Add(snapshot);
                    await _context.SaveChangesAsync();
                    snapshotId = snapshot.Id;
                }
                else
                {
                    snapshotId = lastSnapshot!.Id;
                }
                
                syncLog.SnapshotId = snapshotId;
                string snapshotHash = $"snap-{snapshotId}-{contentHash.Substring(0, 8)}";

                // Ensure GovernmentSource is active
                var govSource = await _context.GovernmentSources.FirstOrDefaultAsync(s => s.Id == sourceId);
                if (govSource == null)
                {
                    govSource = new GovernmentSource
                    {
                        Id = sourceId,
                        SourceName = "NSFDC",
                        IsActive = true,
                        ConnectionStatus = "SYNC_FAILED",
                        VerificationStatus = "Verified"
                    };
                    _context.GovernmentSources.Add(govSource);
                }
                
                // VALIDATION: Staging-Before-Promotion
                int previousSchemeCount = await _context.Schemes.CountAsync(s => s.SourceId == sourceId && s.IsActive && s.LifecycleStatus != "SUPERSEDED");
                
                bool isSuspiciousDrop = previousSchemeCount > 0 && stagingSchemes.Count < previousSchemeCount;
                bool isMissingMinimum = stagingSchemes == null || stagingSchemes.Count == 0;

                if (isMissingMinimum || isSuspiciousDrop)
                {
                    syncLog.Status = "VALIDATION_FAILED";
                    syncLog.ErrorMessage = isSuspiciousDrop ? $"Suspicious drop in scheme count (Previous: {previousSchemeCount}, Current: {stagingSchemes?.Count}). Failing closed." : "No schemes found or null. Failing closed.";
                    syncLog.CompletedAt = DateTime.UtcNow;
                    
                    govSource.LastSync = DateTime.UtcNow;
                    govSource.ConnectionStatus = "VALIDATION_FAILED";
                    _context.GovernmentSources.Update(govSource);
                    
                    _context.SourceSyncLogs.Add(syncLog);
                    await _context.SaveChangesAsync();
                    
                    // Commit the log and source state, but DO NOT promote any scheme data
                    await transaction.CommitAsync();
                    return syncLog;
                }

                govSource.ConnectionStatus = "SYNC_SUCCESS";
                govSource.VerificationStatus = "Verified";
                govSource.LastSync = DateTime.UtcNow;
                govSource.LastSuccessfulSync = DateTime.UtcNow;
                _context.GovernmentSources.Update(govSource);
                
                var incomingSchemeIds = stagingSchemes.Select(s => s.SchemeId).ToList();
                var oldSchemes = await _context.Schemes
                    .Where(s => s.SourceId == sourceId && !incomingSchemeIds.Contains(s.Id) && s.LifecycleStatus != "SUPERSEDED")
                    .ToListAsync();
                foreach (var old in oldSchemes)
                {
                    old.LifecycleStatus = "SUPERSEDED";
                    old.DataOrigin = "LEGACY_REFERENCE";
                    old.SupersededBy = "Normalized_Phase8B_1";
                    _context.Schemes.Update(old);
                    syncLog.RecordsUpdated++;
                }
                
                foreach (var staging in stagingSchemes!)
                {
                    // Fallback for missing OwningAuthority from GovernmentSource metadata
                    if (string.IsNullOrWhiteSpace(staging.OwningAuthority))
                    {
                        staging.OwningAuthority = govSource?.ImplementingAgency ?? govSource?.Department ?? govSource?.Ministry ?? govSource?.SourceName ?? sourceId.Replace("src-", "").ToUpper();
                    }

                    // EXPLICIT PROMOTION LOGIC
                    bool canPromote = true;
                    if (snapshotId <= 0) canPromote = false;
                    if (string.IsNullOrWhiteSpace(staging.Name) || string.IsNullOrWhiteSpace(staging.Description)) canPromote = false;
                    if (string.IsNullOrWhiteSpace(staging.OwningAuthority) || staging.OwningAuthority == "NONE") canPromote = false;
                    if (staging.LifecycleStatus == "DISCOVERY_CATEGORY" || staging.LifecycleStatus == "SUPERSEDED" || staging.LifecycleStatus == "GENERIC_PLACEHOLDER") canPromote = false;
                    if (staging.DataOrigin == "LEGACY_REFERENCE" || staging.DataOrigin == "DEVELOPMENT_SEED") canPromote = false;

                    if (canPromote)
                    {
                        staging.VerificationStatus = "Verified";
                        staging.DataOrigin = "OFFICIAL";
                    }
                    else
                    {
                        staging.VerificationStatus = "NEEDS_REVIEW";
                        if (staging.DataOrigin == "OFFICIAL") staging.DataOrigin = "IMPORTED";
                    }

                    bool isNew = false;
                    bool isUpdated = false;

                    var scheme = await _context.Schemes.FirstOrDefaultAsync(s => s.Id == staging.SchemeId);
                    if (scheme == null)
                    {
                        isNew = true;
                        scheme = new Scheme
                        {
                            Id = staging.SchemeId,
                            Name = staging.Name,
                            SourceId = sourceId,
                            Description = staging.Description,
                            SchemeCategory = staging.SchemeCategory,
                            DataOrigin = staging.DataOrigin ?? "IMPORTED",
                            VerificationStatus = staging.VerificationStatus ?? "NEEDS_REVIEW",
                            IsActive = true,
                            CurrentPublishedVersion = snapshotHash,
                            LastVerified = DateTime.UtcNow,
                            
                            // Phase 8B additions
                            LifecycleStatus = staging.LifecycleStatus,
                            ApplicationStartDate = staging.ApplicationStartDate,
                            ApplicationEndDate = staging.ApplicationEndDate,
                            OwningAuthority = staging.OwningAuthority,
                            OfficialRuleSource = staging.OfficialRuleSource,
                            ApplicationPortal = staging.ApplicationPortal,
                            DiscoveryPortal = staging.DiscoveryPortal
                        };

                        _context.Schemes.Add(scheme);
                        syncLog.RecordsImported++;
                        
                        // Map Application Windows
                        if (staging.ApplicationWindows != null)
                        {
                            foreach (var w in staging.ApplicationWindows)
                            {
                                scheme.ApplicationWindows.Add(new SchemeApplicationWindow
                                {
                                    Cycle = w.Cycle,
                                    ApplicationType = w.ApplicationType,
                                    StartDate = w.StartDate,
                                    EndDate = w.EndDate,
                                    Status = w.Status,
                                    SourceSnapshotId = w.SourceSnapshotId,
                                    LastVerifiedAt = w.LastVerifiedAt
                                });
                            }
                        }

                        
                        // Process Benefit Components
                        if (staging.BenefitComponents != null)
                        {
                            foreach (var stagingBc in staging.BenefitComponents)
                            {
                                var existingBc = scheme.BenefitComponents.FirstOrDefault(b => b.BenefitType == stagingBc.BenefitType && b.ComponentDescription == stagingBc.ComponentDescription);
                                if (existingBc == null)
                                {
                                    existingBc = new SchemeBenefitComponent
                                    {
                                        BenefitType = stagingBc.BenefitType,
                                        ComponentDescription = stagingBc.ComponentDescription
                                    };
                                    scheme.BenefitComponents.Add(existingBc);
                                }
                                
                                // We need the ID for the rules, so we save changes here
                                await _context.SaveChangesAsync();
                                int bcId = existingBc.Id;

                                if (stagingBc.ProjectCostMin.HasValue)
                                    await ProcessRuleAsync(scheme.Id, "ProjectCostMin", "GreaterThanOrEqual", stagingBc.ProjectCostMin.Value.ToString(), stagingBc.ProjectCostMinRaw, snapshotId, snapshotHash, sourceId, finalUrl, stagingBc.SourceSection, contentChanged, staging.VerificationStatus, bcId);

                                if (stagingBc.ProjectCostMax.HasValue)
                                    await ProcessRuleAsync(scheme.Id, "ProjectCostMax", "LessThanOrEqual", stagingBc.ProjectCostMax.Value.ToString(), stagingBc.ProjectCostMaxRaw, snapshotId, snapshotHash, sourceId, finalUrl, stagingBc.SourceSection, contentChanged, staging.VerificationStatus, bcId);

                                if (stagingBc.LoanMin.HasValue)
                                    await ProcessRuleAsync(scheme.Id, "MinimumLoan", "GreaterThanOrEqual", stagingBc.LoanMin.Value.ToString(), stagingBc.LoanMinRaw, snapshotId, snapshotHash, sourceId, finalUrl, stagingBc.SourceSection, contentChanged, staging.VerificationStatus, bcId);

                                if (stagingBc.LoanMax.HasValue)
                                    await ProcessRuleAsync(scheme.Id, "MaximumLoan", "LessThanOrEqual", stagingBc.LoanMax.Value.ToString(), stagingBc.LoanMaxRaw, snapshotId, snapshotHash, sourceId, finalUrl, stagingBc.SourceSection, contentChanged, staging.VerificationStatus, bcId);

                                if (!string.IsNullOrEmpty(stagingBc.SpecialEligibilityRaw))
                                    await ProcessRuleAsync(scheme.Id, "SpecialEligibility", "Equals", stagingBc.SpecialEligibilityRaw, stagingBc.SpecialEligibilityRaw, snapshotId, snapshotHash, sourceId, finalUrl, stagingBc.SourceSection, contentChanged, staging.VerificationStatus, bcId);
                            }
                        }
                    }
                    else
                    {
                        if (scheme.Description != staging.Description || scheme.SchemeCategory != staging.SchemeCategory || scheme.LifecycleStatus != staging.LifecycleStatus || scheme.DataOrigin != (staging.DataOrigin ?? "IMPORTED") || scheme.VerificationStatus != (staging.VerificationStatus ?? "NEEDS_REVIEW"))
                        {
                            scheme.Description = staging.Description;
                            scheme.SchemeCategory = staging.SchemeCategory;
                            scheme.CurrentPublishedVersion = snapshotHash;
                            scheme.LastVerified = DateTime.UtcNow;

                            // Phase 8B update additions (basic fields)
                            scheme.LifecycleStatus = staging.LifecycleStatus;
                            scheme.DataOrigin = staging.DataOrigin ?? "IMPORTED";
                            scheme.VerificationStatus = staging.VerificationStatus ?? "NEEDS_REVIEW";
                            scheme.ApplicationStartDate = staging.ApplicationStartDate;
                            scheme.ApplicationEndDate = staging.ApplicationEndDate;
                            scheme.OwningAuthority = staging.OwningAuthority;
                            scheme.OfficialRuleSource = staging.OfficialRuleSource;
                            scheme.ApplicationPortal = staging.ApplicationPortal;
                            scheme.DiscoveryPortal = staging.DiscoveryPortal;

                            _context.Schemes.Update(scheme);
                            isUpdated = true;
                            syncLog.RecordsUpdated++;
                        }
                        else
                        {
                            scheme.LastVerified = DateTime.UtcNow;
                            _context.Schemes.Update(scheme);
                            syncLog.RecordsUnchanged++;
                        }
                    }
                    
                    if (isNew || isUpdated)
                    {
                        _context.SchemeVersions.Add(new SchemeVersion
                        {
                            SchemeId = scheme.Id,
                            VersionHash = snapshotHash,
                            CreatedAt = DateTime.UtcNow
                        });
                        
                        // Provenance for Scheme Fields
                        AddProvenance(scheme.Id, "Name", staging.Name, staging.Name, sourceId, finalUrl, staging.SourceSection, staging.VerificationStatus);
                        AddProvenance(scheme.Id, "Description", staging.Description, staging.Description, sourceId, finalUrl, staging.SourceSection, staging.VerificationStatus);
                    }

                    // Process Rules
                    await ProcessRuleAsync(scheme.Id, "Community", "Equals", staging.TargetCommunity, staging.TargetCommunity, snapshotId, snapshotHash, sourceId, finalUrl, staging.SourceSection, contentChanged, staging.VerificationStatus);
                    await ProcessRuleAsync(scheme.Id, "Gender", "Equals", staging.Gender, staging.Gender, snapshotId, snapshotHash, sourceId, finalUrl, staging.SourceSection, contentChanged, staging.VerificationStatus);
                    
                    if (staging.IsPwD.HasValue)
                        await ProcessRuleAsync(scheme.Id, "IsPwD", "Equals", staging.IsPwD.Value.ToString(), staging.IsPwD.Value.ToString(), snapshotId, snapshotHash, sourceId, finalUrl, staging.SourceSection, contentChanged, staging.VerificationStatus);
                        
                    if (staging.DisabilityPercentageMin.HasValue)
                        await ProcessRuleAsync(scheme.Id, "DisabilityPercentageMin", "GreaterThanOrEqual", staging.DisabilityPercentageMin.Value.ToString(), staging.DisabilityPercentageMin.Value.ToString(), snapshotId, snapshotHash, sourceId, finalUrl, staging.SourceSection, contentChanged, staging.VerificationStatus);
                        
                    await ProcessRuleAsync(scheme.Id, "ApplicantType", "Equals", staging.ApplicantType, staging.ApplicantType, snapshotId, snapshotHash, sourceId, finalUrl, staging.SourceSection, contentChanged, staging.VerificationStatus);
                    
                    if (staging.GroupMinTargetPercentage.HasValue)
                        await ProcessRuleAsync(scheme.Id, "GroupMinTargetPercentage", "GreaterThanOrEqual", staging.GroupMinTargetPercentage.Value.ToString(), staging.GroupMinTargetPercentage.Value.ToString(), snapshotId, snapshotHash, sourceId, finalUrl, staging.SourceSection, contentChanged, staging.VerificationStatus);
                    
                    if (staging.IncomeCeiling.HasValue)
                        await ProcessRuleAsync(scheme.Id, "AnnualFamilyIncome", "LessThanOrEqual", staging.IncomeCeiling.Value.ToString(), staging.IncomeCeilingRaw, snapshotId, snapshotHash, sourceId, finalUrl, staging.SourceSection, contentChanged, staging.VerificationStatus);
                    
                    if (staging.ProjectCostMax.HasValue)
                        await ProcessRuleAsync(scheme.Id, "ProjectCostMax", "LessThanOrEqual", staging.ProjectCostMax.Value.ToString(), staging.ProjectCostMaxRaw, snapshotId, snapshotHash, sourceId, finalUrl, staging.SourceSection, contentChanged, staging.VerificationStatus);

                    if (staging.ProjectCostMin.HasValue)
                        await ProcessRuleAsync(scheme.Id, "ProjectCostMin", "GreaterThan", staging.ProjectCostMin.Value.ToString(), staging.ProjectCostMinRaw, snapshotId, snapshotHash, sourceId, finalUrl, staging.SourceSection, contentChanged, staging.VerificationStatus);

                    if (staging.LoanMax.HasValue)
                        await ProcessRuleAsync(scheme.Id, "MaximumLoan", "LessThanOrEqual", staging.LoanMax.Value.ToString(), staging.LoanMaxRaw, snapshotId, snapshotHash, sourceId, finalUrl, staging.SourceSection, contentChanged, staging.VerificationStatus);

                    if (!string.IsNullOrEmpty(staging.InterestRateRaw))
                        await ProcessRuleAsync(scheme.Id, "InterestRate", "Equals", staging.InterestRateRaw, staging.InterestRateRaw, snapshotId, snapshotHash, sourceId, finalUrl, staging.SourceSection, contentChanged, staging.VerificationStatus);
                        
                    if (!string.IsNullOrEmpty(staging.TenureRaw))
                        await ProcessRuleAsync(scheme.Id, "Tenure", "Equals", staging.TenureRaw, staging.TenureRaw, snapshotId, snapshotHash, sourceId, finalUrl, staging.SourceSection, contentChanged, staging.VerificationStatus);

                    if (!string.IsNullOrEmpty(staging.MoratoriumRaw))
                        await ProcessRuleAsync(scheme.Id, "Moratorium", "Equals", staging.MoratoriumRaw, staging.MoratoriumRaw, snapshotId, snapshotHash, sourceId, finalUrl, staging.SourceSection, contentChanged, staging.VerificationStatus);
                }

                syncLog.Status = "SUCCESS";
                syncLog.CompletedAt = DateTime.UtcNow;
                _context.SourceSyncLogs.Add(syncLog);
                
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                
                return syncLog;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                syncLog.Status = "FAILED";
                syncLog.ErrorMessage = ex.Message;
                syncLog.CompletedAt = DateTime.UtcNow;
                
                // Save log outside transaction
                _context.SourceSyncLogs.Add(syncLog);
                await _context.SaveChangesAsync();
                return syncLog;
            }
        }

        private async Task ProcessRuleAsync(string schemeId, string field, string op, string? normalizedValue, string? rawValue, int snapshotId, string versionHash, string sourceId, string url, string? section, bool contentChanged, string verificationStatus, int? schemeBenefitComponentId = null)
        {
            if (string.IsNullOrEmpty(normalizedValue)) return;
            
            var existingRule = await _context.SchemeEligibilityRules.FirstOrDefaultAsync(r => r.SchemeId == schemeId && r.Field == field && r.SchemeBenefitComponentId == schemeBenefitComponentId);
            bool isNewOrUpdatedRule = false;
            
            if (existingRule == null)
            {
                existingRule = new SchemeEligibilityRule
                {
                    SchemeId = schemeId,
                    SchemeBenefitComponentId = schemeBenefitComponentId,
                    Field = field,
                    Operator = op,
                    Value = normalizedValue,
                    Mandatory = true,
                    VerificationStatus = verificationStatus,
                    SourceReference = url
                };
                _context.SchemeEligibilityRules.Add(existingRule);
                await _context.SaveChangesAsync();
                isNewOrUpdatedRule = true;
            }
            else if (existingRule.Value != normalizedValue)
            {
                // Create conflict
                _context.SourceConflicts.Add(new SourceConflict
                {
                    SchemeId = schemeId,
                    SchemeBenefitComponentId = schemeBenefitComponentId,
                    Field = field,
                    ExistingValue = existingRule.Value ?? "",
                    CandidateValue = normalizedValue,
                    ExistingSource = "System",
                    CandidateSource = url,
                    PublicationDate = DateTime.UtcNow,
                    Status = "Resolved"
                });
                
                existingRule.Value = normalizedValue;
                existingRule.VerificationStatus = verificationStatus;
                existingRule.SourceReference = url;
                _context.SchemeEligibilityRules.Update(existingRule);
                await _context.SaveChangesAsync();
                isNewOrUpdatedRule = true;
            }
            
            if (isNewOrUpdatedRule)
            {
                // Versioning
                _context.EligibilityRuleVersions.Add(new EligibilityRuleVersion
                {
                    RuleId = existingRule.Id,
                    SchemeId = schemeId,
                    SchemeBenefitComponentId = schemeBenefitComponentId,
                    VersionHash = versionHash,
                    NormalizedValue = normalizedValue,
                    SourceSnapshotId = snapshotId,
                    EffectiveFrom = DateTime.UtcNow,
                    CurrentStatus = "Active",
                    VerificationStatus = verificationStatus,
                    SourceEvidence = url,
                    CreatedAt = DateTime.UtcNow
                });
                
                // Provenance
                AddProvenance(schemeId, field, normalizedValue, rawValue, sourceId, url, section, verificationStatus);
            }
        }

        private void AddProvenance(string entityId, string field, string normalizedValue, string? rawValue, string sourceId, string url, string? section, string verificationStatus)
        {
            var prov = new FieldProvenance
            {
                EntityId = entityId,
                FieldName = field,
                SourceId = sourceId,
                VerifiedAt = DateTime.UtcNow,
                RawValue = rawValue ?? normalizedValue,
                SourceUrl = url,
                SourceLocation = section,
                ExtractedAt = DateTime.UtcNow,
                VerificationStatus = verificationStatus
            };
            _context.FieldProvenance.Add(prov);
        }
    }
}

