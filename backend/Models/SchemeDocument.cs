using System;
using System.ComponentModel.DataAnnotations;

namespace ArthSetu.Models
{
    public class SchemeDocument
    {
        [Key]
        public Guid DocumentId { get; set; }
        
        public Guid SchemeId { get; set; }
        public Scheme Scheme { get; set; }
        
        public Guid? ComponentId { get; set; }
        public SchemeComponent Component { get; set; }
        
        public string DocumentName { get; set; }
        public bool IsRequired { get; set; }
        
        // "Conditional" if it's required only under certain conditions
        public string Condition { get; set; }
        
        public string Source { get; set; }
        public DateTime? LastVerified { get; set; }
    }
}
