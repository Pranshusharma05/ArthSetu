using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ArthSetu.Models
{
    public class Scheme
    {
        [Key]
        public Guid SchemeId { get; set; }
        public string OfficialSchemeId { get; set; }
        public string SchemeName { get; set; }
        public string ShortDescription { get; set; }
        
        public string SchemeCategory { get; set; }
        public string Purpose { get; set; }
        public string BenefitType { get; set; } // Loan, Subsidy, Scholarship, Grant, etc.
        
        public string GovernmentLevel { get; set; }
        public string OwningMinistry { get; set; }
        public string Department { get; set; }
        public string ImplementingAgency { get; set; }
        
        public bool ActiveStatus { get; set; }
        public string OfficialApplicationUrl { get; set; }
        public string OfficialSourceUrl { get; set; }
        
        public DateTime? EffectiveFrom { get; set; }
        public DateTime? EffectiveUntil { get; set; }
        
        public DateTime? LastFetched { get; set; }
        public DateTime? LastVerified { get; set; }
        public string VerificationStatus { get; set; }
        
        public string CurrentPublishedVersion { get; set; }

        public ICollection<SchemeComponent> Components { get; set; }
        public ICollection<SchemeEligibilityRule> EligibilityRules { get; set; }
        public ICollection<SchemeDocument> Documents { get; set; }
    }
}
