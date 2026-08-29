using System;
using System.ComponentModel.DataAnnotations;

namespace ArthSetu.Models
{
    public class GovernmentSource
    {
        [Key]
        public Guid SourceId { get; set; }
        public string SourceName { get; set; }
        public string OwningMinistry { get; set; }
        public string Department { get; set; }
        public string ImplementingOrganization { get; set; }
        public string OfficialDomain { get; set; }
        
        // Central / State / UT
        public string GovernmentLevel { get; set; }
        public string StateOrUT { get; set; }
        
        public string SourceCategory { get; set; }
        public string SourceType { get; set; }
        public string IngestionMethod { get; set; }
        
        public bool ApiAvailable { get; set; }
        public string ApiReference { get; set; }
        
        public DateTime? LastSync { get; set; }
        public DateTime? LastSuccessfulSync { get; set; }
        public DateTime? LastVerified { get; set; }
        
        public string SourceHealth { get; set; }
        // Pending Integration, Connected, Verified, Sync Failed, Temporarily Unavailable, Needs Review, Disabled
        public string SourceStatus { get; set; } 
        
        public string TermsAccessNotes { get; set; }
        public string FailureMessage { get; set; }
        
        public bool IsActive { get; set; }
    }
}
