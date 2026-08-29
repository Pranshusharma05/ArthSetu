using Microsoft.AspNetCore.Mvc;
using ArthSetuBackend.Data;
using ArthSetuBackend.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ArthSetuBackend.Controllers
{
    [ApiController]
    [Route("api/schemes")]
    public class SchemeMatchingController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        private static readonly string[] ExcludedOrigins =
        {
            "DEMO", "MOCK", "DEVELOPMENT_SEED", "LEGACY_REFERENCE", "DISCOVERY_CATEGORY",
            "NEEDS_REVIEW", "SUPERSEDED", "GENERIC_PLACEHOLDER"
        };

        private static readonly string[] OutputOnlyRuleFields =
        {
            "InterestRate", "MaximumLoan", "Tenure", "Moratorium", "BenefitAmount", "SubsidyAmount"
        };

        private static readonly Regex SyntheticNamePattern = new(
            @"^(general\s+scheme|scheme\s+[a-z]?\s*\d+|test\s+scheme|demo\s+scheme|sample\s+scheme|mock\s+scheme|seed\s+scheme|generated\s+scheme|placeholder)|\b(discovered\s+scheme|wave\s+[a-z])\b",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public SchemeMatchingController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("stats")]
        public IActionResult GetStats()
        {
            var schemes = _context.Schemes.AsNoTracking().ToList();
            var rules = _context.SchemeEligibilityRules.AsNoTracking().ToList();
            var conflicts = _context.SourceConflicts.AsNoTracking().ToList();
            var sources = _context.GovernmentSources.AsNoTracking().ToDictionary(s => s.Id, s => s);

            var publishable = schemes.Where(s =>
            {
                sources.TryGetValue(s.SourceId ?? string.Empty, out var source);
                var schemeRules = rules.Where(r => r.SchemeId == s.Id).ToList();
                var hasCriticalConflict = conflicts.Any(c => c.SchemeId == s.Id && !IsResolvedConflict(c.Status));
                return IsCitizenPublishable(s, source, schemeRules, hasCriticalConflict);
            }).ToList();

            return Ok(new
            {
                total = schemes.Count,
                verified = publishable.Count,
                central = publishable.Count(s => string.Equals(s.GovernmentLevel ?? s.Scope, "Central", StringComparison.OrdinalIgnoreCase)),
                state = publishable.Count(s => string.Equals(s.GovernmentLevel ?? s.Scope, "State", StringComparison.OrdinalIgnoreCase)),
                ut = publishable.Count(s => string.Equals(s.GovernmentLevel ?? s.Scope, "UT", StringComparison.OrdinalIgnoreCase)),
                needsReview = schemes.Count(s => string.Equals(s.VerificationStatus, "NEEDS_REVIEW", StringComparison.OrdinalIgnoreCase) || string.Equals(s.VerificationStatus, "Needs Review", StringComparison.OrdinalIgnoreCase)),
                demo = schemes.Count(s => string.Equals(s.DataOrigin, "DEMO", StringComparison.OrdinalIgnoreCase) || string.Equals(s.DataOrigin, "MOCK", StringComparison.OrdinalIgnoreCase)),
                appUrls = publishable.Count(s => !string.IsNullOrWhiteSpace(s.OfficialApplicationUrl)),
                sources = _context.GovernmentSources.Count()
            });
        }

        [HttpPost("match")]
        public IActionResult EvaluateEligibility([FromBody] UserProfile profile)
        {
            var targetPurpose = NormalizePurpose(profile.Purpose);

            var schemes = _context.Schemes
                .AsNoTracking()
                .Include(s => s.Source)
                .Include(s => s.ApplicationWindows)
                .Where(s => s.IsActive && s.DataOrigin == "OFFICIAL" &&
                    (s.VerificationStatus == "Verified" || s.VerificationStatus == "VERIFIED"))
                .ToList();

            var allRules = _context.SchemeEligibilityRules.AsNoTracking().ToList();
            var unresolvedConflictSchemeIds = _context.SourceConflicts
                .AsNoTracking()
                .Where(c => c.Status != "RESOLVED" && c.Status != "Resolved" && c.Status != "CLOSED" && c.Status != "Closed")
                .Select(c => c.SchemeId)
                .Distinct()
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            var activeSchemes = schemes.Where(s =>
            {
                var rules = allRules.Where(r => r.SchemeId == s.Id).ToList();
                if (!IsCitizenPublishable(s, s.Source, rules, unresolvedConflictSchemeIds.Contains(s.Id))) return false;
                if (!PurposeMatches(s, profile.Purpose, targetPurpose)) return false;
                if (!GeographyMatches(s, profile.State, profile.District)) return false;
                return true;
            }).ToList();

            var recommended = new List<object>();
            var notEligible = new List<object>();
            var moreInfoNeeded = new List<object>();

            foreach (var scheme in activeSchemes)
            {
                var rules = allRules.Where(r => r.SchemeId == scheme.Id && !OutputOnlyRuleFields.Contains(r.Field)).ToList();
                if (rules.Count == 0) continue;

                var evalRes = EvaluateRules(rules, profile);
                var applicationStatus = GetCurrentApplicationStatus(scheme);
                var sourceUrl = FirstNonEmpty(scheme.OfficialRuleSource, scheme.OfficialSourceUrl, NormalizeOfficialDomain(scheme.Source?.OfficialDomain));
                var owningAuthority = GetOwningAuthority(scheme);

                var schemeData = new
                {
                    id = scheme.Id,
                    name = scheme.Name,
                    description = scheme.Description,
                    benefitType = scheme.BenefitType,
                    schemeCategory = scheme.SchemeCategory,
                    purpose = scheme.Purpose,
                    ministry = scheme.Ministry,
                    department = scheme.Department,
                    owningAuthority,
                    governmentLevel = scheme.GovernmentLevel ?? scheme.Scope,
                    applicationRoute = GetApplicationRoute(scheme, evalRes, applicationStatus),
                    applicationStatus,
                    applicationUrl = IsTrustedApplicationUrl(scheme) ? scheme.OfficialApplicationUrl : null,
                    verificationStatus = scheme.VerificationStatus,
                    lastVerified = scheme.LastVerified?.ToString("dd MMM yyyy") ?? scheme.Source?.LastVerified?.ToString("dd MMM yyyy") ?? "",
                    officialSource = scheme.Source?.SourceName ?? owningAuthority,
                    sourceUrl,
                    ruleComparisons = evalRes.Passed.Concat(evalRes.Failed).Concat(evalRes.Missing.Select(m => new { ruleName = m, status = "Missing" })).ToList(),
                    missingRules = evalRes.Missing
                };

                if (evalRes.EligibilityState == "Not Eligible") notEligible.Add(schemeData);
                else if (evalRes.EligibilityState == "More Information Needed") moreInfoNeeded.Add(schemeData);
                else recommended.Add(schemeData);
            }

            return Ok(new { recommended, otherEligible = Array.Empty<object>(), moreInfoNeeded, notEligible });
        }

        [HttpPost("dynamic-questions")]
        public IActionResult GetDynamicQuestions([FromBody] UserProfile profile)
        {
            var targetPurpose = NormalizePurpose(profile.Purpose);
            var schemes = _context.Schemes
                .AsNoTracking()
                .Include(s => s.Source)
                .Where(s => s.IsActive && s.DataOrigin == "OFFICIAL" &&
                    (s.VerificationStatus == "Verified" || s.VerificationStatus == "VERIFIED"))
                .ToList();
            var allRules = _context.SchemeEligibilityRules.AsNoTracking().ToList();
            var unresolvedConflictSchemeIds = _context.SourceConflicts
                .AsNoTracking()
                .Where(c => c.Status != "RESOLVED" && c.Status != "Resolved" && c.Status != "CLOSED" && c.Status != "Closed")
                .Select(c => c.SchemeId)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            var missingRules = new Dictionary<string, SchemeEligibilityRule>(StringComparer.OrdinalIgnoreCase);

            foreach (var scheme in schemes)
            {
                var rules = allRules.Where(r => r.SchemeId == scheme.Id && !OutputOnlyRuleFields.Contains(r.Field)).ToList();
                if (!IsCitizenPublishable(scheme, scheme.Source, rules, unresolvedConflictSchemeIds.Contains(scheme.Id))) continue;
                if (!PurposeMatches(scheme, profile.Purpose, targetPurpose) || !GeographyMatches(scheme, profile.State, profile.District)) continue;

                var evalRes = EvaluateRules(rules, profile);
                if (evalRes.EligibilityState == "Not Eligible") continue;

                foreach (var field in evalRes.Missing)
                {
                    if (!missingRules.ContainsKey(field))
                    {
                        var sourceRule = rules.FirstOrDefault(r => string.Equals(r.Field, field, StringComparison.OrdinalIgnoreCase));
                        if (sourceRule != null) missingRules[field] = sourceRule;
                    }
                }
            }

            var questions = new List<object>();
            var handled = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            void Add(string field, object question)
            {
                questions.Add(question);
                handled.Add(field);
            }

            if (missingRules.ContainsKey("ProjectCostMax") || missingRules.ContainsKey("ProjectCostMin") || missingRules.ContainsKey("projectCost"))
            {
                var field = missingRules.ContainsKey("ProjectCostMax") ? "ProjectCostMax" : missingRules.ContainsKey("ProjectCostMin") ? "ProjectCostMin" : "projectCost";
                Add(field, new { id = field, type = "currency", label = "Estimated Project Cost (₹)", helpText = "Required by at least one current verified candidate scheme." });
                handled.Add("ProjectCostMax"); handled.Add("ProjectCostMin"); handled.Add("projectCost");
            }
            if (missingRules.ContainsKey("BusinessActivity") || missingRules.ContainsKey("businessActivity"))
            {
                Add("BusinessActivity", new { id = "BusinessActivity", type = "taxonomy", label = "Business / Economic Activity", helpText = "Asked because a current verified candidate scheme has an activity rule." });
                handled.Add("businessActivity");
            }
            if (missingRules.ContainsKey("Gender") && string.IsNullOrWhiteSpace(profile.Gender))
                Add("Gender", new { id = "Gender", type = "single_choice", label = "Gender", options = new[] { new { label = "Female", value = "Female" }, new { label = "Male", value = "Male" }, new { label = "Other", value = "Other" } } });
            if (missingRules.ContainsKey("IsPwD") && !profile.IsPwD.HasValue)
                Add("IsPwD", new { id = "IsPwD", type = "single_choice", label = "Are you a Person with Disability (PwD)?", options = new[] { new { label = "Yes", value = "true" }, new { label = "No", value = "false" } } });
            if (missingRules.ContainsKey("DisabilityPercentageMin") && !ResolveDisabilityPercentage(profile).HasValue && profile.IsPwD != false)
                Add("DisabilityPercentageMin", new { id = "DisabilityPercentage", type = "numeric", label = "Disability Percentage (%)", helpText = "The eligibility threshold itself is evaluated from the verified scheme rule." });
            if (missingRules.ContainsKey("ApplicantType") && string.IsNullOrWhiteSpace(profile.ApplicantType))
                Add("ApplicantType", new { id = "ApplicantType", type = "single_choice", label = "Applicant Type", options = new[] { new { label = "Individual", value = "Individual" }, new { label = "Self-Help Group (SHG)", value = "SHG" } } });

            foreach (var entry in missingRules.Where(kvp => !handled.Contains(kvp.Key)))
            {
                var field = entry.Key;
                var rule = entry.Value;
                var label = HumanizeField(field);
                var op = rule.Operator ?? string.Empty;

                if (op is "LessThanOrEqual" or "GreaterThanOrEqual" or "LessThan" or "GreaterThan" or "Min" or "Max")
                {
                    questions.Add(new { id = field, type = "numeric", label, helpText = "Required by a current verified Government scheme rule." });
                }
                else if ((op == "Equals" || op == "InList") && !string.IsNullOrWhiteSpace(rule.Value))
                {
                    var options = rule.Value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                        .Distinct(StringComparer.OrdinalIgnoreCase)
                        .Select(v => new { label = v, value = v })
                        .ToArray();
                    if (options.Length > 0) questions.Add(new { id = field, type = "single_choice", label, options });
                    else questions.Add(new { id = field, type = "text", label });
                }
                else
                {
                    questions.Add(new { id = field, type = "text", label, helpText = "Required by a current verified Government scheme rule." });
                }
            }

            return Ok(questions);
        }

        private static string NormalizePurpose(string? purpose)
        {
            var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["Business"] = "Income-Generating Activities",
                ["Business & Entrepreneurship"] = "Income-Generating Activities",
                ["Entrepreneurship"] = "Income-Generating Activities",
                ["Income Generating"] = "Income-Generating Activities"
            };
            if (string.IsNullOrWhiteSpace(purpose)) return string.Empty;
            return map.TryGetValue(purpose, out var mapped) ? mapped : purpose;
        }

        private static bool PurposeMatches(Scheme scheme, string? rawPurpose, string targetPurpose)
        {
            if (string.IsNullOrWhiteSpace(rawPurpose)) return true;
            if (string.IsNullOrWhiteSpace(scheme.Purpose)) return false;
            return string.Equals(scheme.Purpose, targetPurpose, StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(scheme.Purpose, rawPurpose, StringComparison.OrdinalIgnoreCase);
        }

        private static bool GeographyMatches(Scheme scheme, string? state, string? district)
        {
            if (string.IsNullOrWhiteSpace(state)) return true;
            var level = scheme.GovernmentLevel ?? scheme.Scope ?? string.Empty;
            var applicability = scheme.ApplicableStateUT ?? string.Empty;
            var districtApplicability = scheme.ApplicableDistrict ?? string.Empty;

            if (string.Equals(level, "Central", StringComparison.OrdinalIgnoreCase) && string.IsNullOrWhiteSpace(applicability)) return true;
            if (!string.IsNullOrWhiteSpace(applicability) &&
                !applicability.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                    .Any(v => string.Equals(v, state, StringComparison.OrdinalIgnoreCase) || string.Equals(v, "ALL", StringComparison.OrdinalIgnoreCase) || string.Equals(v, "ALL INDIA", StringComparison.OrdinalIgnoreCase)))
                return false;

            if (!string.IsNullOrWhiteSpace(districtApplicability) && !string.IsNullOrWhiteSpace(district) &&
                !districtApplicability.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                    .Any(v => string.Equals(v, district, StringComparison.OrdinalIgnoreCase) || string.Equals(v, "ALL", StringComparison.OrdinalIgnoreCase)))
                return false;

            if ((string.Equals(level, "State", StringComparison.OrdinalIgnoreCase) || string.Equals(level, "UT", StringComparison.OrdinalIgnoreCase)) && string.IsNullOrWhiteSpace(applicability))
                return false;

            return true;
        }

        private static bool IsCitizenPublishable(Scheme scheme, GovernmentSource? source, List<SchemeEligibilityRule> rules, bool hasCriticalConflict)
        {
            if (!scheme.IsActive) return false;
            if (!string.Equals(scheme.DataOrigin, "OFFICIAL", StringComparison.OrdinalIgnoreCase)) return false;
            if (ExcludedOrigins.Any(o => string.Equals(o, scheme.DataOrigin, StringComparison.OrdinalIgnoreCase))) return false;
            if (!string.Equals(scheme.VerificationStatus, "VERIFIED", StringComparison.OrdinalIgnoreCase) && !string.Equals(scheme.VerificationStatus, "Verified", StringComparison.OrdinalIgnoreCase)) return false;
            if (string.IsNullOrWhiteSpace(scheme.Name) || SyntheticNamePattern.IsMatch(scheme.Name)) return false;
            if (source == null || string.IsNullOrWhiteSpace(source.SourceName)) return false;
            if (string.IsNullOrWhiteSpace(GetOwningAuthority(scheme))) return false;
            if (string.IsNullOrWhiteSpace(scheme.OfficialRuleSource) && string.IsNullOrWhiteSpace(scheme.OfficialSourceUrl) && string.IsNullOrWhiteSpace(source.OfficialDomain)) return false;
            if (hasCriticalConflict) return false;
            if (string.Equals(scheme.LifecycleStatus, "SUPERSEDED", StringComparison.OrdinalIgnoreCase) || string.Equals(scheme.LifecycleStatus, "DISCONTINUED", StringComparison.OrdinalIgnoreCase)) return false;

            var meaningfulRules = rules.Where(r => !OutputOnlyRuleFields.Contains(r.Field)).ToList();
            if (meaningfulRules.Count == 0) return false; // no known rule is NOT equivalent to universally eligible
            if (!meaningfulRules.Any(r => r.Mandatory)) return false;

            return true;
        }

        private static bool IsResolvedConflict(string? status) =>
            string.Equals(status, "RESOLVED", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(status, "CLOSED", StringComparison.OrdinalIgnoreCase);

        private static string GetOwningAuthority(Scheme scheme)
        {
            return FirstNonEmpty(scheme.OwningAuthority,
                string.Join(" · ", new[] { scheme.Ministry, scheme.Department }.Where(v => !string.IsNullOrWhiteSpace(v))),
                scheme.ImplementingAgency,
                scheme.Source?.Ministry,
                scheme.Source?.Department,
                scheme.Source?.SourceName);
        }

        private static string FirstNonEmpty(params string?[] values) => values.FirstOrDefault(v => !string.IsNullOrWhiteSpace(v)) ?? string.Empty;

        private static string? NormalizeOfficialDomain(string? domain)
        {
            if (string.IsNullOrWhiteSpace(domain)) return null;
            if (Uri.TryCreate(domain, UriKind.Absolute, out var absolute)) return absolute.ToString();
            return Uri.TryCreate($"https://{domain.Trim('/')}", UriKind.Absolute, out var uri) ? uri.ToString() : null;
        }

        private static bool IsTrustedApplicationUrl(Scheme scheme)
        {
            if (string.IsNullOrWhiteSpace(scheme.OfficialApplicationUrl)) return false;
            return Uri.TryCreate(scheme.OfficialApplicationUrl, UriKind.Absolute, out var uri) && (uri.Scheme == Uri.UriSchemeHttps || uri.Scheme == Uri.UriSchemeHttp);
        }

        private static string GetCurrentApplicationStatus(Scheme scheme)
        {
            var now = DateTime.UtcNow.Date;
            var windows = scheme.ApplicationWindows?.OrderByDescending(w => w.EndDate ?? DateTime.MaxValue).ToList() ?? new List<SchemeApplicationWindow>();
            var current = windows.FirstOrDefault(w => (!w.StartDate.HasValue || w.StartDate.Value.Date <= now) && (!w.EndDate.HasValue || w.EndDate.Value.Date >= now));
            if (current != null)
            {
                if (!string.IsNullOrWhiteSpace(current.Status)) return current.Status!.ToUpperInvariant();
                return "OPEN";
            }

            var future = windows.Where(w => w.StartDate.HasValue && w.StartDate.Value.Date > now).OrderBy(w => w.StartDate).FirstOrDefault();
            if (future != null) return "NOT_YET_OPEN";
            if (windows.Any(w => w.EndDate.HasValue && w.EndDate.Value.Date < now)) return "CLOSED";

            if (scheme.ApplicationStartDate.HasValue || scheme.ApplicationEndDate.HasValue)
            {
                if (scheme.ApplicationStartDate.HasValue && now < scheme.ApplicationStartDate.Value.Date) return "NOT_YET_OPEN";
                if (scheme.ApplicationEndDate.HasValue && now > scheme.ApplicationEndDate.Value.Date) return "CLOSED";
                return "OPEN";
            }

            if (string.Equals(scheme.ApplicationMode, "NO_APPLICATION_REQUIRED", StringComparison.OrdinalIgnoreCase)) return "NOT_APPLICABLE";
            return "UNKNOWN";
        }

        private static string GetApplicationRoute(Scheme scheme, (string EligibilityState, List<object> Passed, List<object> Failed, List<string> Missing) evalRes, string applicationStatus)
        {
            if (evalRes.EligibilityState == "Not Eligible") return "Not Eligible";
            if (applicationStatus == "CLOSED") return "Applications Closed";
            if (applicationStatus == "NOT_YET_OPEN") return "Not Yet Open";

            var mode = scheme.ApplicationMode ?? string.Empty;
            if (string.Equals(mode, "NO_APPLICATION_REQUIRED", StringComparison.OrdinalIgnoreCase)) return "No Application Required";
            if (string.Equals(mode, "PARTNER_ROUTED", StringComparison.OrdinalIgnoreCase)) return "Find Authorized Partner";
            if (string.Equals(mode, "INSTITUTION_ROUTED", StringComparison.OrdinalIgnoreCase)) return "Apply Through Institution";
            if (string.Equals(mode, "CSC_ROUTED", StringComparison.OrdinalIgnoreCase)) return "Apply Through CSC";
            if (string.Equals(mode, "OFFLINE", StringComparison.OrdinalIgnoreCase)) return "View Offline Application Process";

            if (string.Equals(mode, "OFFICIAL_ONLINE_PORTAL", StringComparison.OrdinalIgnoreCase) && applicationStatus == "OPEN" && IsTrustedApplicationUrl(scheme))
                return !string.IsNullOrWhiteSpace(scheme.ApplicationPortal) ? $"Apply on {scheme.ApplicationPortal}" : "Apply on Official Portal";

            if (applicationStatus == "UNKNOWN") return "Application Status Not Verified";
            if (applicationStatus == "NOT_APPLICABLE") return "View Official Scheme Information";
            return "Application Route Not Verified";
        }

        private static (string EligibilityState, List<object> Passed, List<object> Failed, List<string> Missing) EvaluateRules(List<SchemeEligibilityRule> rules, UserProfile profile)
        {
            var passed = new List<object>();
            var failed = new List<object>();
            var missing = new List<string>();

            foreach (var rule in rules.OrderBy(r => r.EvaluationOrder))
            {
                if (OutputOnlyRuleFields.Contains(rule.Field)) continue;

                var userValue = ResolveValue(profile, rule.Field);
                if (userValue == null || string.IsNullOrWhiteSpace(userValue.ToString()))
                {
                    if (rule.Mandatory) missing.Add(rule.Field);
                    continue;
                }

                var isPass = false;
                var expectedCondition = rule.Value ?? string.Empty;
                var op = rule.Operator ?? string.Empty;
                var userValueStr = userValue.ToString() ?? string.Empty;

                if (op == "Equals" || op == "InList")
                {
                    var vals = (rule.Value ?? string.Empty).Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                    isPass = vals.Any(v => string.Equals(v, userValueStr, StringComparison.OrdinalIgnoreCase));
                }
                else if (TryNumeric(userValueStr, out var userNumber) && decimal.TryParse(rule.Value, out var ruleNumber))
                {
                    if (op == "LessThanOrEqual" || op == "Max") isPass = userNumber <= ruleNumber;
                    else if (op == "GreaterThanOrEqual" || op == "Min") isPass = userNumber >= ruleNumber;
                    else if (op == "GreaterThan") isPass = userNumber > ruleNumber;
                    else if (op == "LessThan") isPass = userNumber < ruleNumber;
                }

                if (isPass) passed.Add(new { ruleName = HumanizeField(rule.Field), userValue = userValueStr, schemeCondition = expectedCondition, status = "Matched" });
                else failed.Add(new { ruleName = HumanizeField(rule.Field), userValue = userValueStr, schemeCondition = expectedCondition, status = "Failed" });
            }

            var state = failed.Any() ? "Not Eligible" : missing.Any() ? "More Information Needed" : "Eligible";
            return (state, passed, failed, missing.Distinct(StringComparer.OrdinalIgnoreCase).Select(HumanizeField).ToList());
        }

        private static bool TryNumeric(string raw, out decimal value)
        {
            var cleaned = raw.Replace(",", string.Empty).Replace("₹", string.Empty).Replace("%", string.Empty).Trim();
            return decimal.TryParse(cleaned, out value);
        }

        private static int? ResolveDisabilityPercentage(UserProfile profile)
        {
            if (profile.DisabilityPercentage.HasValue) return profile.DisabilityPercentage;
            if (profile.DynamicAnswers != null && profile.DynamicAnswers.TryGetValue("DisabilityPercentage", out var dynamicValue) && int.TryParse(dynamicValue, out var parsed)) return parsed;
            return null;
        }

        private static object? ResolveValue(UserProfile profile, string field)
        {
            if (field.Equals("Community", StringComparison.OrdinalIgnoreCase) || field.Equals("category", StringComparison.OrdinalIgnoreCase)) return profile.Category;
            if (field.Equals("AnnualFamilyIncome", StringComparison.OrdinalIgnoreCase) || field.Equals("income", StringComparison.OrdinalIgnoreCase)) return profile.Income;
            if (field.Equals("State", StringComparison.OrdinalIgnoreCase) || field.Equals("state", StringComparison.OrdinalIgnoreCase)) return profile.State;
            if (field.Equals("District", StringComparison.OrdinalIgnoreCase) || field.Equals("district", StringComparison.OrdinalIgnoreCase)) return profile.District;
            if (field.Equals("Gender", StringComparison.OrdinalIgnoreCase)) return profile.Gender;
            if (field.Equals("IsPwD", StringComparison.OrdinalIgnoreCase)) return profile.IsPwD;
            if (field.Equals("DisabilityPercentageMin", StringComparison.OrdinalIgnoreCase) || field.Equals("DisabilityPercentage", StringComparison.OrdinalIgnoreCase)) return ResolveDisabilityPercentage(profile);
            if (field.Equals("ApplicantType", StringComparison.OrdinalIgnoreCase)) return profile.ApplicantType;
            if (field.Equals("EWS", StringComparison.OrdinalIgnoreCase) || field.Equals("IsEWS", StringComparison.OrdinalIgnoreCase)) return profile.IsEws;
            if (field.Equals("Minority", StringComparison.OrdinalIgnoreCase) || field.Equals("IsMinority", StringComparison.OrdinalIgnoreCase)) return profile.IsMinority;
            if (field.Equals("ExServiceman", StringComparison.OrdinalIgnoreCase) || field.Equals("IsExServiceman", StringComparison.OrdinalIgnoreCase)) return profile.IsExServiceman;
            if (field.StartsWith("Age", StringComparison.OrdinalIgnoreCase) && profile.Dob.HasValue) return CalculateAge(profile.Dob.Value.Date, DateTime.UtcNow.Date);

            if (field.StartsWith("ProjectCost", StringComparison.OrdinalIgnoreCase) && profile.DynamicAnswers != null)
            {
                if (profile.DynamicAnswers.TryGetValue("ProjectCostMax", out var projectCostMax)) return projectCostMax;
                if (profile.DynamicAnswers.TryGetValue("ProjectCostMin", out var projectCostMin)) return projectCostMin;
                if (profile.DynamicAnswers.TryGetValue("projectCost", out var projectCost)) return projectCost;
            }

            if (profile.DynamicAnswers != null)
            {
                if (profile.DynamicAnswers.TryGetValue(field, out var exact)) return exact;
                var camelField = char.ToLowerInvariant(field[0]) + field[1..];
                if (profile.DynamicAnswers.TryGetValue(camelField, out var camel)) return camel;
            }

            var prop = typeof(UserProfile).GetProperties().FirstOrDefault(p => string.Equals(p.Name, field, StringComparison.OrdinalIgnoreCase));
            return prop?.GetValue(profile);
        }

        private static int CalculateAge(DateTime dob, DateTime today)
        {
            var age = today.Year - dob.Year;
            if (dob.Date > today.AddYears(-age)) age--;
            return age;
        }

        private static string HumanizeField(string field)
        {
            if (string.IsNullOrWhiteSpace(field)) return "Additional information";
            var normalized = Regex.Replace(field, "(Min|Max)$", string.Empty);
            normalized = Regex.Replace(normalized, "([a-z0-9])([A-Z])", "$1 $2");
            return normalized.Replace("Is Pw D", "PwD").Trim();
        }
    }

    [ApiController]
    [Route("api/locations")]
    public class LocationController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public LocationController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("districts")]
        public IActionResult GetDistricts([FromQuery] string state)
        {
            if (string.IsNullOrWhiteSpace(state)) return BadRequest(new { message = "State/UT is required." });

            var parent = _context.LocationMaster.AsNoTracking().FirstOrDefault(l =>
                (l.Type == "State" || l.Type == "UT") && l.Code == state);

            if (parent == null) return Ok(Array.Empty<object>());

            var districts = _context.LocationMaster.AsNoTracking()
                .Where(l => l.Type == "District" && l.ParentId == parent.Id)
                .OrderBy(l => l.Name)
                .Select(l => new { code = l.Code, name = l.Name })
                .ToList();

            return Ok(districts);
        }
    }

    public class UserProfile
    {
        public string? Purpose { get; set; }
        public string? BeneficiaryType { get; set; }
        public DateTime? Dob { get; set; }
        public string? Category { get; set; }
        public string? Income { get; set; }
        public string? State { get; set; }
        public string? District { get; set; }
        public string? Gender { get; set; }
        public bool? IsPwD { get; set; }
        public int? DisabilityPercentage { get; set; }
        public bool? IsEws { get; set; }
        public bool? IsMinority { get; set; }
        public bool? IsExServiceman { get; set; }
        public string? ApplicantType { get; set; }
        public Dictionary<string, string>? DynamicAnswers { get; set; }
    }
}
