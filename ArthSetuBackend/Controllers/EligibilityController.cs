using Microsoft.AspNetCore.Mvc;
using ArthSetuBackend.Data;
using ArthSetuBackend.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace ArthSetuBackend.Controllers
{
    [ApiController]
    [Route("api/schemes")]
    public class SchemeMatchingController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SchemeMatchingController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("stats")]
        public IActionResult GetStats()
        {
            return Ok(new {
                total = _context.Schemes.Count(),
                verified = _context.Schemes.Count(s => s.VerificationStatus == "Verified" || s.VerificationStatus == "VERIFIED"),
                central = _context.Schemes.Count(s => (s.VerificationStatus == "Verified" || s.VerificationStatus == "VERIFIED") && s.Scope == "Central"),
                state = _context.Schemes.Count(s => (s.VerificationStatus == "Verified" || s.VerificationStatus == "VERIFIED") && s.Scope != "Central"),
                needsReview = _context.Schemes.Count(s => s.VerificationStatus == "NEEDS_REVIEW" || s.VerificationStatus == "Needs Review"),
                demo = _context.Schemes.Count(s => s.DataOrigin == "DEMO" || s.DataOrigin == "MOCK"),
                appUrls = _context.Schemes.Count(s => !string.IsNullOrEmpty(s.OfficialApplicationUrl)),
                sources = _context.GovernmentSources.Count()
            });
        }

        [HttpPost("match")]
        public IActionResult EvaluateEligibility([FromBody] UserProfile profile)
        {
            // Map frontend purpose labels to database purpose values
            var purposeMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "Business", "Income-Generating Activities" },
                { "Business & Entrepreneurship", "Income-Generating Activities" },
                { "Entrepreneurship", "Income-Generating Activities" },
                { "Income Generating", "Income-Generating Activities" },
            };
            var targetPurpose = purposeMap.TryGetValue(profile.Purpose ?? "", out var mappedPurpose)
                ? mappedPurpose
                : profile.Purpose;
            
            var excludedOrigins = new[] { "DEMO", "MOCK", "DEVELOPMENT_SEED", "LEGACY_REFERENCE", "DISCOVERY_CATEGORY", "NEEDS_REVIEW", "SUPERSEDED", "GENERIC_PLACEHOLDER" };

            // Build purpose category aliases for proper matching
            var matchCategoryAliases = new List<string>();
            if (!string.IsNullOrEmpty(profile.Purpose))
            {
                var purposeLower = profile.Purpose.ToLower();
                if (purposeLower.Contains("business") || purposeLower.Contains("entrepreneur") || purposeLower.Contains("income"))
                    matchCategoryAliases.AddRange(new[] { "Business", "Income-Generating Activities", "Micro Finance", "Term Loan" });
                else if (purposeLower.Contains("education") || purposeLower.Contains("scholarship"))
                    matchCategoryAliases.AddRange(new[] { "Education", "Scholarship" });
                else if (purposeLower.Contains("agriculture") || purposeLower.Contains("farm"))
                    matchCategoryAliases.AddRange(new[] { "Agriculture" });
                else if (purposeLower.Contains("startup"))
                    matchCategoryAliases.AddRange(new[] { "Startup", "Business" });
                else
                    matchCategoryAliases.Add(profile.Purpose);
            }
            
            var activeSchemes = _context.Schemes
                                    .Include(s => s.Source)
                .Include(s => s.ApplicationWindows)
                .Include(s => s.Source)
                .Include(s => s.DiscoveryCategories)
                .Where(s => s.IsActive && s.DataOrigin == "OFFICIAL" && !excludedOrigins.Contains(s.DataOrigin)
                    && (s.VerificationStatus == "Verified" || s.VerificationStatus == "VERIFIED")
                    && (s.RulesetStatus == "RULESET_COMPLETE")
                    && s.DiscoveryCategories.Any(c => matchCategoryAliases.Contains(c.CategoryName) || c.CategoryName == targetPurpose || c.CategoryName == profile.Purpose))
                .ToList();
                                    
            var allRules = _context.SchemeEligibilityRules.ToList();

            var recommended = new List<object>();
            var notEligible = new List<object>();
            var moreInfoNeeded = new List<object>();

            foreach (var scheme in activeSchemes)
            {
                var rules = allRules.Where(r => r.SchemeId == scheme.Id).ToList();
                var evalRes = EvaluateRules(rules, profile);
                
                var schemeData = new
                {
                    id = scheme.Id,
                    name = scheme.Name,
                    description = scheme.Description,
                    benefitType = scheme.BenefitType,
                    schemeCategory = scheme.SchemeCategory,
                    purpose = scheme.Purpose,
                    applicationRoute = GetApplicationRoute(scheme, evalRes),
                    verificationStatus = scheme.VerificationStatus,
                    lastVerified = scheme.LastVerified?.ToString("yyyy-MM-dd") ?? "",
                    officialSource = scheme.Source?.SourceName ?? "Government Source",
                    source = scheme.Source?.OfficialDomain ?? "india.gov.in",
                    ruleComparisons = evalRes.Passed.Concat(evalRes.Failed).Concat(evalRes.Missing.Select(m => new { ruleName = m, status = "Missing" })).ToList(),
                    missingRules = evalRes.Missing
                };

                if (evalRes.EligibilityState == "Not Eligible")
                    notEligible.Add(schemeData);
                else if (evalRes.EligibilityState == "More Information Needed")
                    moreInfoNeeded.Add(schemeData);
                else
                    recommended.Add(schemeData);
            }

            return Ok(new { recommended, otherEligible = new List<object>(), moreInfoNeeded, notEligible });
        }

        [HttpPost("dynamic-questions")]
        public IActionResult GetDynamicQuestions([FromBody] UserProfile profile)
        {
            var purposeMap2 = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "Business", "Income-Generating Activities" },
                { "Business & Entrepreneurship", "Income-Generating Activities" },
                { "Entrepreneurship", "Income-Generating Activities" },
                { "Income Generating", "Income-Generating Activities" },
            };
            var targetPurpose = purposeMap2.TryGetValue(profile.Purpose ?? "", out var mappedPurpose2)
                ? mappedPurpose2
                : profile.Purpose;

            var categoryAliases = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            if (profile.Purpose != null)
            {
                var purposeLower = profile.Purpose.ToLower();
                if (purposeLower.Contains("business") || purposeLower.Contains("entrepreneur") || purposeLower.Contains("income"))
                {
                    categoryAliases.Add("Business");
                    categoryAliases.Add("Income-Generating Activities");
                    categoryAliases.Add("Micro Finance");
                    categoryAliases.Add("Term Loan");
                }
                else if (purposeLower.Contains("education") || purposeLower.Contains("scholarship"))
                {
                    categoryAliases.Add("Education");
                    categoryAliases.Add("Scholarship");
                }
                else if (purposeLower.Contains("agriculture") || purposeLower.Contains("farm"))
                {
                    categoryAliases.Add("Agriculture");
                }
                else if (purposeLower.Contains("startup"))
                {
                    categoryAliases.Add("Startup");
                    categoryAliases.Add("Business");
                }
                else
                {
                    categoryAliases.Add(profile.Purpose);
                }
            }
            var categoryAliasList = categoryAliases.ToList(); // EF requires List for Contains
            
            var activeSchemes = _context.Schemes
                                    .Include(s => s.DiscoveryCategories)
                                    .Where(s => s.IsActive && s.DataOrigin == "OFFICIAL" 
                                        && (s.VerificationStatus == "Verified" || s.VerificationStatus == "VERIFIED") 
                                        && (s.RulesetStatus == "RULESET_COMPLETE")
                                        && (string.IsNullOrEmpty(profile.Purpose) || s.Purpose == targetPurpose || s.Purpose == profile.Purpose || s.DiscoveryCategories.Any(c => categoryAliasList.Contains(c.CategoryName) || c.CategoryName == profile.Purpose)))
                                    .ToList();
            
            var allRules = _context.SchemeEligibilityRules.ToList();
            
            // Re-evaluate schemes after each answer to remove failing candidates
            var survivingSchemes = new List<Scheme>();
            var allMissing = new HashSet<string>();
            var missingCounts = new Dictionary<string, int>();

            foreach (var scheme in activeSchemes)
            {
                var rules = allRules.Where(r => r.SchemeId == scheme.Id).ToList();
                var evalRes = EvaluateRules(rules, profile);
                
                // Keep only schemes that have not failed
                if (evalRes.EligibilityState != "Not Eligible")
                {
                    survivingSchemes.Add(scheme);
                    foreach(var m in evalRes.Missing) 
                    {
                        allMissing.Add(m);
                        if (!missingCounts.ContainsKey(m)) missingCounts[m] = 0;
                        missingCounts[m]++;
                    }
                }
            }

            var questions = new List<object>();

            // Determine the next best question by sorting missing fields by frequency
            var sortedMissing = missingCounts.OrderByDescending(kv => kv.Value).Select(kv => kv.Key).ToList();

            // To not dump all questions, we can limit or just provide them in order of importance.
            // Let's map standard ones that actually exist in the surviving scheme rules.
            
            int addedQuestions = 0;
            bool projectCostAsked = false;
            foreach (var m in sortedMissing)
            {
                if (addedQuestions >= 5) break;

                var triggeredBySchemes = survivingSchemes.Where(s => allRules.Any(r => r.SchemeId == s.Id && r.Field.Equals(m, StringComparison.OrdinalIgnoreCase))).Select(s => s.Name).Distinct().ToList();
                var ruleIds = allRules.Where(r => r.Field.Equals(m, StringComparison.OrdinalIgnoreCase) && survivingSchemes.Any(s => s.Id == r.SchemeId)).Select(r => r.Id.ToString()).ToList();
                var officialSources = allRules.Where(r => r.Field.Equals(m, StringComparison.OrdinalIgnoreCase) && survivingSchemes.Any(s => s.Id == r.SchemeId)).Select(r => r.SourceReference ?? "Official Guidelines").Distinct().ToList();

                if ((m.Equals("ProjectCostMax", StringComparison.OrdinalIgnoreCase) || m.Equals("ProjectCostMin", StringComparison.OrdinalIgnoreCase) || m.Equals("projectCost", StringComparison.OrdinalIgnoreCase)) && !projectCostAsked)
                {
                    questions.Add(new { id = "projectCost", canonicalField = m, type = "currency", label = "Estimated Project Cost (₹)", helpText = "Total amount needed for your business or project", triggeredBy = triggeredBySchemes, ruleIds = ruleIds, sources = officialSources });
                    addedQuestions++;
                    projectCostAsked = true;
                }
                else if (m.Equals("BusinessActivity", StringComparison.OrdinalIgnoreCase))
                {
                    questions.Add(new { id = "BusinessActivity", canonicalField = m, type = "taxonomy", label = "Select Business / Economic Activity", triggeredBy = triggeredBySchemes, ruleIds = ruleIds, sources = officialSources });
                    addedQuestions++;
                }
                else if (m.Equals("Gender", StringComparison.OrdinalIgnoreCase) && string.IsNullOrEmpty(profile.Gender))
                {
                    questions.Add(new { id = "Gender", canonicalField = m, type = "single_choice", label = "Gender", options = new[] { new {label="Female", value="Female"}, new {label="Male", value="Male"}, new {label="Other", value="Other"} }, triggeredBy = triggeredBySchemes, ruleIds = ruleIds, sources = officialSources });
                    addedQuestions++;
                }
                else if (m.Equals("Age", StringComparison.OrdinalIgnoreCase) && (!profile.Age.HasValue || profile.Age == 0))
                {
                    questions.Add(new { id = "Age", canonicalField = m, type = "numeric", label = "Age (Years)", triggeredBy = triggeredBySchemes, ruleIds = ruleIds, sources = officialSources });
                    addedQuestions++;
                }
                else if (m.Equals("IsPwD", StringComparison.OrdinalIgnoreCase) && !profile.IsPwD.HasValue)
                {
                    questions.Add(new { id = "IsPwD", canonicalField = m, type = "single_choice", label = "Are you a Person with Disability (PwD)?", options = new[] { new {label="Yes", value="true"}, new {label="No", value="false"} }, triggeredBy = triggeredBySchemes, ruleIds = ruleIds, sources = officialSources });
                    addedQuestions++;
                }
                else if (m.Equals("DisabilityPercentageMin", StringComparison.OrdinalIgnoreCase) && !profile.DisabilityPercentage.HasValue && profile.IsPwD != false)
                {
                    questions.Add(new { id = "DisabilityPercentage", canonicalField = m, type = "numeric", label = "Disability Percentage (%)", triggeredBy = triggeredBySchemes, ruleIds = ruleIds, sources = officialSources });
                    addedQuestions++;
                }
                else if (m.Equals("ApplicantType", StringComparison.OrdinalIgnoreCase) && string.IsNullOrEmpty(profile.ApplicantType))
                {
                    questions.Add(new { id = "ApplicantType", canonicalField = m, type = "single_choice", label = "Applicant Type", options = new[] { new {label="Individual", value="Individual"}, new {label="Self-Help Group (SHG)", value="SHG"}, new {label="FPO / Cooperative", value="FPO"}, new {label="Central/State agency", value="Central/State agency"} }, triggeredBy = triggeredBySchemes, ruleIds = ruleIds, sources = officialSources });
                    addedQuestions++;
                }
                else if (m.Equals("FarmerType", StringComparison.OrdinalIgnoreCase))
                {
                    questions.Add(new { id = "FarmerType", canonicalField = m, type = "single_choice", label = "Farmer Type", options = new[] { new {label="Individual", value="Individual"}, new {label="Joint Borrower", value="Joint Borrower"}, new {label="Tenant Farmer", value="Tenant Farmer"}, new {label="Share Cropper", value="Share Cropper"}, new {label="Land-holding farmer family", value="Land-holding farmer family"}, new {label="Owner Cultivator", value="Owner Cultivator"}, new {label="Oral Lessee", value="Oral Lessee"}, new {label="Self Help Group", value="Self Help Group"}, new {label="Joint Liability Group", value="Joint Liability Group"} }, triggeredBy = triggeredBySchemes, ruleIds = ruleIds, sources = officialSources });
                    addedQuestions++;
                }
                else if (m.Equals("educationLevel", StringComparison.OrdinalIgnoreCase) || m.Equals("education", StringComparison.OrdinalIgnoreCase))
                {
                    questions.Add(new { id = "educationLevel", canonicalField = m, type = "single_choice", label = "Education Level", options = new[] { new {label="8th Pass", value="8"}, new {label="10th Pass", value="10"}, new {label="12th Pass", value="12"}, new {label="Undergraduate", value="15"}, new {label="Postgraduate", value="17"} }, triggeredBy = triggeredBySchemes, ruleIds = ruleIds, sources = officialSources });
                    addedQuestions++;
                }
                else if (m.Equals("PriorGovernmentSubsidy", StringComparison.OrdinalIgnoreCase))
                {
                    questions.Add(new { id = "PriorGovernmentSubsidy", canonicalField = m, type = "yes_no", label = "Have you received any prior Government Subsidy?", triggeredBy = triggeredBySchemes, ruleIds = ruleIds, sources = officialSources });
                    addedQuestions++;
                }
                else if (m.Equals("FamilyMemberCountInScheme", StringComparison.OrdinalIgnoreCase))
                {
                    questions.Add(new { id = "FamilyMemberCountInScheme", canonicalField = m, type = "numeric", label = "How many family members are already enrolled in this scheme?", triggeredBy = triggeredBySchemes, ruleIds = ruleIds, sources = officialSources });
                    addedQuestions++;
                }
                else if (m.Equals("RequestedLoanAmount", StringComparison.OrdinalIgnoreCase))
                {
                    questions.Add(new { id = "RequestedLoanAmount", canonicalField = m, type = "currency", label = "Requested Loan Amount (₹)", triggeredBy = triggeredBySchemes, ruleIds = ruleIds, sources = officialSources });
                    addedQuestions++;
                }
                else if (m.Equals("ExistingEnterprise", StringComparison.OrdinalIgnoreCase) || m.Equals("ExistingMicroFoodProcessingUnit", StringComparison.OrdinalIgnoreCase) || m.Equals("OwnershipRight", StringComparison.OrdinalIgnoreCase) || m.Equals("EnterpriseStatus", StringComparison.OrdinalIgnoreCase))
                {
                    questions.Add(new { id = m, canonicalField = m, type = "yes_no", label = $"Confirm: {m}", triggeredBy = triggeredBySchemes, ruleIds = ruleIds, sources = officialSources });
                    addedQuestions++;
                }
                else if (m.Equals("previousTarunRepaid", StringComparison.OrdinalIgnoreCase))
                {
                    questions.Add(new { id = "previousTarunRepaid", canonicalField = m, type = "yes_no", label = "Have you successfully repaid a previous PMMY Tarun loan?", triggeredBy = triggeredBySchemes, ruleIds = ruleIds, sources = officialSources });
                    addedQuestions++;
                }
                else if (m.Equals("AdmissionStatus", StringComparison.OrdinalIgnoreCase))
                {
                    questions.Add(new { id = "AdmissionStatus", canonicalField = m, type = "single_choice", label = "Current Admission Status", options = new[] { new {label="Admitted to Top 860 QSI/NIRF Institution", value="Admitted to Top 860 QSI/NIRF Institution"}, new {label="Admitted to Notified Institution", value="Admitted to Notified Institution"}, new {label="Admitted", value="Admitted"}, new {label="Other", value="Other"} }, triggeredBy = triggeredBySchemes, ruleIds = ruleIds, sources = officialSources });
                    addedQuestions++;
                }
                else if (m.Equals("Category", StringComparison.OrdinalIgnoreCase) || m.Equals("SocialCategory", StringComparison.OrdinalIgnoreCase) && string.IsNullOrEmpty(profile.Category))
                {
                    questions.Add(new { id = "SocialCategory", canonicalField = m, type = "single_choice", label = "Social Category", options = new[] { new {label="General", value="General"}, new {label="SC", value="SC"}, new {label="ST", value="ST"}, new {label="OBC", value="OBC"}, new {label="Muslim", value="Muslim"}, new {label="Christian", value="Christian"}, new {label="Sikh", value="Sikh"} }, triggeredBy = triggeredBySchemes, ruleIds = ruleIds, sources = officialSources });
                    addedQuestions++;
                }
                else if (m.Equals("AnnualFamilyIncome", StringComparison.OrdinalIgnoreCase) || m.Equals("income", StringComparison.OrdinalIgnoreCase))
                {
                    questions.Add(new { id = "income", canonicalField = m, type = "currency", label = "Annual Family Income (₹)", triggeredBy = triggeredBySchemes, ruleIds = ruleIds, sources = officialSources });
                    addedQuestions++;
                }
                else if (!m.Equals("MemberInstitutionEligibilityCertification", StringComparison.OrdinalIgnoreCase) && !m.Equals("InstitutionEligibility", StringComparison.OrdinalIgnoreCase) && !m.Equals("InstitutionType", StringComparison.OrdinalIgnoreCase))
                {
                    questions.Add(new { id = m, canonicalField = m, type = "text", label = $"Please provide: {m}", triggeredBy = triggeredBySchemes, ruleIds = ruleIds, sources = officialSources });
                    addedQuestions++;
                }
            }

            return Ok(questions);
        }

        [HttpGet("category-coverage")]
        public IActionResult GetCategoryCoverage()
        {
            var categories = new[] {
                "Agriculture", "Banking", "Business", "Education", "Health", "Housing", 
                "Public Safety", "Science", "Skills", "Social Welfare", "Sports", 
                "Transport", "Tourism", "Utility", "Women"
            };

            var coverage = new Dictionary<string, int>();

            foreach(var cat in categories)
            {
                var count = _context.Schemes.Count(s => 
                    s.IsActive && 
                    s.DataOrigin == "OFFICIAL" && 
                    (s.VerificationStatus == "Verified" || s.VerificationStatus == "VERIFIED") &&
                    s.RulesetStatus == "RULESET_COMPLETE" &&
                    !new[] { "DEMO", "MOCK", "DEVELOPMENT_SEED", "LEGACY_REFERENCE", "DISCOVERY_CATEGORY", "NEEDS_REVIEW", "SUPERSEDED", "GENERIC_PLACEHOLDER" }.Contains(s.DataOrigin) &&
                    (s.Purpose.Contains(cat) || s.DiscoveryCategories.Any(c => c.CategoryName.Contains(cat)))
                );
                coverage[cat] = count;
            }

            // Map some aliases for specific counting
            coverage["Business & Entrepreneurship"] = _context.Schemes.Count(s => 
                s.IsActive && s.DataOrigin == "OFFICIAL" && s.VerificationStatus.ToUpper() == "VERIFIED" && s.RulesetStatus == "RULESET_COMPLETE" &&
                (s.Purpose == "Business" || s.Purpose == "Income-Generating Activities" || s.DiscoveryCategories.Any(c => c.CategoryName == "Business" || c.CategoryName == "Income-Generating Activities"))
            );

            coverage["Education & Learning"] = _context.Schemes.Count(s => 
                s.IsActive && s.DataOrigin == "OFFICIAL" && s.VerificationStatus.ToUpper() == "VERIFIED" && s.RulesetStatus == "RULESET_COMPLETE" &&
                (s.Purpose == "Scholarship" || s.DiscoveryCategories.Any(c => c.CategoryName == "Education" || c.CategoryName == "Scholarship"))
            );

            coverage["Agriculture, Rural & Environment"] = _context.Schemes.Count(s => 
                s.IsActive && s.DataOrigin == "OFFICIAL" && s.VerificationStatus.ToUpper() == "VERIFIED" && s.RulesetStatus == "RULESET_COMPLETE" &&
                (s.Purpose == "Agriculture" || s.DiscoveryCategories.Any(c => c.CategoryName == "Agriculture"))
            );

            return Ok(coverage);
        }

                        private string GetApplicationRoute(Scheme scheme, (string EligibilityState, List<object> Passed, List<object> Failed, List<string> Missing) evalRes)
        {
            string windowStatus = "UNKNOWN";
            var window = scheme.ApplicationWindows.OrderByDescending(w => w.EndDate).FirstOrDefault();
            if (window != null)
            {
                windowStatus = window.Status ?? "UNKNOWN";
            }

            if (evalRes.EligibilityState == "Not Eligible") return "Not Eligible";
            if (windowStatus == "CLOSED") return "Applications Closed";
            if (windowStatus == "NOT_YET_OPEN") return "Not Yet Open";

            if (scheme.ApplicationMode == "OFFICIAL_ONLINE_PORTAL")
            {
                if (!string.IsNullOrEmpty(scheme.ApplicationPortal))
                    return $"APPLY ON {scheme.ApplicationPortal.ToUpper()}";
                return "APPLY ON OFFICIAL PORTAL";
            }
            if (scheme.ApplicationMode == "PARTNER_ROUTED")
                return "CONTACT AUTHORIZED CHANNEL PARTNER";
            if (scheme.ApplicationMode == "INSTITUTION_ROUTED")
                return "APPLY THROUGH INSTITUTION";
            if (scheme.ApplicationMode == "CSC_ROUTED")
                return "APPLY VIA CSC";
            if (scheme.ApplicationMode == "OFFLINE")
                return "OFFLINE APPLICATION";

            if (windowStatus == "OPEN" && !string.IsNullOrEmpty(scheme.OfficialApplicationUrl))
                return "Apply on Official Portal";

            return "Check Official Portal / Status not verified";
        }

        private (string EligibilityState, List<object> Passed, List<object> Failed, List<string> Missing) EvaluateRules(List<SchemeEligibilityRule> rules, UserProfile profile)
        {
            var passed = new List<object>();
            var failed = new List<object>();
            var missing = new List<string>();
            bool hasMandatoryRule = false;
            bool hasEvaluatedMandatoryPass = false;

            foreach (var rule in rules)
            {
                // Skip scheme output/info fields — not user eligibility criteria
                if (rule.Field == "InterestRate" || rule.Field == "MaximumLoan" || rule.Field == "Tenure" || rule.Field == "MarginMoney" || rule.Field == "SubsidyRate" || rule.Field == "Moratorium" || rule.Field == "SpecialEligibility") continue;

                if (rule.Mandatory) hasMandatoryRule = true;

                if (!string.IsNullOrEmpty(rule.ConditionField))
                {
                    var condUserVal = ResolveValue(profile, rule.ConditionField);
                    if (condUserVal == null || string.IsNullOrWhiteSpace(condUserVal.ToString()))
                    {
                        if (rule.Mandatory) missing.Add(rule.ConditionField);
                        continue;
                    }
                    bool condPass = false;
                    string cOp = rule.ConditionOperator ?? "==";
                    if (cOp == "==" || cOp == "Equals")
                    {
                        condPass = condUserVal.ToString().Equals(rule.ConditionValue, StringComparison.OrdinalIgnoreCase);
                    }
                    else if (cOp == ">" || cOp == "GreaterThan")
                    {
                        if (decimal.TryParse(condUserVal.ToString().Replace(",", "").Replace("₹", "").Trim(), out decimal cuVal) && decimal.TryParse(rule.ConditionValue, out decimal cmVal))
                        {
                            condPass = cuVal > cmVal;
                        }
                    }
                    
                    if (!condPass) continue;
                }

                var userValue = ResolveValue(profile, rule.Field);
                if (userValue == null || string.IsNullOrWhiteSpace(userValue.ToString()))
                {
                    if (rule.Mandatory) missing.Add(rule.Field);
                    continue;
                }

                bool isPass = false;
                string expectedCondition = rule.Value ?? "";
                string op = rule.Operator ?? "";
                string userValueStr = userValue.ToString() ?? "";

                if (op == "Equals" || op == "InList")
                {
                    var vals = (rule.Value ?? "").Split(',').Select(s => s.Trim().ToLower()).ToList();
                    isPass = vals.Contains(userValueStr.ToLower());
                }
                else if (op == "LessThanOrEqual" || op == "Max")
                {
                    string cleanVal = userValueStr.Replace(",", "").Replace("₹", "").Trim();
                    if (decimal.TryParse(cleanVal, out decimal uVal) && decimal.TryParse(rule.Value, out decimal maxVal))
                    {
                        isPass = uVal <= maxVal;
                        expectedCondition = $"Max {rule.Value}";
                    }
                }
                else if (op == "GreaterThanOrEqual" || op == "Min")
                {
                    string cleanVal = userValueStr.Replace(",", "").Replace("₹", "").Trim();
                    if (decimal.TryParse(cleanVal, out decimal uVal) && decimal.TryParse(rule.Value, out decimal minVal))
                    {
                        isPass = uVal >= minVal;
                        expectedCondition = $"Min {rule.Value}";
                    }
                }
                else if (op == "GreaterThan")
                {
                    string cleanVal = userValueStr.Replace(",", "").Replace("₹", "").Trim();
                    if (decimal.TryParse(cleanVal, out decimal uVal) && decimal.TryParse(rule.Value, out decimal minVal))
                    {
                        isPass = uVal > minVal;
                        expectedCondition = $"More than {rule.Value}";
                    }
                }
                else if (op == "LessThan")
                {
                    string cleanVal = userValueStr.Replace(",", "").Replace("₹", "").Trim();
                    if (decimal.TryParse(cleanVal, out decimal uVal) && decimal.TryParse(rule.Value, out decimal maxVal))
                    {
                        isPass = uVal < maxVal;
                        expectedCondition = $"Less than {rule.Value}";
                    }
                }
                else 
                {
                    // Fallback for Unknown operator, if mandatory we should fail it to be safe
                    // Actually, if it's unknown operator but matched exact, we can pass, otherwise fail.
                    isPass = userValueStr.Equals(expectedCondition, StringComparison.OrdinalIgnoreCase);
                }

                if (isPass)
                {
                    passed.Add(new { ruleName = rule.Field, userValue = userValueStr, schemeCondition = expectedCondition, status = "Matched" });
                    if (rule.Mandatory) hasEvaluatedMandatoryPass = true;
                }
                else
                {
                    failed.Add(new { ruleName = rule.Field, userValue = userValueStr, schemeCondition = expectedCondition, status = "Failed" });
                }
            }

            string state = "Eligible";
            if (!hasMandatoryRule) state = "Not Eligible"; // No-rule schemes must NOT become eligible
            else if (failed.Any(f => true)) state = "Not Eligible"; // Any FAIL -> NOT ELIGIBLE
            else if (missing.Any()) state = "More Information Needed"; // No FAIL + at least one mandatory UNKNOWN
            else if (!hasEvaluatedMandatoryPass) state = "Not Eligible"; // No evaluated rule must NOT mean eligible
            else state = "Eligible"; // all applicable mandatory rules PASS

            return (state, passed, failed, missing);
        }

        private object? ResolveValue(UserProfile profile, string field)
        {
            // Mappings
            if (field == "Community" || field == "category") return profile.Category;
            if (field == "AnnualFamilyIncome" || field == "income") return profile.Income;
            if (field == "State" || field == "state") return profile.State;
            if (field == "Gender") return profile.Gender;
            if (field == "IsPwD") return profile.IsPwD;
            if (field == "DisabilityPercentageMin") return profile.DisabilityPercentage;
            if (field == "ApplicantType") return profile.ApplicantType;
            
            if (field.StartsWith("ProjectCost") && profile.DynamicAnswers != null)
            {
                if (profile.DynamicAnswers.ContainsKey("ProjectCostMax")) return profile.DynamicAnswers["ProjectCostMax"];
                if (profile.DynamicAnswers.ContainsKey("projectCost")) return profile.DynamicAnswers["projectCost"];
            }

            var prop = typeof(UserProfile).GetProperty(char.ToUpper(field[0]) + field.Substring(1));
            if (prop != null)
            {
                var val = prop.GetValue(profile);
                if (val != null) return val;
            }

            if (profile.DynamicAnswers != null)
            {
                if (profile.DynamicAnswers.ContainsKey(field)) return profile.DynamicAnswers[field];
                var camelField = char.ToLower(field[0]) + field.Substring(1);
                if (profile.DynamicAnswers.ContainsKey(camelField)) return profile.DynamicAnswers[camelField];
            }

            return null;
        }
    }

    [ApiController]
    [Route("api/locations")]
    public class LocationController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public LocationController(ApplicationDbContext context) { _context = context; }

        // ISO 3166-2 alias → canonical LGD state name (uppercase, as stored in DB)
        private static readonly Dictionary<string, string> _isoAliasToStateName = new(StringComparer.OrdinalIgnoreCase)
        {
            { "AN", "ANDAMAN AND NICOBAR ISLANDS" },
            { "AP", "ANDHRA PRADESH" },
            { "AR", "ARUNACHAL PRADESH" },
            { "AS", "ASSAM" },
            { "BR", "BIHAR" },
            { "CH", "CHANDIGARH" },
            { "CT", "CHHATTISGARH" },
            { "DN", "DADRA AND NAGAR HAVELI AND DAMAN AND DIU" },
            { "DL", "DELHI" },
            { "GA", "GOA" },
            { "GJ", "GUJARAT" },
            { "HR", "HARYANA" },
            { "HP", "HIMACHAL PRADESH" },
            { "JK", "JAMMU AND KASHMIR" },
            { "JH", "JHARKHAND" },
            { "KA", "KARNATAKA" },
            { "KL", "KERALA" },
            { "LA", "LADAKH" },
            { "LD", "LAKSHADWEEP" },
            { "MP", "MADHYA PRADESH" },
            { "MH", "MAHARASHTRA" },
            { "MN", "MANIPUR" },
            { "ML", "MEGHALAYA" },
            { "MZ", "MIZORAM" },
            { "NL", "NAGALAND" },
            { "OR", "ODISHA" },
            { "PY", "PUDUCHERRY" },
            { "PB", "PUNJAB" },
            { "RJ", "RAJASTHAN" },
            { "SK", "SIKKIM" },
            { "TN", "TAMIL NADU" },
            { "TG", "TELANGANA" },
            { "TR", "TRIPURA" },
            { "UP", "UTTAR PRADESH" },
            { "UT", "UTTARAKHAND" },
            { "WB", "WEST BENGAL" }
        };

        [HttpGet("states")]
        public IActionResult GetStates()
        {
            // Returns id (DB PK), code (LGD numeric), name, and dataOrigin for each state/UT.
            // Includes COMMUNITY_MIRROR rows while official LGD acquisition remains BLOCKED.
            // Frontend uses id as the stable district-lookup identifier.
            var states = _context.LocationMaster
                .Where(l => (l.Type == "State" || l.Type == "UT")
                    && (l.VerificationStatus == "VERIFIED" || l.DataOrigin == "COMMUNITY_MIRROR"))
                .Select(l => new { id = l.Id, code = l.Code, name = l.Name, dataOrigin = l.DataOrigin, verificationStatus = l.VerificationStatus })
                .OrderBy(l => l.name)
                .ToList();
            return Ok(states);
        }

        [HttpGet("districts")]
        public IActionResult GetDistricts([FromQuery] string state)
        {
            if (string.IsNullOrWhiteSpace(state)) return Ok(new object[0]);

            Models.LocationMaster? parent = null;

            // Resolution order:
            // 1. Try as DB integer Id (preferred — frontend sends this after fetching states)
            if (int.TryParse(state, out int stateId))
            {
                parent = _context.LocationMaster.FirstOrDefault(l => l.Id == stateId && (l.Type == "State" || l.Type == "UT"));
            }

            // 2. Try exact LGD numeric code match (string compare)
            if (parent == null)
            {
                parent = _context.LocationMaster.FirstOrDefault(l => l.Code == state && (l.Type == "State" || l.Type == "UT"));
            }

            // 3. Try ISO 3166-2 alias → canonical name lookup
            if (parent == null && _isoAliasToStateName.TryGetValue(state, out var canonicalName))
            {
                parent = _context.LocationMaster.FirstOrDefault(l =>
                    l.Name.ToUpper() == canonicalName && (l.Type == "State" || l.Type == "UT"));
            }

            // 4. Try case-insensitive name match (if frontend sends full name)
            if (parent == null)
            {
                var upperState = state.ToUpper();
                parent = _context.LocationMaster.FirstOrDefault(l =>
                    l.Name.ToUpper() == upperState && (l.Type == "State" || l.Type == "UT"));
            }

            if (parent == null) return Ok(new object[0]);

            // Return districts — includes COMMUNITY_MIRROR rows while official acquisition is BLOCKED.
            // Excludes nothing by VerificationStatus so dropdown functions during official-import pending state.
            var districts = _context.LocationMaster
                .Where(l => l.ParentId == parent.Id && l.Type == "District")
                .Select(l => new { code = l.Code, name = l.Name, dataOrigin = l.DataOrigin, verificationStatus = l.VerificationStatus })
                .OrderBy(l => l.name)
                .ToList();
            return Ok(districts);
        }

        /// <summary>
        /// Returns the current provenance status of LocationMaster data.
        /// Use this to determine if an official LGD CSV import is still required.
        /// </summary>
        [HttpGet("provenance-status")]
        public IActionResult GetProvenanceStatus()
        {
            var total = _context.LocationMaster.Count();
            var verified = _context.LocationMaster.Count(l => l.VerificationStatus == "VERIFIED" && l.DataOrigin == "OFFICIAL");
            var communityMirror = _context.LocationMaster.Count(l => l.DataOrigin == "COMMUNITY_MIRROR");
            var unverified = _context.LocationMaster.Count(l => l.VerificationStatus == "UNVERIFIED");

            var lgdSource = _context.GovernmentSources.FirstOrDefault(s => s.Id == "src-lgd");

            return Ok(new
            {
                totalRows = total,
                officialVerifiedRows = verified,
                communityMirrorRows = communityMirror,
                unverifiedRows = unverified,
                officialAcquisitionStatus = lgdSource?.ConnectionStatus ?? "NOT_REGISTERED",
                officialSource = "Local Government Directory (LGD), Ministry of Panchayati Raj, Government of India",
                officialSourceUrl = "https://lgdirectory.gov.in / https://data.gov.in (LGD States/Districts resources)",
                importInstruction = "POST an official LGD CSV file to /api/locations/import with field 'file' and 'dataType' (states|districts)",
                message = verified == 0
                    ? "WARNING: No OFFICIAL+VERIFIED rows. All data is from community mirror (github.com/planemad/india-local-government-directory). Official LGD CSV import required for production provenance."
                    : $"OK: {verified} OFFICIAL+VERIFIED rows present."
            });
        }

        /// <summary>
        /// Accepts an officially downloaded LGD CSV and imports it with OFFICIAL+VERIFIED provenance.
        /// Download the CSV from lgdirectory.gov.in or data.gov.in LGD resources.
        /// Required columns for states: state_code, state_name, state_or_ut_code_indicator
        /// Required columns for districts: district_code, district_name, state_code
        /// </summary>
        [HttpPost("import")]
        public async Task<IActionResult> ImportOfficialLgdCsv([FromForm] IFormFile file, [FromForm] string dataType)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { error = "No file uploaded. Download the official LGD CSV from lgdirectory.gov.in or data.gov.in and upload it here." });

            if (dataType != "states" && dataType != "districts")
                return BadRequest(new { error = "dataType must be 'states' or 'districts'" });

            string content;
            using (var reader = new System.IO.StreamReader(file.OpenReadStream()))
                content = await reader.ReadToEndAsync();

            var lines = content.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            if (lines.Length < 2)
                return BadRequest(new { error = "CSV file appears empty or has no data rows." });

            var header = lines[0].Split(',').Select(h => h.Trim().ToLower().Trim('"')).ToArray();

            // Validate expected LGD columns
            string[] requiredStateColumns = { "state_code", "state_name" };
            string[] requiredDistrictColumns = { "district_code", "district_name", "state_code" };
            var requiredCols = dataType == "states" ? requiredStateColumns : requiredDistrictColumns;

            var missingCols = requiredCols.Where(c => !header.Contains(c)).ToList();
            if (missingCols.Any())
            {
                // Try flexible LGD column names
                var altStateMap = new Dictionary<string, string[]> {
                    { "state_code", new[] { "statecode", "lgd_state_code", "code", "s.no", "state code" } },
                    { "state_name", new[] { "statename", "lgd_state_name", "name", "state name" } },
                    { "district_code", new[] { "districtcode", "lgd_district_code", "district code" } },
                    { "district_name", new[] { "districtname", "lgd_district_name", "district name" } }
                };
                // Allow if alternate column names match
                var stillMissing = missingCols.Where(c =>
                    !altStateMap.ContainsKey(c) || !altStateMap[c].Any(alt => header.Contains(alt))).ToList();
                if (stillMissing.Any())
                    return BadRequest(new { error = $"Missing required columns: {string.Join(", ", stillMissing)}. This does not appear to be an official LGD CSV.", detectedColumns = header });
            }

            // Compute a SHA256 hash of the uploaded file content for snapshot provenance
            string contentHash;
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var hashBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(content));
                contentHash = string.Concat(hashBytes.Select(b => b.ToString("x2")));
            }

            // Ensure LGD GovernmentSource exists
            var lgdSource = _context.GovernmentSources.FirstOrDefault(s => s.Id == "src-lgd");
            if (lgdSource == null)
            {
                lgdSource = new Models.GovernmentSource
                {
                    Id = "src-lgd",
                    SourceName = "Local Government Directory (LGD)",
                    Ministry = "Ministry of Panchayati Raj",
                    OfficialDomain = "lgdirectory.gov.in",
                    IsActive = true
                };
                _context.GovernmentSources.Add(lgdSource);
            }
            lgdSource.ConnectionStatus = "VERIFIED";
            lgdSource.IngestionMethod = "OFFICIAL_CSV_IMPORT";

            // Create SourceSnapshot
            var snapshot = new Models.SourceSnapshot
            {
                SourceId = "src-lgd",
                SnapshotDate = DateTime.UtcNow,
                SourceUrl = $"Official LGD CSV upload — {dataType} — filename: {file.FileName}",
                ContentHash = contentHash,
                VerificationStatus = "VERIFIED"
            };
            _context.SourceSnapshots.Add(snapshot);
            await _context.SaveChangesAsync();

            int imported = 0, updated = 0, skipped = 0;

            if (dataType == "states")
            {
                int codeIdx = Array.IndexOf(header, "state_code");
                if (codeIdx < 0) codeIdx = Array.IndexOf(header, "statecode");
                int nameIdx = Array.IndexOf(header, "state_name");
                if (nameIdx < 0) nameIdx = Array.IndexOf(header, "statename");
                int utIdx = Array.IndexOf(header, "state_or_ut_code_indicator");

                for (int i = 1; i < lines.Length; i++)
                {
                    var cols = lines[i].Split(',').Select(c => c.Trim().Trim('"')).ToArray();
                    if (cols.Length <= Math.Max(codeIdx, nameIdx)) { skipped++; continue; }
                    var code = cols[codeIdx];
                    var name = cols[nameIdx].ToUpper();
                    var type = (utIdx >= 0 && cols.Length > utIdx && cols[utIdx].ToUpper() == "U") ? "UT" : "State";
                    if (string.IsNullOrEmpty(code) || string.IsNullOrEmpty(name)) { skipped++; continue; }

                    var existing = _context.LocationMaster.FirstOrDefault(l => l.Code == code && (l.Type == "State" || l.Type == "UT"));
                    if (existing == null)
                    {
                        _context.LocationMaster.Add(new Models.LocationMaster { Code = code, Name = name, Type = type, DataOrigin = "OFFICIAL", VerificationStatus = "VERIFIED" });
                        imported++;
                    }
                    else
                    {
                        existing.Name = name;
                        existing.Type = type;
                        existing.DataOrigin = "OFFICIAL";
                        existing.VerificationStatus = "VERIFIED";
                        updated++;
                    }
                }
            }
            else // districts
            {
                int codeIdx = Array.IndexOf(header, "district_code");
                if (codeIdx < 0) codeIdx = Array.IndexOf(header, "districtcode");
                int nameIdx = Array.IndexOf(header, "district_name");
                if (nameIdx < 0) nameIdx = Array.IndexOf(header, "districtname");
                int stateCodeIdx = Array.IndexOf(header, "state_code");
                if (stateCodeIdx < 0) stateCodeIdx = Array.IndexOf(header, "statecode");

                for (int i = 1; i < lines.Length; i++)
                {
                    var cols = lines[i].Split(',').Select(c => c.Trim().Trim('"')).ToArray();
                    if (cols.Length <= Math.Max(codeIdx, Math.Max(nameIdx, stateCodeIdx))) { skipped++; continue; }
                    var code = cols[codeIdx];
                    var name = cols[nameIdx].ToUpper();
                    var stateCode = cols[stateCodeIdx];
                    if (string.IsNullOrEmpty(code) || string.IsNullOrEmpty(name)) { skipped++; continue; }

                    var stateRow = _context.LocationMaster.FirstOrDefault(l => l.Code == stateCode && (l.Type == "State" || l.Type == "UT"));
                    if (stateRow == null) { skipped++; continue; }

                    var existing = _context.LocationMaster.FirstOrDefault(l => l.Code == code && l.Type == "District");
                    if (existing == null)
                    {
                        _context.LocationMaster.Add(new Models.LocationMaster { Code = code, Name = name, Type = "District", ParentId = stateRow.Id, DataOrigin = "OFFICIAL", VerificationStatus = "VERIFIED" });
                        imported++;
                    }
                    else
                    {
                        existing.Name = name;
                        existing.ParentId = stateRow.Id;
                        existing.DataOrigin = "OFFICIAL";
                        existing.VerificationStatus = "VERIFIED";
                        updated++;
                    }
                }
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                dataType,
                rowsImported = imported,
                rowsUpdated = updated,
                rowsSkipped = skipped,
                contentHash,
                snapshotId = snapshot.Id,
                sourceId = "src-lgd",
                verificationStatus = "VERIFIED",
                dataOrigin = "OFFICIAL",
                message = $"Official LGD {dataType} imported. {imported} new, {updated} updated, {skipped} skipped."
            });
        }
    }

    public class UserProfile
    {
        public string? Purpose { get; set; }
        public string? Category { get; set; }
        public string? Income { get; set; }
        public string? State { get; set; }
        public string? Gender { get; set; }
        public bool? IsPwD { get; set; }
        public int? DisabilityPercentage { get; set; }
        public string? ApplicantType { get; set; }
        public int? Age { get; set; }
        public Dictionary<string, string>? DynamicAnswers { get; set; }
    }
}








