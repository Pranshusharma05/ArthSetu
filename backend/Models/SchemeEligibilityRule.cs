using System;
using System.ComponentModel.DataAnnotations;

namespace ArthSetu.Models
{
    public class SchemeEligibilityRule
    {
        [Key]
        public Guid RuleId { get; set; }
        public Guid SchemeId { get; set; }
        public Scheme Scheme { get; set; }
        
        public Guid? ComponentId { get; set; }
        public SchemeComponent Component { get; set; }
        
        public string RuleField { get; set; } // Age, Income, Category, etc.
        public string RuleType { get; set; } 
        public string Operator { get; set; } // Equals, Min, Max, Range, InList, etc.
        public string Value { get; set; }
        public string SecondaryValue { get; set; }
        
        public bool IsMandatory { get; set; }
        
        public DateTime? EffectiveFrom { get; set; }
        public DateTime? EffectiveUntil { get; set; }
        
        public string SourceReference { get; set; }
        public string VerificationStatus { get; set; }
        public int Priority { get; set; }
    }
}
