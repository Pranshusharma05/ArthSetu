using System;
using System.ComponentModel.DataAnnotations;

namespace ArthSetu.Models
{
    public class SourceSnapshot
    {
        [Key]
        public Guid SnapshotId { get; set; }
        public Guid SourceId { get; set; }
        
        public DateTime FetchTimestamp { get; set; }
        public string SourceVersionHash { get; set; }
        
        public string RawMetadata { get; set; }
        public string ProcessingStatus { get; set; }
        public string ParserStatus { get; set; }
        public string VerificationStatus { get; set; }
    }
}
