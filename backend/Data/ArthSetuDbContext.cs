using Microsoft.EntityFrameworkCore;
using ArthSetu.Models;

namespace ArthSetu.Data
{
    public class ArthSetuDbContext : DbContext
    {
        public ArthSetuDbContext(DbContextOptions<ArthSetuDbContext> options) : base(options) { }

        // Original PRD Tables
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<BeneficiaryProfile> BeneficiaryProfiles { get; set; }
        
        public DbSet<SchemeFAQ> SchemeFAQs { get; set; }
        
        public DbSet<ChannelPartner> ChannelPartners { get; set; }
        public DbSet<PartnerLocation> PartnerLocations { get; set; }
        public DbSet<PartnerScheme> PartnerSchemes { get; set; }
        public DbSet<PartnerCapacity> PartnerCapacity { get; set; }
        public DbSet<PartnerPerformance> PartnerPerformance { get; set; }
        
        public DbSet<Application> Applications { get; set; }
        public DbSet<ApplicationDocument> ApplicationDocuments { get; set; }
        public DbSet<ApplicationStatusHistory> ApplicationStatusHistory { get; set; }
        
        public DbSet<LoanCalculation> LoanCalculations { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<ChatSession> ChatSessions { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        
        // New Architecture Tables for Government Data
        public DbSet<GovernmentSource> GovernmentSources { get; set; }
        public DbSet<Scheme> Schemes { get; set; }
        public DbSet<SchemeComponent> SchemeComponents { get; set; }
        public DbSet<SchemeEligibilityRule> SchemeEligibilityRules { get; set; }
        public DbSet<SchemeDocument> SchemeDocuments { get; set; }
        public DbSet<SchemeVersion> SchemeVersions { get; set; }
        public DbSet<FieldProvenance> FieldProvenances { get; set; }
        public DbSet<SourceSnapshot> SourceSnapshots { get; set; }
        public DbSet<SourceConflict> SourceConflicts { get; set; }
        public DbSet<SourceSyncLog> SourceSyncLogs { get; set; }
        public DbSet<LocationMaster> LocationMaster { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
