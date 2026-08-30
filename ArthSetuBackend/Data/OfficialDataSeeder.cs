using ArthSetuBackend.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ArthSetuBackend.Data
{
    public static class OfficialDataSeeder
    {
        public static void Seed(ApplicationDbContext context)
        {
            // Only seed real, canonical official schemes
            var schemesToUpsert = new List<Scheme>
            {
                new Scheme { Id = "pmegp", Name = "Prime Minister's Employment Generation Programme (PMEGP)", Purpose = "Business", SchemeCategory = "Business", LifecycleStatus = "ACTIVE", DataOrigin = "OFFICIAL", VerificationStatus = "VERIFIED", IsActive = true, ApplicationWindowStatus = "OPEN", OwningAuthority = "KVIC", Description = "PMEGP" },
                new Scheme { Id = "pmfme", Name = "PM Formalization of Micro Food Processing Enterprises (PMFME)", Purpose = "Business", SchemeCategory = "Business", LifecycleStatus = "ACTIVE", DataOrigin = "OFFICIAL", VerificationStatus = "VERIFIED", IsActive = true, ApplicationWindowStatus = "OPEN", OwningAuthority = "MoFPI", Description = "PMFME" },
                new Scheme { Id = "pmmy", Name = "Pradhan Mantri MUDRA Yojana (PMMY)", Purpose = "Business", SchemeCategory = "Business", LifecycleStatus = "ACTIVE", DataOrigin = "OFFICIAL", VerificationStatus = "VERIFIED", IsActive = true, ApplicationWindowStatus = "OPEN", OwningAuthority = "DFS", Description = "PMMY" },
                new Scheme { Id = "pm-vishwakarma", Name = "PM Vishwakarma Scheme", Purpose = "Business", SchemeCategory = "Business", LifecycleStatus = "ACTIVE", DataOrigin = "OFFICIAL", VerificationStatus = "VERIFIED", IsActive = true, ApplicationWindowStatus = "OPEN", OwningAuthority = "MSME", Description = "Vishwakarma" },
                new Scheme { Id = "sisfs", Name = "Startup India Seed Fund Scheme", Purpose = "Startup", SchemeCategory = "Startup", LifecycleStatus = "ACTIVE", DataOrigin = "OFFICIAL", VerificationStatus = "VERIFIED", IsActive = true, ApplicationWindowStatus = "CLOSED", OwningAuthority = "DPIIT", Description = "SISFS" },
                new Scheme { Id = "cgss", Name = "Credit Guarantee Scheme for Startups (CGSS)", Purpose = "Startup", SchemeCategory = "Startup", LifecycleStatus = "ACTIVE", DataOrigin = "OFFICIAL", VerificationStatus = "VERIFIED", IsActive = true, ApplicationWindowStatus = "OPEN", OwningAuthority = "DPIIT", Description = "CGSS" },
                new Scheme { Id = "pm-kisan", Name = "Pradhan Mantri Kisan Samman Nidhi (PM-KISAN)", Purpose = "Agriculture", SchemeCategory = "Agriculture", LifecycleStatus = "ACTIVE", DataOrigin = "OFFICIAL", VerificationStatus = "VERIFIED", IsActive = true, ApplicationWindowStatus = "OPEN", OwningAuthority = "Ministry of Agriculture", Description = "PM-KISAN" },
                new Scheme { Id = "aif", Name = "Agriculture Infrastructure Fund (AIF)", Purpose = "Agriculture", SchemeCategory = "Agriculture", LifecycleStatus = "ACTIVE", DataOrigin = "OFFICIAL", VerificationStatus = "VERIFIED", IsActive = true, ApplicationWindowStatus = "OPEN", OwningAuthority = "Ministry of Agriculture", Description = "AIF" },
                new Scheme { Id = "kcc", Name = "Kisan Credit Card (KCC)", Purpose = "Agriculture", SchemeCategory = "Agriculture", LifecycleStatus = "ACTIVE", DataOrigin = "OFFICIAL", VerificationStatus = "VERIFIED", IsActive = true, ApplicationWindowStatus = "OPEN", OwningAuthority = "Ministry of Agriculture", Description = "KCC" },
                new Scheme { Id = "post-matric-sc", Name = "Post Matric Scholarship for SC Students", Purpose = "Education", SchemeCategory = "Education", LifecycleStatus = "ACTIVE", DataOrigin = "OFFICIAL", VerificationStatus = "VERIFIED", IsActive = true, ApplicationWindowStatus = "OPEN", OwningAuthority = "Ministry of Social Justice", Description = "Post Matric SC" },
                new Scheme { Id = "pre-matric-minority", Name = "Pre Matric Scholarships Scheme for Minorities", Purpose = "Education", SchemeCategory = "Education", LifecycleStatus = "ACTIVE", DataOrigin = "OFFICIAL", VerificationStatus = "VERIFIED", IsActive = true, ApplicationWindowStatus = "OPEN", OwningAuthority = "Ministry of Minority Affairs", Description = "Pre Matric Minority" },
                new Scheme { Id = "top-class-sc", Name = "Top Class Education Scheme for SC Students", Purpose = "Education", SchemeCategory = "Education", LifecycleStatus = "ACTIVE", DataOrigin = "OFFICIAL", VerificationStatus = "VERIFIED", IsActive = true, ApplicationWindowStatus = "OPEN", OwningAuthority = "Ministry of Social Justice", Description = "Top Class SC" },
                new Scheme { Id = "pm-vidyalaxmi", Name = "PM-Vidyalaxmi Scheme", Purpose = "Education", SchemeCategory = "Education", LifecycleStatus = "ACTIVE", DataOrigin = "OFFICIAL", VerificationStatus = "VERIFIED", IsActive = true, ApplicationWindowStatus = "OPEN", OwningAuthority = "Ministry of Education", Description = "Vidyalaxmi Loan" }
            };

            foreach (var s in schemesToUpsert)
            {
                var existing = context.Schemes.FirstOrDefault(x => x.Id == s.Id);
                if (existing == null) context.Schemes.Add(s);
                else
                {
                    existing.Name = s.Name;
                    existing.Purpose = s.Purpose;
                    existing.SchemeCategory = s.SchemeCategory;
                    existing.LifecycleStatus = s.LifecycleStatus;
                    existing.ApplicationWindowStatus = s.ApplicationWindowStatus;
                    existing.OwningAuthority = s.OwningAuthority;
                    existing.DataOrigin = s.DataOrigin;
                    existing.VerificationStatus = s.VerificationStatus;
                    existing.IsActive = s.IsActive;
                    existing.Description = s.Description;
                    context.Schemes.Update(existing);
                }
            }
            context.SaveChanges();

            // Seed Components
            var componentsToUpsert = new List<SchemeBenefitComponent>
            {
                new SchemeBenefitComponent { SchemeId = "pmmy", BenefitType = "Shishu", ComponentDescription = "Loans up to Rs 50,000" },
                new SchemeBenefitComponent { SchemeId = "pmmy", BenefitType = "Kishore", ComponentDescription = "Loans from Rs 50,001 to Rs 5 lakh" },
                new SchemeBenefitComponent { SchemeId = "pmmy", BenefitType = "Tarun", ComponentDescription = "Loans from Rs 5,00,001 to Rs 10 lakh" },
                new SchemeBenefitComponent { SchemeId = "pmmy", BenefitType = "Tarun Plus", ComponentDescription = "Loans from Rs 10,00,001 to Rs 20 lakh" },
                new SchemeBenefitComponent { SchemeId = "pmfme", BenefitType = "Individual Micro Enterprises", ComponentDescription = "Support for Individual Micro Enterprises" },
                new SchemeBenefitComponent { SchemeId = "pmfme", BenefitType = "FPOs/SHGs/Cooperatives", ComponentDescription = "Support for FPOs, SHGs, and Producer Cooperatives" }
            };
            
            // Delete existing components for these schemes to ensure idempotency and clean state
            var schemeIdsWithComps = componentsToUpsert.Select(c => c.SchemeId).Distinct().ToList();
            var existingComps = context.SchemeBenefitComponents.Where(c => schemeIdsWithComps.Contains(c.SchemeId)).ToList();
            context.SchemeBenefitComponents.RemoveRange(existingComps);
            context.SaveChanges();

            foreach (var c in componentsToUpsert)
            {
                context.SchemeBenefitComponents.Add(c);
            }
            context.SaveChanges();

            // Seed Rules with proven provenance
            var rulesToUpsert = new List<SchemeEligibilityRule>
            {
                new SchemeEligibilityRule { SchemeId = "pmmy", Field = "purpose", Operator = "InList", Value = "business", Mandatory = true, EligibilitySourceType = "USER_DECLARED", EvaluationOrder = 1, SourceSnapshotId = 1, VerificationStatus = "VERIFIED", LastVerifiedAt = DateTime.UtcNow, GovernmentSourceId = "src-mudra", OfficialSourceUrl = "https://www.mudra.org.in/" },
                new SchemeEligibilityRule { SchemeId = "pmmy", Field = "previousTarunRepaid", Operator = "Equals", Value = "yes", Mandatory = true, ConditionField = "SchemeComponent", ConditionOperator = "Equals", ConditionValue = "Tarun Plus", EligibilitySourceType = "USER_DECLARED", EvaluationOrder = 2, SourceSnapshotId = 1, VerificationStatus = "VERIFIED", LastVerifiedAt = DateTime.UtcNow, GovernmentSourceId = "src-mudra", OfficialSourceUrl = "https://www.mudra.org.in/" },
                new SchemeEligibilityRule { SchemeId = "pmegp", Field = "purpose", Operator = "InList", Value = "business", Mandatory = true, EligibilitySourceType = "USER_DECLARED", EvaluationOrder = 1, SourceSnapshotId = 1, VerificationStatus = "VERIFIED", LastVerifiedAt = DateTime.UtcNow, GovernmentSourceId = "src-pmegp", OfficialSourceUrl = "https://www.kviconline.gov.in/pmegpeportal/" },
                new SchemeEligibilityRule { SchemeId = "pmegp", Field = "education", Operator = "Equals", Value = "8thPass", Mandatory = true, ConditionField = "Expression", ConditionOperator = "EvaluatesTo", ConditionValue = "(ProjectSector=Manufacturing AND ProjectCost>1000000) OR (ProjectSector=Service AND ProjectCost>500000)", EligibilitySourceType = "USER_DECLARED", EvaluationOrder = 2, SourceSnapshotId = 1, VerificationStatus = "VERIFIED", LastVerifiedAt = DateTime.UtcNow, GovernmentSourceId = "src-pmegp", OfficialSourceUrl = "https://www.kviconline.gov.in/pmegpeportal/pmegpweb/docs/pmegp-guidelines.pdf" },
                new SchemeEligibilityRule { SchemeId = "pmfme", Field = "purpose", Operator = "InList", Value = "business", Mandatory = true, EligibilitySourceType = "USER_DECLARED", EvaluationOrder = 1, SourceSnapshotId = 1, VerificationStatus = "VERIFIED", LastVerifiedAt = DateTime.UtcNow, GovernmentSourceId = "src-pmfme", OfficialSourceUrl = "https://pmfme.mofpi.gov.in/" },
                new SchemeEligibilityRule { SchemeId = "pm-kisan", Field = "purpose", Operator = "InList", Value = "agriculture", Mandatory = true, EligibilitySourceType = "USER_DECLARED", EvaluationOrder = 1, SourceSnapshotId = 1, VerificationStatus = "VERIFIED", LastVerifiedAt = DateTime.UtcNow, GovernmentSourceId = "src-pmkisan", OfficialSourceUrl = "https://pmkisan.gov.in/" },
                new SchemeEligibilityRule { SchemeId = "pm-kisan", Field = "institutionalLandHolder", Operator = "Equals", Value = "no", Mandatory = true, EligibilitySourceType = "USER_DECLARED", EvaluationOrder = 2, SourceSnapshotId = 1, VerificationStatus = "VERIFIED", LastVerifiedAt = DateTime.UtcNow, GovernmentSourceId = "src-pmkisan", OfficialSourceUrl = "https://pmkisan.gov.in/Documents/Exclusion_Categories.pdf" },
                new SchemeEligibilityRule { SchemeId = "pm-kisan", Field = "constitutionalPostHolder", Operator = "Equals", Value = "no", Mandatory = true, EligibilitySourceType = "USER_DECLARED", EvaluationOrder = 3, SourceSnapshotId = 1, VerificationStatus = "VERIFIED", LastVerifiedAt = DateTime.UtcNow, GovernmentSourceId = "src-pmkisan", OfficialSourceUrl = "https://pmkisan.gov.in/Documents/Exclusion_Categories.pdf" },
                new SchemeEligibilityRule { SchemeId = "sisfs", Field = "purpose", Operator = "InList", Value = "startup", Mandatory = true, EligibilitySourceType = "USER_DECLARED", EvaluationOrder = 1, SourceSnapshotId = 1, VerificationStatus = "VERIFIED", LastVerifiedAt = DateTime.UtcNow, GovernmentSourceId = "src-sisfs", OfficialSourceUrl = "https://seedfund.startupindia.gov.in/" },
                new SchemeEligibilityRule { SchemeId = "cgss", Field = "purpose", Operator = "InList", Value = "startup", Mandatory = true, EligibilitySourceType = "USER_DECLARED", EvaluationOrder = 1, SourceSnapshotId = 1, VerificationStatus = "VERIFIED", LastVerifiedAt = DateTime.UtcNow, GovernmentSourceId = "src-cgss", OfficialSourceUrl = "https://www.startupindia.gov.in/" },
                new SchemeEligibilityRule { SchemeId = "cgss", Field = "memberInstitutionEligibility", Operator = "Equals", Value = "yes", Mandatory = true, EligibilitySourceType = "EXTERNAL_AUTHORITY_ASSESSMENT", EvaluationOrder = 2, SourceSnapshotId = 1, VerificationStatus = "VERIFIED", LastVerifiedAt = DateTime.UtcNow, GovernmentSourceId = "src-cgss", OfficialSourceUrl = "https://www.startupindia.gov.in/" }
            };

            // Delete existing rules for canonical schemes to avoid dupes
            var schemeIdsWithRules = rulesToUpsert.Select(r => r.SchemeId).Distinct().ToList();
            var existingRules = context.SchemeEligibilityRules.Where(r => schemeIdsWithRules.Contains(r.SchemeId)).ToList();
            context.SchemeEligibilityRules.RemoveRange(existingRules);
            context.SaveChanges();

            foreach (var r in rulesToUpsert)
            {
                context.SchemeEligibilityRules.Add(r);
            }
            context.SaveChanges();
        }
    }
}
