using System;

namespace ArthSetuBackend.Models
{
    public class Scheme
    {
        public string Id { get; set; } = string.Empty;
        public string? OfficialSchemeId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? Purpose { get; set; }
        public string? SchemeCategory { get; set; }
        public string? BenefitType { get; set; }
        public string? Ministry { get; set; }
        public string? Department { get; set; }
        public string? ImplementingAgency { get; set; }
        public string? Scope { get; set; } // Central / State / UT Scope
        public string? GovernmentLevel { get; set; }
        public string? GeographicApplicabilityType { get; set; }
        public string? ApplicableStateUT { get; set; }
        public string? ApplicableDistrict { get; set; }
        public string? OfficialSourceUrl { get; set; }
        public string? OfficialApplicationUrl { get; set; }
        public string? CurrentPublishedVersion { get; set; }
        public DateTime? EffectiveFrom { get; set; }
        public DateTime? EffectiveUntil { get; set; }
        public DateTime? LastFetched { get; set; }
        public DateTime? LastVerified { get; set; }
        public string? VerificationStatus { get; set; }
        public bool IsActive { get; set; }
        public string DataOrigin { get; set; } = "Development Seed";
        public string? SourceId { get; set; }
        
        public GovernmentSource? Source { get; set; }
        
        public string? LifecycleStatus { get; set; }
        public DateTime? ApplicationStartDate { get; set; }
        public DateTime? ApplicationEndDate { get; set; }
        public string? OwningAuthority { get; set; }
        public string? OfficialRuleSource { get; set; }
                public string? ApplicationPortal { get; set; }
        public string? DiscoveryPortal { get; set; }
        public string? SupersededBy { get; set; }

        public string? ApplicationMode { get; set; } // OFFICIAL_ONLINE_PORTAL, PARTNER_ROUTED, INSTITUTION_ROUTED, CSC_ROUTED, OFFLINE, APPLICATION_STATUS_UNKNOWN
        public bool? LoginRequired { get; set; }
        public bool? ChannelPartnerRequired { get; set; }
        public bool? InstitutionRequired { get; set; }
        public bool? OfflineAllowed { get; set; }
        public string? ApplicationInstructions { get; set; }

        public ICollection<SchemeApplicationWindow> ApplicationWindows { get; set; } = new List<SchemeApplicationWindow>();
        public ICollection<SchemeBenefitComponent> BenefitComponents { get; set; } = new List<SchemeBenefitComponent>();
    }
}


