using System;

namespace ArthSetuBackend.Models
{
    public class GovernmentSource
    {
        public string Id { get; set; } = string.Empty;
        public string SourceName { get; set; } = string.Empty;
        public string? Ministry { get; set; }
        public string? Department { get; set; }
        public string? ImplementingAgency { get; set; }
        public string? OfficialDomain { get; set; }
        public string? GovernmentLevel { get; set; } // Central / State / UT
        public string? State { get; set; } // State / UT if applicable
        public string? SourceCategory { get; set; }
        public string? SourceType { get; set; }
        public string? IngestionMethod { get; set; }
        public bool ApiAvailable { get; set; }
        public string? ConnectionStatus { get; set; }
        public DateTime? LastSync { get; set; }
        public DateTime? LastSuccessfulSync { get; set; }
        public DateTime? LastVerified { get; set; }
        public string? FreshnessStatus { get; set; }
        public DateTime? LastAttemptedSyncAt { get; set; }
        public string? LastError { get; set; }
        public DateTime? SourceUpdatedAt { get; set; }
        public string? SourceHealth { get; set; }
        public string? Terms { get; set; }
        public string? CoverageStatus { get; set; }
        public string? FailureInformation { get; set; }
        public bool IsActive { get; set; }
        public string? VerificationStatus { get; set; }
    }
}

