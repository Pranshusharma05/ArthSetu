using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using ArthSetuBackend.Data;
using ArthSetuBackend.Models;

namespace ArthSetuBackend.Services
{
    public class PmegpConnectorService : IGovernmentConnector
    {
        private readonly HttpClient _httpClient;
        private readonly SourceSyncService _syncService;

        public PmegpConnectorService(HttpClient httpClient, SourceSyncService syncService)
        {
            _httpClient = httpClient;
            _syncService = syncService;
        }

        public string SourceId => "src-pmegp";
        public string SourceName => "PMEGP (KVIC)";
        public string SourceUrl => "https://www.kviconline.gov.in";

        public async Task<SourceSyncLog> RunSyncAsync()
        {
                        var schemes = new List<SchemeStaging>
            {
                new SchemeStaging
                {
                    SchemeId = "pmegp",
                    Name = "Prime Minister's Employment Generation Programme (PMEGP)",
                    Description = "Credit-linked subsidy programme for generating employment.",
                    SchemeCategory = "Business",
                    OwningAuthority = "Ministry of Micro, Small and Medium Enterprises",
                    LifecycleStatus = "ACTIVE",
                    ApplicationPortal = "https://www.kviconline.gov.in/pmegpeportal",
                    DiscoveryPortal = "https://www.kviconline.gov.in",
                    ProjectCostMax = 5000000m,
                    ProjectCostMaxRaw = "Max ?50 Lakh",
                    SourceSection = "PMEGP Guidelines",
                    BenefitComponents = new List<SchemeStagingBenefitComponent>
                    {
                        new SchemeStagingBenefitComponent 
                        { 
                            BenefitType = "Subsidy", 
                            ComponentDescription = "Manufacturing Sector",
                            ProjectCostMax = 5000000m,
                            ProjectCostMaxRaw = "Max ?50 Lakh"
                        },
                        new SchemeStagingBenefitComponent 
                        { 
                            BenefitType = "Subsidy", 
                            ComponentDescription = "Service Sector",
                            ProjectCostMax = 2000000m,
                            ProjectCostMaxRaw = "Max ?20 Lakh"
                        }
                    }
                }
            };

            var payload = JsonSerializer.Serialize(schemes);
            return await _syncService.RunSyncTransactionAsync(
                SourceId,
                nameof(PmegpConnectorService),
                SourceUrl,
                payload,
                200,
                "application/json",
                schemes
            );
        }
    }
}


