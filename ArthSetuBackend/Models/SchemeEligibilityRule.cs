using System;

namespace ArthSetuBackend.Models
{
    public class SchemeEligibilityRule
    {
        public int Id { get; set; }
        public string SchemeId { get; set; } = string.Empty;
        public string? SchemeComponentId { get; set; }
        public int? SchemeBenefitComponentId { get; set; } // Phase 8B.1 normalized relation
        public SchemeBenefitComponent? SchemeBenefitComponent { get; set; }
        public string Field { get; set; } = string.Empty;
        public string Operator { get; set; } = string.Empty;
        public string? Value { get; set; }
        public string? SecondaryValue { get; set; }
        public bool Mandatory { get; set; }
        public DateTime? EffectiveFrom { get; set; }
        public DateTime? EffectiveUntil { get; set; }
        public string? SourceReference { get; set; }
        public string? VerificationStatus { get; set; }
        public int EvaluationOrder { get; set; }
        
        public Scheme? Scheme { get; set; }
    }
}
