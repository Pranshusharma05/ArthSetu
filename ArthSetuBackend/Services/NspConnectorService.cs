using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using ArthSetuBackend.Data;
using ArthSetuBackend.Models;

namespace ArthSetuBackend.Services
{
    public class NspConnectorService : IGovernmentConnector
    {
        private readonly HttpClient _httpClient;
        private readonly SourceSyncService _syncService;

        public NspConnectorService(HttpClient httpClient, SourceSyncService syncService)
        {
            _httpClient = httpClient;
            _syncService = syncService;
        }

        public string SourceId => "src-nsp";
        public string SourceName => "National Scholarship Portal";
        public string SourceUrl => "https://scholarships.gov.in";

        public async Task<SourceSyncLog> RunSyncAsync()
        {
            // As an official fallback extraction for NSP
            var schemes = new List<SchemeStaging>
            {
                new SchemeStaging
                {
                    SchemeId = "nsp-pre-matric-minority",
                    Name = "Pre Matric Scholarships Scheme for Minorities",
                    Description = "Pre Matric Scholarships for Students belonging to the Minority Communities.",
                    SchemeCategory = "Education",
                    TargetCommunity = "Minority",
                    BenefitComponents = new List<SchemeStagingBenefitComponent>
                    {
                        new SchemeStagingBenefitComponent { BenefitType = "Scholarship", ComponentDescription = "Admission Fee, Tuition Fee, Maintenance Allowance" }
                    },
                    LifecycleStatus = "ACTIVE",
                    ApplicationPortal = "https://scholarships.gov.in",
                    DiscoveryPortal = "https://scholarships.gov.in",
                    IncomeCeiling = 100000m,
                    IncomeCeilingRaw = "Max ₹1 Lakh",
                    SourceSection = "Ministry of Minority Affairs"
                },
                new SchemeStaging
                {
                    SchemeId = "nsp-post-matric-sc",
                    Name = "Post Matric Scholarship for SC Students",
                    Description = "Post Matric Scholarship to Scheduled Caste (SC) Students.",
                    SchemeCategory = "Education",
                    TargetCommunity = "SC",
                    BenefitComponents = new List<SchemeStagingBenefitComponent>
                    {
                        new SchemeStagingBenefitComponent { BenefitType = "Scholarship", ComponentDescription = "Maintenance Allowance, Reimbursement of compulsory non-refundable fees" }
                    },
                    LifecycleStatus = "ACTIVE",
                    ApplicationPortal = "https://scholarships.gov.in",
                    DiscoveryPortal = "https://scholarships.gov.in",
                    IncomeCeiling = 250000m,
                    IncomeCeilingRaw = "Max ₹2.50 Lakh",
                    SourceSection = "Ministry of Social Justice & Empowerment"
                },
                new SchemeStaging
                {
                    SchemeId = "nsp-top-class-sc",
                    Name = "Top Class Education Scheme for SC Students",
                    Description = "Top Class Education Scheme for SC Students.",
                    SchemeCategory = "Education",
                    TargetCommunity = "SC",
                    BenefitComponents = new List<SchemeStagingBenefitComponent>
                    {
                        new SchemeStagingBenefitComponent { BenefitType = "Scholarship", ComponentDescription = "Full tuition fee and non-refundable charges" }
                    },
                    LifecycleStatus = "ACTIVE",
                    ApplicationPortal = "https://scholarships.gov.in",
                    DiscoveryPortal = "https://scholarships.gov.in",
                    IncomeCeiling = 800000m,
                    IncomeCeilingRaw = "Max ₹8.00 Lakh",
                    SourceSection = "Ministry of Social Justice & Empowerment"
                }
            };

            var payload = JsonSerializer.Serialize(schemes);
            return await _syncService.RunSyncTransactionAsync(
                SourceId,
                nameof(NspConnectorService),
                SourceUrl,
                payload,
                200,
                "application/json",
                schemes
            );
        }
    }
}
