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
                                    .Where(s => 
                                        s.IsActive && 
                                        s.DataOrigin == "OFFICIAL" && 
                                        !excludedOrigins.Contains(s.DataOrigin) && 
                                        (s.VerificationStatus == "Verified" || s.VerificationStatus == "VERIFIED") && 
                                        (string.IsNullOrEmpty(profile.Purpose) || s.Purpose == targetPurpose || s.Purpose == profile.Purpose || matchCategoryAliases.Contains(s.SchemeCategory) || matchCategoryAliases.Contains(s.Purpose)))
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
                                    .Where(s => s.IsActive && s.DataOrigin == "OFFICIAL" && s.VerificationStatus == "Verified" && (string.IsNullOrEmpty(profile.Purpose) || s.Purpose == targetPurpose || s.Purpose == profile.Purpose || categoryAliasList.Contains(s.SchemeCategory) || categoryAliasList.Contains(s.Purpose)))
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
                if (addedQuestions >= 5) break; // ask max 5 at a time

                if ((m.Equals("ProjectCostMax", StringComparison.OrdinalIgnoreCase) || m.Equals("ProjectCostMin", StringComparison.OrdinalIgnoreCase) || m.Equals("projectCost", StringComparison.OrdinalIgnoreCase)) && !projectCostAsked)
                {
                    questions.Add(new { id = "projectCost", type = "currency", label = "Estimated Project Cost (₹)", helpText = "Total amount needed for your business or project" });
                    addedQuestions++;
                    projectCostAsked = true;
                }
                else if (m.Equals("BusinessActivity", StringComparison.OrdinalIgnoreCase))
                {
                    questions.Add(new { id = "BusinessActivity", type = "taxonomy", label = "Select Business / Economic Activity" });
                    addedQuestions++;
                }
                else if (m.Equals("Gender", StringComparison.OrdinalIgnoreCase) && string.IsNullOrEmpty(profile.Gender))
                {
                    questions.Add(new { id = "Gender", type = "single_choice", label = "Gender", options = new[] { new {label="Female", value="Female"}, new {label="Male", value="Male"}, new {label="Other", value="Other"} } });
                    addedQuestions++;
                }
                else if (m.Equals("IsPwD", StringComparison.OrdinalIgnoreCase) && !profile.IsPwD.HasValue)
                {
                    questions.Add(new { id = "IsPwD", type = "single_choice", label = "Are you a Person with Disability (PwD)?", options = new[] { new {label="Yes", value="true"}, new {label="No", value="false"} } });
                    addedQuestions++;
                }
                else if (m.Equals("DisabilityPercentageMin", StringComparison.OrdinalIgnoreCase) && !profile.DisabilityPercentage.HasValue && profile.IsPwD != false)
                {
                    questions.Add(new { id = "DisabilityPercentage", type = "numeric", label = "Disability Percentage (%)" });
                    addedQuestions++;
                }
                else if (m.Equals("ApplicantType", StringComparison.OrdinalIgnoreCase) && string.IsNullOrEmpty(profile.ApplicantType))
                {
                    questions.Add(new { id = "ApplicantType", type = "single_choice", label = "Applicant Type", options = new[] { new {label="Individual", value="Individual"}, new {label="Self-Help Group (SHG)", value="SHG"} } });
                    addedQuestions++;
                }
                else if (m.Equals("educationLevel", StringComparison.OrdinalIgnoreCase))
                {
                    questions.Add(new { id = "educationLevel", type = "single_choice", label = "Education Level", options = new[] { new {label="8th Pass", value="8th Pass"}, new {label="10th Pass", value="10th Pass"}, new {label="12th Pass", value="12th Pass"}, new {label="Undergraduate", value="Undergraduate"}, new {label="Postgraduate", value="Postgraduate"} } });
                    addedQuestions++;
                }
                else if (m.Equals("PriorGovernmentSubsidy", StringComparison.OrdinalIgnoreCase))
                {
                    questions.Add(new { id = "PriorGovernmentSubsidy", type = "single_choice", label = "Have you received any prior Government Subsidy?", options = new[] { new {label="Yes", value="Yes"}, new {label="No", value="No"} } });
                    addedQuestions++;
                }
                else if (m.Equals("RequestedLoanAmount", StringComparison.OrdinalIgnoreCase))
                {
                    questions.Add(new { id = "RequestedLoanAmount", type = "currency", label = "Requested Loan Amount (₹)" });
                    addedQuestions++;
                }
                else if (m.Equals("ExistingEnterprise", StringComparison.OrdinalIgnoreCase))
                {
                    questions.Add(new { id = "ExistingEnterprise", type = "single_choice", label = "Is this an existing enterprise?", options = new[] { new {label="Yes", value="Yes"}, new {label="No", value="No"} } });
                    addedQuestions++;
                }
                else if (m.Equals("Category", StringComparison.OrdinalIgnoreCase) && string.IsNullOrEmpty(profile.Category))
                {
                    questions.Add(new { id = "Category", type = "single_choice", label = "Social Category", options = new[] { new {label="General", value="General"}, new {label="SC", value="SC"}, new {label="ST", value="ST"}, new {label="OBC", value="OBC"} } });
                    addedQuestions++;
                }
                else if (m.Equals("AnnualFamilyIncome", StringComparison.OrdinalIgnoreCase) || m.Equals("income", StringComparison.OrdinalIgnoreCase))
                {
                    questions.Add(new { id = "income", type = "currency", label = "Annual Family Income (₹)" });
                    addedQuestions++;
                }
            }

            return Ok(questions);
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

        [HttpGet("states")]
        public IActionResult GetStates()
        {
            var states = _context.LocationMaster
                .Where(l => (l.Type == "State" || l.Type == "UT") && l.VerificationStatus == "VERIFIED")
                .Select(l => new { code = l.Code, name = l.Name })
                .ToList();
            return Ok(states);
        }

        [HttpGet("districts")]
        public IActionResult GetDistricts([FromQuery] string state)
        {
            var parent = _context.LocationMaster.FirstOrDefault(l => l.Code == state && (l.Type == "State" || l.Type == "UT"));
            if (parent == null) return Ok(new object[0]);

            var districts = _context.LocationMaster
                .Where(l => l.ParentId == parent.Id && l.Type == "District" && l.VerificationStatus == "VERIFIED")
                .Select(l => new { code = l.Code, name = l.Name })
                .ToList();
            return Ok(districts);
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
        public Dictionary<string, string>? DynamicAnswers { get; set; }
    }
}








