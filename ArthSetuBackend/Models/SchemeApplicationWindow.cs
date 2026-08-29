using System;

namespace ArthSetuBackend.Models
{
    public class SchemeApplicationWindow
    {
        public int Id { get; set; }
        public string SchemeId { get; set; } = string.Empty;
        public Scheme? Scheme { get; set; }
        
        public string? Cycle { get; set; } // e.g. "2024-25"
        public string? ApplicationType { get; set; } // "NEW" or "RENEWAL"
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Status { get; set; } // "OPEN", "CLOSED", "NOT_YET_OPEN", "UNKNOWN"
        public string? SourceSnapshotId { get; set; }
        public DateTime? LastVerifiedAt { get; set; }
    }
}
