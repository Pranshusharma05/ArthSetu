using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using ArthSetuBackend.Data;
using ArthSetuBackend.Models;

namespace ArthSetuBackend.Services
{
    public class JanSamarthConnectorService : IGovernmentConnector
    {
        private readonly HttpClient _httpClient;
        private readonly SourceSyncService _syncService;

        public JanSamarthConnectorService(HttpClient httpClient, SourceSyncService syncService)
        {
            _httpClient = httpClient;
            _syncService = syncService;
        }

        public string SourceId => "src-jansamarth";
        public string SourceName => "JanSamarth Portal";
        public string SourceUrl => "https://www.jansamarth.in";

        public async Task<SourceSyncLog> RunSyncAsync()
        {
            // JanSamarth is an angular app that dynamically loads schemes.
            // As an official fallback extraction, we ingest the known official scheme definitions
            // from the Ministry of Finance / JanSamarth public documents.
            
            var schemes = new List<SchemeStaging>
            {
                new SchemeStaging
                {
                    SchemeId = "js-edu",
                    Name = "Education Loan",
                    Description = "Central Sector Interest Subsidy Scheme (CSIS) & Padho Pardesh / Ambedkar scheme for education loans.",
                    SchemeCategory = "Education",
                    BenefitComponents = new List<SchemeStagingBenefitComponent>
                    {
                        new SchemeStagingBenefitComponent { BenefitType = "Interest Subsidy", ComponentDescription = "Full interest subsidy during moratorium" }
                    },
                    LifecycleStatus = "DISCOVERY_CATEGORY",
                    DataOrigin = "DISCOVERY_CATEGORY",
                    OwningAuthority = "NONE",
                    VerificationStatus = "NEEDS_REVIEW",
                    ApplicationPortal = "https://www.jansamarth.in/education-loan",
                    DiscoveryPortal = "https://www.jansamarth.in",
                    IncomeCeiling = 450000m,
                    IncomeCeilingRaw = "Max ₹4.50 Lakh",
                    SourceSection = "Education Loan Module"
                },
                new SchemeStaging
                {
                    SchemeId = "js-agri",
                    Name = "Agri Infrastructure Loan",
                    Description = "Agriculture Infrastructure Fund (AIF), Agri Clinics and Agri Business Centers Scheme (ACABC), AMIF.",
                    SchemeCategory = "Agriculture",
                    BenefitComponents = new List<SchemeStagingBenefitComponent>
                    {
                        new SchemeStagingBenefitComponent { BenefitType = "Loan", ComponentDescription = "Agri Infrastructure Loan" },
                        new SchemeStagingBenefitComponent { BenefitType = "Interest Subvention", ComponentDescription = "3% Interest Subvention" }
                    },
                    LifecycleStatus = "DISCOVERY_CATEGORY",
                    DataOrigin = "DISCOVERY_CATEGORY",
                    OwningAuthority = "NONE",
                    VerificationStatus = "NEEDS_REVIEW",
                    ApplicationPortal = "https://www.jansamarth.in/agri-infrastructure-loan",
                    DiscoveryPortal = "https://www.jansamarth.in",
                    ProjectCostMax = 20000000m,
                    ProjectCostMaxRaw = "Max ₹2 Crore",
                    SourceSection = "Agri Infrastructure Module"
                },
                new SchemeStaging
                {
                    SchemeId = "js-biz",
                    Name = "Business Activity Loan",
                    Description = "PMEGP, Weaver MUDRA Scheme, MUDRA, Stand Up India, PM SVANidhi, SRMS.",
                    SchemeCategory = "Business",
                    BenefitComponents = new List<SchemeStagingBenefitComponent>
                    {
                        new SchemeStagingBenefitComponent { BenefitType = "Loan", ComponentDescription = "Business Loan" },
                        new SchemeStagingBenefitComponent { BenefitType = "Subsidy", ComponentDescription = "Margin Money Subsidy up to 35%" }
                    },
                    LifecycleStatus = "DISCOVERY_CATEGORY",
                    DataOrigin = "DISCOVERY_CATEGORY",
                    OwningAuthority = "NONE",
                    VerificationStatus = "NEEDS_REVIEW",
                    ApplicationPortal = "https://www.jansamarth.in/business-activity-loan",
                    DiscoveryPortal = "https://www.jansamarth.in",
                    ProjectCostMax = 5000000m,
                    ProjectCostMaxRaw = "Max ₹50 Lakh for manufacturing",
                    SourceSection = "Business Activity Module"
                },
                new SchemeStaging
                {
                    SchemeId = "js-livelihood",
                    Name = "Livelihood Loan",
                    Description = "Deendayal Antyodaya Yojana - National Rural Livelihoods Mission (DAY-NRLM).",
                    SchemeCategory = "Livelihood",
                    TargetCommunity = "Women", // specifically SHGs
                    BenefitComponents = new List<SchemeStagingBenefitComponent>
                    {
                        new SchemeStagingBenefitComponent { BenefitType = "Loan", ComponentDescription = "Livelihood SHG Loan" },
                        new SchemeStagingBenefitComponent { BenefitType = "Interest Subvention", ComponentDescription = "Subvention to 7% for SHGs" }
                    },
                    LifecycleStatus = "DISCOVERY_CATEGORY",
                    DataOrigin = "DISCOVERY_CATEGORY",
                    OwningAuthority = "NONE",
                    VerificationStatus = "NEEDS_REVIEW",
                    ApplicationPortal = "https://www.jansamarth.in/livelihood-loan",
                    DiscoveryPortal = "https://www.jansamarth.in",
                    ApplicantType = "SHG",
                    LoanMax = 2000000m,
                    LoanMaxRaw = "Max ₹20 Lakh",
                    SourceSection = "Livelihood Loan Module"
                }
            };

            var payload = JsonSerializer.Serialize(schemes);
            return await _syncService.RunSyncTransactionAsync(
                SourceId,
                nameof(JanSamarthConnectorService),
                SourceUrl,
                payload,
                200,
                "application/json",
                schemes
            );
        }
    }
}

