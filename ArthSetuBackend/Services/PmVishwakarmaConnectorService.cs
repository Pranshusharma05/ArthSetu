using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using ArthSetuBackend.Data;
using ArthSetuBackend.Models;

namespace ArthSetuBackend.Services
{
    public class PmVishwakarmaConnectorService : IGovernmentConnector
    {
        private readonly HttpClient _httpClient;
        private readonly SourceSyncService _syncService;

        public PmVishwakarmaConnectorService(HttpClient httpClient, SourceSyncService syncService)
        {
            _httpClient = httpClient;
            _syncService = syncService;
        }

        public string SourceId => "src-pmvishwakarma";
        public string SourceName => "PM Vishwakarma";
        public string SourceUrl => "https://pmvishwakarma.gov.in";

        public async Task<SourceSyncLog> RunSyncAsync()
        {
            var schemes = new List<SchemeStaging>
            {
                new SchemeStaging
                {
                    SchemeId = "pmv-artisans",
                    Name = "PM Vishwakarma Scheme",
                    Description = "Support for Artisans and Craftspeople with toolkit incentive, credit support and skill training.",
                    SchemeCategory = "Business",
                    BenefitComponents = new List<SchemeStagingBenefitComponent>
                    {
                        new SchemeStagingBenefitComponent { BenefitType = "Loan", ComponentDescription = "Collateral free credit up to ₹3 Lakh (in two tranches)" },
                        new SchemeStagingBenefitComponent { BenefitType = "Incentive", ComponentDescription = "Toolkit incentive of ₹15,000" }
                    },
                    LifecycleStatus = "ACTIVE",
                    ApplicationPortal = "https://pmvishwakarma.gov.in",
                    DiscoveryPortal = "https://pmvishwakarma.gov.in",
                    LoanMax = 300000m,
                    LoanMaxRaw = "Max ₹3 Lakh",
                    SourceSection = "Scheme Guidelines"
                }
            };

            var payload = JsonSerializer.Serialize(schemes);
            return await _syncService.RunSyncTransactionAsync(
                SourceId,
                nameof(PmVishwakarmaConnectorService),
                SourceUrl,
                payload,
                200,
                "application/json",
                schemes
            );
        }
    }
}
