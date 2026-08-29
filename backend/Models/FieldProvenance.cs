using System;
using System.ComponentModel.DataAnnotations;

namespace ArthSetu.Models
{
    public class FieldProvenance
    {
        [Key]
        public Guid ProvenanceId { get; set; }
        
        public Guid SchemeId { get; set; }
        public string Field { get; set; }
        public string Value { get; set; }
        
        public Guid? SourceId { get; set; }
        public GovernmentSource Source { get; set; }
        
        public string SourceUrl { get; set; }
        public string DocumentSectionPage { get; set; }
        
        public DateTime? PublishedDate { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public DateTime? VerifiedDate { get; set; }
        
        public string VerificationStatus { get; set; }
    }
}
