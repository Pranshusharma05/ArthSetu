using System;
using System.ComponentModel.DataAnnotations;

namespace ArthSetu.Models
{
    public class SchemeVersion
    {
        [Key]
        public Guid VersionId { get; set; }
        public Guid SchemeId { get; set; }
        
        public string VersionNumber { get; set; }
        public DateTime? EffectiveDate { get; set; }
        
        public string Source { get; set; }
        public DateTime? PublicationDate { get; set; }
        public DateTime? VerificationDate { get; set; }
        
        public string Status { get; set; }
        public string ChangeSummary { get; set; }
        
        public DateTime CreatedAt { get; set; }
        public DateTime? PublishedAt { get; set; }
        
        public string SerializedSchemeData { get; set; } // Store the JSON snapshot of that version
    }
}
