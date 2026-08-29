using System;

namespace ArthSetuBackend.Models
{
    public class LocationMaster
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // State, UT, District
        public int? ParentId { get; set; }
    }

    public class SourceSnapshot
    {
        public int Id { get; set; }
        public string SourceId { get; set; } = string.Empty;
        public DateTime SnapshotDate { get; set; }
        public string? Payload { get; set; }
        public string? SourceUrl { get; set; }
        public string? FinalUrl { get; set; }
        public string? Title { get; set; }
        public string? ContentType { get; set; }
        public DateTime? PublishedDate { get; set; }
        public string? ETag { get; set; }
        public string? LastModified { get; set; }
        public string? ContentHash { get; set; }
        public string? ConnectorVersion { get; set; }
        public string? VerificationStatus { get; set; }
    }

    public class SchemeVersion
    {
        public int Id { get; set; }
        public string SchemeId { get; set; } = string.Empty;
        public string VersionHash { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class EligibilityRuleVersion
    {
        public int Id { get; set; }
        public int RuleId { get; set; }
        public string VersionHash { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        
        public string? SchemeId { get; set; }
        public int? SchemeBenefitComponentId { get; set; }
        public string? NormalizedValue { get; set; }
        public int? SourceSnapshotId { get; set; }
        public DateTime? EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public string? CurrentStatus { get; set; }
        public string? VerificationStatus { get; set; }
        public int? SupersededBy { get; set; }
        public string? SourceEvidence { get; set; }
    }

    public class FieldProvenance
    {
        public int Id { get; set; }
        public string EntityId { get; set; } = string.Empty;
        public string FieldName { get; set; } = string.Empty;
        public string SourceId { get; set; } = string.Empty;
        public DateTime VerifiedAt { get; set; }
        
        public string? RawValue { get; set; }
        public string? SourceUrl { get; set; }
        public string? SourceLocation { get; set; }
        public DateTime? ExtractedAt { get; set; }
        public string? VerificationStatus { get; set; }
    }

    public class SourceConflict
    {
        public int Id { get; set; }
        public string SchemeId { get; set; } = string.Empty;
        public int? SchemeBenefitComponentId { get; set; }
        public string Field { get; set; } = string.Empty;
        public string ExistingValue { get; set; } = string.Empty;
        public string CandidateValue { get; set; } = string.Empty;
        public string ExistingSource { get; set; } = string.Empty;
        public string CandidateSource { get; set; } = string.Empty;
        public DateTime PublicationDate { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class SourceSyncLog
    {
        public int Id { get; set; }
        public string SourceId { get; set; } = string.Empty;
        public DateTime SyncDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? ErrorMessage { get; set; }
        
        public string? Connector { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string? FetchStatus { get; set; }
        public int? HttpStatus { get; set; }
        public int? SnapshotId { get; set; }
        public bool? ContentChanged { get; set; }
        
        public int? RecordsDiscovered { get; set; }
        public int? RecordsParsed { get; set; }
        public int? RecordsImported { get; set; }
        public int? RecordsUpdated { get; set; }
        public int? RecordsUnchanged { get; set; }
        public int? RecordsSkipped { get; set; }
        public int? RecordsNeedingReview { get; set; }
        public int? ConflictsCreated { get; set; }
    }
}
