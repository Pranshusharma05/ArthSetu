using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using ArthSetuBackend.Data;
using ArthSetuBackend.Models;

namespace ArthSetuBackend.Services
{
    public class PmfmeConnectorService : IGovernmentConnector
    {
        private readonly HttpClient _httpClient;
        private readonly SourceSyncService _syncService;

        public PmfmeConnectorService(HttpClient httpClient, SourceSyncService syncService)
        {
            _httpClient = httpClient;
            _syncService = syncService;
        }

        public string SourceId => "src-pmfme";
        public string SourceName => "PM Formalization of Micro Food Processing Enterprises";
        public string SourceUrl => "https://pmfme.mofpi.gov.in";

        public async Task<SourceSyncLog> RunSyncAsync()
        {
            var schemes = new List<SchemeStaging>
            {
                new SchemeStaging
                {
                    SchemeId = "pmfme-individual",
                    Name = "PMFME Individual Micro Enterprises",
                    Description = "Credit-linked capital subsidy at 35% of the eligible project cost with a maximum ceiling of Rs.10.0 lakh per unit.",
                    SchemeCategory = "Business",
                    BenefitComponents = new List<SchemeStagingBenefitComponent>
                    {
                        new SchemeStagingBenefitComponent { BenefitType = "Subsidy", ComponentDescription = "Capital Subsidy at 35% up to ₹10 Lakh" }
                    },
                    LifecycleStatus = "ACTIVE",
                    ApplicationPortal = "https://pmfme.mofpi.gov.in",
                    DiscoveryPortal = "https://pmfme.mofpi.gov.in",
                    ProjectCostMax = 2850000m, // (approx corresponding to 10L subsidy at 35%)
                    ProjectCostMaxRaw = "Max subsidy ₹10 Lakh",
                    SourceSection = "PMFME Guidelines"
                }
            };

            var payload = JsonSerializer.Serialize(schemes);
            return await _syncService.RunSyncTransactionAsync(
                SourceId,
                nameof(PmfmeConnectorService),
                SourceUrl,
                payload,
                200,
                "application/json",
                schemes
            );
        }
    }
}
