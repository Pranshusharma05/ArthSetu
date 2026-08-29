using ArthSetuBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace ArthSetuBackend.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<GovernmentSource> GovernmentSources { get; set; }
        public DbSet<Scheme> Schemes { get; set; }
        public DbSet<SchemeEligibilityRule> SchemeEligibilityRules { get; set; }
        public DbSet<LocationMaster> LocationMaster { get; set; }
        public DbSet<SourceSnapshot> SourceSnapshots { get; set; }
        public DbSet<SchemeVersion> SchemeVersions { get; set; }
        public DbSet<EligibilityRuleVersion> EligibilityRuleVersions { get; set; }
        public DbSet<FieldProvenance> FieldProvenance { get; set; }
        public DbSet<SourceConflict> SourceConflicts { get; set; }
        public DbSet<SourceSyncLog> SourceSyncLogs { get; set; }
        public DbSet<ChannelPartner> ChannelPartners { get; set; }
        public DbSet<PartnerScheme> PartnerSchemes { get; set; }
        public DbSet<PartnerOperationalStatus> PartnerOperationalStatuses { get; set; }
        
        // Phase 8B Additions
        public DbSet<SchemeApplicationWindow> SchemeApplicationWindows { get; set; }
        public DbSet<SchemeBenefitComponent> SchemeBenefitComponents { get; set; }
        public DbSet<DiscoveryCandidate> DiscoveryCandidates { get; set; }
        public DbSet<SchemeSourceReference> SchemeSourceReferences { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Explicit table mappings
            modelBuilder.Entity<GovernmentSource>().ToTable("GovernmentSources");
            modelBuilder.Entity<Scheme>().ToTable("Schemes");
            modelBuilder.Entity<SchemeEligibilityRule>().ToTable("SchemeEligibilityRules");

            modelBuilder.Entity<ChannelPartner>().ToTable("ChannelPartners");
            modelBuilder.Entity<PartnerScheme>().ToTable("PartnerSchemes");
            modelBuilder.Entity<PartnerOperationalStatus>().ToTable("PartnerOperationalStatuses");

            // Phase 8B Additions
            modelBuilder.Entity<SchemeApplicationWindow>().ToTable("SchemeApplicationWindows");
            modelBuilder.Entity<SchemeBenefitComponent>().ToTable("SchemeBenefitComponents");
            modelBuilder.Entity<DiscoveryCandidate>().ToTable("DiscoveryCandidates");
            modelBuilder.Entity<SchemeSourceReference>().ToTable("SchemeSourceReferences");
        }
    }
}


