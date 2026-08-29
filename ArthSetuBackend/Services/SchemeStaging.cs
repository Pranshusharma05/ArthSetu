using System.Collections.Generic;

namespace ArthSetuBackend.Services
{
    public class SchemeStaging
    {
        public string SchemeId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string SchemeCategory { get; set; } = string.Empty;
        
        public string? TargetCommunity { get; set; }
        
        // New dimensions
        public string? Gender { get; set; }
        public bool? IsPwD { get; set; }
        public int? DisabilityPercentageMin { get; set; }
        public string? ApplicantType { get; set; }
        public int? GroupMinTargetPercentage { get; set; }

        public decimal? IncomeCeiling { get; set; }
        public string? IncomeCeilingRaw { get; set; }
        
        public decimal? ProjectCostMax { get; set; }
        public string? ProjectCostMaxRaw { get; set; }
        
        public decimal? ProjectCostMin { get; set; }
        public string? ProjectCostMinRaw { get; set; }
        
        public decimal? LoanMax { get; set; }
        public string? LoanMaxRaw { get; set; }
        
        public string? InterestRateRaw { get; set; }
        public string? TenureRaw { get; set; }
        public string? MoratoriumRaw { get; set; }
        
        public string? SourceSection { get; set; }

        // Phase 8B Additions
        public string? LifecycleStatus { get; set; }
        public DateTime? ApplicationStartDate { get; set; }
        public DateTime? ApplicationEndDate { get; set; }
        public string? OwningAuthority { get; set; }
        public string? OfficialRuleSource { get; set; }
        public string? ApplicationPortal { get; set; }
        public string? DiscoveryPortal { get; set; }

        public string DataOrigin { get; set; } = "IMPORTED";
        public string VerificationStatus { get; set; } = "NEEDS_REVIEW";

        public List<SchemeStagingApplicationWindow> ApplicationWindows { get; set; } = new List<SchemeStagingApplicationWindow>();
        public List<SchemeStagingBenefitComponent> BenefitComponents { get; set; } = new List<SchemeStagingBenefitComponent>();
    }

    public class SchemeStagingApplicationWindow
    {
        public string? Cycle { get; set; }
        public string? ApplicationType { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Status { get; set; }
        public string? SourceSnapshotId { get; set; }
        public DateTime? LastVerifiedAt { get; set; }
    }

    public class SchemeStagingBenefitComponent
    {
        public string? ComponentId { get; set; }
        public string BenefitType { get; set; } = string.Empty;
        public string? ComponentDescription { get; set; }
        
        // Component-specific rules
        public decimal? ProjectCostMin { get; set; }
        public string? ProjectCostMinRaw { get; set; }
        public decimal? ProjectCostMax { get; set; }
        public string? ProjectCostMaxRaw { get; set; }
        public decimal? LoanMax { get; set; }
        public string? LoanMaxRaw { get; set; }
        public decimal? LoanMin { get; set; }
        public string? LoanMinRaw { get; set; }
        public string? InterestRateRaw { get; set; }
        public string? TenureRaw { get; set; }
        public string? MoratoriumRaw { get; set; }
        public string? SpecialEligibilityRaw { get; set; }
        
        public string? SourceSection { get; set; }
    }
}
