using System;
using System.ComponentModel.DataAnnotations;

namespace ArthSetu.Models
{
    public class SourceConflict
    {
        [Key]
        public Guid ConflictId { get; set; }
        public Guid SchemeId { get; set; }
        public string Field { get; set; }
        
        public string CurrentVerifiedValue { get; set; }
        public string NewCandidateValue { get; set; }
        
        public string ExistingSource { get; set; }
        public string NewSource { get; set; }
        
        public DateTime? ExistingPublicationDate { get; set; }
        public DateTime? NewPublicationDate { get; set; }
        
        public string ConflictStatus { get; set; }
        public string AdminReviewStatus { get; set; }
    }
}
