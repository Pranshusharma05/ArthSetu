using System;
using System.ComponentModel.DataAnnotations;

namespace ArthSetu.Models
{
    public class User { [Key] public Guid Id { get; set; } }
    public class Role { [Key] public Guid Id { get; set; } }
    public class UserRole { [Key] public Guid Id { get; set; } }
    public class BeneficiaryProfile { [Key] public Guid Id { get; set; } }
    
    public class SchemeFAQ { [Key] public Guid Id { get; set; } }
    
    public class ChannelPartner { [Key] public Guid Id { get; set; } }
    public class PartnerLocation { [Key] public Guid Id { get; set; } }
    public class PartnerScheme { [Key] public Guid Id { get; set; } }
    public class PartnerCapacity { [Key] public Guid Id { get; set; } }
    public class PartnerPerformance { [Key] public Guid Id { get; set; } }
    
    public class Application { [Key] public Guid Id { get; set; } }
    public class ApplicationDocument { [Key] public Guid Id { get; set; } }
    public class ApplicationStatusHistory { [Key] public Guid Id { get; set; } }
    
    public class LoanCalculation { [Key] public Guid Id { get; set; } }
    public class Notification { [Key] public Guid Id { get; set; } }
    public class ChatSession { [Key] public Guid Id { get; set; } }
    public class ChatMessage { [Key] public Guid Id { get; set; } }
    public class AuditLog { [Key] public Guid Id { get; set; } }
}
