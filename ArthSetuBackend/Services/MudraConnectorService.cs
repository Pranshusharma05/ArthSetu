using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using ArthSetuBackend.Data;
using ArthSetuBackend.Models;

namespace ArthSetuBackend.Services
{
    public class MudraConnectorService : IGovernmentConnector
    {
        private readonly HttpClient _httpClient;
        private readonly SourceSyncService _syncService;

        public MudraConnectorService(HttpClient httpClient, SourceSyncService syncService)
        {
            _httpClient = httpClient;
            _syncService = syncService;
        }

        public string SourceId => "src-mudra";
        public string SourceName => "Pradhan Mantri MUDRA Yojana";
        public string SourceUrl => "https://www.mudra.org.in";

        public async Task<SourceSyncLog> RunSyncAsync()
        {
                        var schemes = new List<SchemeStaging>
            {
                new SchemeStaging
                {
                    SchemeId = "pmmy",
                    Name = "Pradhan Mantri MUDRA Yojana (PMMY)",
                    Description = "Loans up to ?20 Lakh under Shishu, Kishore, Tarun, and Tarun Plus categories.",
                    SchemeCategory = "Business",
                    OwningAuthority = "Department of Financial Services",
                    LifecycleStatus = "ACTIVE",
                    ApplicationPortal = "https://www.mudra.org.in",
                    DiscoveryPortal = "https://www.mudra.org.in",
                    LoanMax = 2000000m,
                    LoanMaxRaw = "Max ?20 Lakh",
                    SourceSection = "MUDRA Offerings",
                    BenefitComponents = new List<SchemeStagingBenefitComponent>
                    {
                        new SchemeStagingBenefitComponent 
                        { 
                            BenefitType = "Loan", 
                            ComponentDescription = "Shishu - Loans up to ?50,000",
                            LoanMax = 50000m,
                            LoanMaxRaw = "Max ?50,000"
                        },
                        new SchemeStagingBenefitComponent 
                        { 
                            BenefitType = "Loan", 
                            ComponentDescription = "Kishore - Loans above ?50,000 and up to ?5 lakh",
                            ProjectCostMin = 50000m,
                            ProjectCostMinRaw = "More than ?50,000",
                            LoanMax = 500000m,
                            LoanMaxRaw = "Max ?5 Lakh"
                        },
                        new SchemeStagingBenefitComponent 
                        { 
                            BenefitType = "Loan", 
                            ComponentDescription = "Tarun - Loans above ?5 lakh and up to ?10 lakh",
                            ProjectCostMin = 500000m,
                            ProjectCostMinRaw = "More than ?5 Lakh",
                            LoanMax = 1000000m,
                            LoanMaxRaw = "Max ?10 Lakh"
                        },
                        new SchemeStagingBenefitComponent 
                        { 
                            BenefitType = "Loan", 
                            ComponentDescription = "Tarun Plus - Loans above ?10 lakh and up to ?20 lakh",
                            ProjectCostMin = 1000000m,
                            ProjectCostMinRaw = "More than ?10 Lakh",
                            LoanMax = 2000000m,
                            LoanMaxRaw = "Max ?20 Lakh",
                            SpecialEligibilityRaw = "Must have successfully repaid previous Tarun loan"
                        }
                    }
                }
            };

            var payload = JsonSerializer.Serialize(schemes);
            return await _syncService.RunSyncTransactionAsync(
                SourceId,
                nameof(MudraConnectorService),
                SourceUrl,
                payload,
                200,
                "application/json",
                schemes
            );
        }
    }
}


