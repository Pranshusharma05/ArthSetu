using System;
using System.ComponentModel.DataAnnotations;

namespace ArthSetuBackend.Models
{
    public class DiscoveryCandidate
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        
        [Required]
        public string ExternalName { get; set; } = string.Empty;
        
        [Required]
        public string DiscoverySource { get; set; } = string.Empty;
        
        public string? GovernmentLevel { get; set; }
        public string? CandidateMinistry { get; set; }
        public string? CandidateDepartment { get; set; }
        public string? CandidateStateUT { get; set; }
        public string? CandidateOwningAuthority { get; set; }
        public string? CandidateApplicationPortal { get; set; }
        
        public DateTime DiscoveredAt { get; set; } = DateTime.UtcNow;
        
        public string ResolutionStatus { get; set; } = "PENDING";
        public string? CitizenClassification { get; set; }
    }
}

