using System;
using System.Collections.Generic;

namespace ArthSetuBackend.Models
{
    public class ChannelPartner
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string PartnerType { get; set; } = string.Empty;
        public string? ContactInfo { get; set; }
        public string? RegisteredAddress { get; set; }
                public string State { get; set; } = string.Empty;
        public string Pincode { get; set; } = string.Empty;
        public string SourceSnapshot { get; set; } = string.Empty;
        public string VerificationStatus { get; set; } = "NEEDS_REVIEW";
        public DateTime? LastVerifiedAt { get; set; }

        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<PartnerScheme> PartnerSchemes { get; set; } = new List<PartnerScheme>();
        public ICollection<PartnerOperationalStatus> OperationalStatuses { get; set; } = new List<PartnerOperationalStatus>();
    }

    public class PartnerScheme
    {
        public int Id { get; set; }
        public int PartnerId { get; set; }
        public string SchemeId { get; set; } = string.Empty;

        public ChannelPartner? Partner { get; set; }
        public Scheme? Scheme { get; set; }
    }

    public class PartnerOperationalStatus
    {
        public int Id { get; set; }
        public int PartnerId { get; set; }
        public string State { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;

        public ChannelPartner? Partner { get; set; }
    }
}

