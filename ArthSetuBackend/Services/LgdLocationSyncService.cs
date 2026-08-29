using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ArthSetuBackend.Data;
using ArthSetuBackend.Models;
using System.Linq;

namespace ArthSetuBackend.Services
{
    public class LgdLocationSyncService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<LgdLocationSyncService> _logger;

        public LgdLocationSyncService(ApplicationDbContext context, ILogger<LgdLocationSyncService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task SyncLgdDataAsync()
        {
            _logger.LogInformation("Starting LGD Location Sync...");

            var lgdSource = _context.GovernmentSources.FirstOrDefault(s => s.Id == "LGD");
            if (lgdSource == null)
            {
                lgdSource = new GovernmentSource
                {
                    Id = "LGD",
                    SourceName = "Local Government Directory",
                    Ministry = "Ministry of Panchayati Raj",
                    OfficialDomain = "lgdirectory.gov.in",
                    SourceType = "Administrative Location Master",
                    IngestionMethod = "API Setu / NAPIX",
                    ApiAvailable = true,
                    IsActive = true
                };
                _context.GovernmentSources.Add(lgdSource);
            }

            lgdSource.LastAttemptedSyncAt = DateTime.UtcNow;

            string apiEndpoint = "https://lgdirectory.gov.in/api/v1/states"; 
            bool hasNapixCredentials = false; 

            if (!hasNapixCredentials)
            {
                var errorMsg = "Requires official NAPIX credentials/authorization for API access. Captcha blocked for direct downloads.";
                _logger.LogWarning($"LGD Sync Blocked: {errorMsg}");
                lgdSource.ConnectionStatus = "BLOCKED";
                lgdSource.VerificationStatus = "BLOCKED";
                lgdSource.FailureInformation = errorMsg;
                lgdSource.LastError = errorMsg;
                
                var snapshot = new SourceSnapshot
                {
                    SourceId = "LGD",
                    SourceUrl = apiEndpoint,
                    SnapshotDate = DateTime.UtcNow,
                    VerificationStatus = "BLOCKED",
                    ContentHash = "NONE",
                    
                };
                _context.SourceSnapshots.Add(snapshot);
                await _context.SaveChangesAsync();
                return;
            }

            await _context.SaveChangesAsync();
        }
    }
}
