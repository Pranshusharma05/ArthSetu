using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArthSetuBackend.Models
{
    public class SchemeSourceReference
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        
        [Required]
        public string SchemeId { get; set; } = string.Empty;
        
        [Required]
        public string SourceId { get; set; } = string.Empty;
        
        public string? ExternalReferenceId { get; set; }
        
        public string? Url { get; set; }
        
        public bool IsPrimary { get; set; } = false;
        
        [ForeignKey("SchemeId")]
        public Scheme? Scheme { get; set; }
        
        [ForeignKey("SourceId")]
        public GovernmentSource? Source { get; set; }
    }
}
