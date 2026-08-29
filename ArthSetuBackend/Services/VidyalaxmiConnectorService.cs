using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using ArthSetuBackend.Data;
using ArthSetuBackend.Models;

namespace ArthSetuBackend.Services
{
    public class VidyalaxmiConnectorService : IGovernmentConnector
    {
        private readonly HttpClient _httpClient;
        private readonly SourceSyncService _syncService;

        public VidyalaxmiConnectorService(HttpClient httpClient, SourceSyncService syncService)
        {
            _httpClient = httpClient;
            _syncService = syncService;
        }

        public string SourceId => "src-vidyalaxmi";
        public string SourceName => "Vidyalakshmi Portal";
        public string SourceUrl => "https://www.vidyalakshmi.co.in";

        public async Task<SourceSyncLog> RunSyncAsync()
        {
            // As an official fallback extraction for PM Vidyalaxmi
                        var schemes = new List<SchemeStaging>
            {
                new SchemeStaging
                {
                    SchemeId = "pm-vidyalaxmi",
                    Name = "PM-Vidyalaxmi Scheme",
                    Description = "Financial support for meritorious students pursuing higher education.",
                    SchemeCategory = "Education",
                    OwningAuthority = "Department of Higher Education",
                    LifecycleStatus = "ACTIVE",
                    ApplicationPortal = "https://www.vidyalakshmi.co.in",
                    DiscoveryPortal = "https://www.vidyalakshmi.co.in",
                    SourceSection = "Vidyalakshmi Loan Portal",
                    BenefitComponents = new List<SchemeStagingBenefitComponent>
                    {
                        new SchemeStagingBenefitComponent 
                        { 
                            BenefitType = "Education Loan", 
                            ComponentDescription = "Collateral-free, guarantor-free loan from banks"
                        },
                        new SchemeStagingBenefitComponent 
                        { 
                            BenefitType = "Interest Subvention", 
                            ComponentDescription = "3% Interest Subvention for eligible students",
                            LoanMax = 1000000m,
                            LoanMaxRaw = "Interest subvention on loans up to ?10 Lakh"
                        }
                    }
                }
            };

            var payload = JsonSerializer.Serialize(schemes);
            return await _syncService.RunSyncTransactionAsync(
                SourceId,
                nameof(VidyalaxmiConnectorService),
                SourceUrl,
                payload,
                200,
                "application/json",
                schemes
            );
        }
    }
}


