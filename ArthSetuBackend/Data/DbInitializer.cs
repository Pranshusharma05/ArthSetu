using ArthSetuBackend.Models;
using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ArthSetuBackend.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            context.Database.EnsureCreated();
            
            // Check if NSFDC is verified already
            var nsfdcSource = context.GovernmentSources.FirstOrDefault(s => s.Id == "src-nsfdc");
            if (nsfdcSource != null && nsfdcSource.ConnectionStatus == "Verified" && context.Schemes.Any(s => s.SourceId == "src-nsfdc"))
            {
                return; // DB already has NSFDC data seeded
            }

            // Update or add sources
            var sourcesToRegister = new[] {
                new GovernmentSource { Id = "src-nsfdc", SourceName = "NSFDC", ConnectionStatus = "Verified", IsActive = true, IngestionMethod = "Manual / Document-Based Verification Required" },
                new GovernmentSource { Id = "src-nbcfdc", SourceName = "NBCFDC", ConnectionStatus = "REGISTERED", IsActive = true, IngestionMethod = "Automated" },
                new GovernmentSource { Id = "src-nstfdc", SourceName = "NSTFDC", ConnectionStatus = "REGISTERED", IsActive = true, IngestionMethod = "Automated" },
                new GovernmentSource { Id = "src-ndfdc", SourceName = "NDFDC", ConnectionStatus = "REGISTERED", IsActive = true, IngestionMethod = "Automated" },
                new GovernmentSource { Id = "src-jansamarth", SourceName = "JanSamarth Portal", ConnectionStatus = "REGISTERED", IsActive = true, IngestionMethod = "Automated" },
                new GovernmentSource { Id = "src-nsp", SourceName = "National Scholarship Portal", ConnectionStatus = "REGISTERED", IsActive = true, IngestionMethod = "Automated" },
                new GovernmentSource { Id = "src-vidyalaxmi", SourceName = "Vidyalakshmi Portal", ConnectionStatus = "REGISTERED", IsActive = true, IngestionMethod = "Automated" },
                new GovernmentSource { Id = "src-mudra", SourceName = "Pradhan Mantri MUDRA Yojana", ConnectionStatus = "REGISTERED", IsActive = true, IngestionMethod = "Automated" },
                new GovernmentSource { Id = "src-pmegp", SourceName = "PMEGP (KVIC)", ConnectionStatus = "REGISTERED", IsActive = true, IngestionMethod = "Automated" },
                new GovernmentSource { Id = "src-pmvishwakarma", SourceName = "PM Vishwakarma", ConnectionStatus = "REGISTERED", IsActive = true, IngestionMethod = "Automated" },
                new GovernmentSource { Id = "src-pmfme", SourceName = "PM Formalization of Micro Food Processing Enterprises", ConnectionStatus = "REGISTERED", IsActive = true, IngestionMethod = "Automated" }
            };

            foreach (var src in sourcesToRegister)
            {
                var existingSource = context.GovernmentSources.FirstOrDefault(s => s.Id == src.Id);
                if (existingSource == null)
                {
                    context.GovernmentSources.Add(src);
                }
                else
                {
                    existingSource.ConnectionStatus = src.ConnectionStatus;
                    existingSource.IngestionMethod = src.IngestionMethod;
                    context.GovernmentSources.Update(existingSource);
                }
            }
            
            context.SaveChanges();
if (!context.ChannelPartners.Any()) {
    var banks = new[] {
        "Bank of Baroda", "Bank of India", "Bank of Maharashtra", "Canara Bank",
        "Central Bank of India", "Indian Bank", "Indian Overseas Bank",
        "Punjab & Sind Bank", "Punjab National Bank", "State Bank of India", "UCO Bank"
    };
    foreach (var bank in banks) {
        var partner = new ChannelPartner {
            Name = bank,
            PartnerType = "HEAD_OFFICE",
            RegisteredAddress = "Corporate Office",
            State = "National",
            VerificationStatus = "Verified",
            SourceSnapshot = "NSFDC_SCA_LIST_2026",
            LastVerifiedAt = DateTime.UtcNow
        };
        context.ChannelPartners.Add(partner);
        context.SaveChanges();
        var nsfdcSchemes = context.Schemes.Where(s => s.Id.StartsWith("nsfdc")).ToList();
        foreach (var s in nsfdcSchemes) {
            context.PartnerSchemes.Add(new PartnerScheme { PartnerId = partner.Id, SchemeId = s.Id });
        }
        context.SaveChanges();
    }
}
            
            // Note: NSFDC Official Scheme Seeding has been removed.
            // Official data is now ingested via the NSFDC Connector Service (POST /api/sync/nsfdc).
            // Channel Partners are also ingested via connector or separate service.
        }
    }
}


