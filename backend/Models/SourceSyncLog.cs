using System;
using System.ComponentModel.DataAnnotations;

namespace ArthSetu.Models
{
    public class SourceSyncLog
    {
        [Key]
        public Guid SyncLogId { get; set; }
        public Guid SourceId { get; set; }
        
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        
        public int RecordsFound { get; set; }
        public int NewSchemes { get; set; }
        public int UpdatedSchemes { get; set; }
        public int NoChanges { get; set; }
        
        public string Warnings { get; set; }
        public string Errors { get; set; }
        
        public string ResultStatus { get; set; }
    }
}
