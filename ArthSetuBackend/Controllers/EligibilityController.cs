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
            
            var activeSchemes = _context.Schemes
                                    .Include(s => s.Source)
                                      .Include(s => s.ApplicationWindows)
                                    .Where(s => 
                                        s.IsActive && 
                                        s.DataOrigin == "OFFICIAL" && 
                                        !excludedOrigins.Contains(s.DataOrigin) && 
                                        (s.VerificationStatus == "Verified" || s.VerificationStatus == "VERIFIED") && 
                                        (string.IsNullOrEmpty(profile.Purpose) || string.IsNullOrEmpty(s.Purpose) || s.Purpose == targetPurpose || s.Purpose == profile.Purpose))
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
            
            var activeSchemes = _context.Schemes
                                    .Where(s => s.IsActive && s.DataOrigin != "DEVELOPMENT_SEED" && s.VerificationStatus == "Verified" && (string.IsNullOrEmpty(profile.Purpose) || string.IsNullOrEmpty(s.Purpose) || s.Purpose == targetPurpose || s.Purpose == profile.Purpose))
                                    .ToList();
            
            var allRules = _context.SchemeEligibilityRules.ToList();
            var allMissing = new HashSet<string>();

            foreach (var scheme in activeSchemes)
            {
                var rules = allRules.Where(r => r.SchemeId == scheme.Id).ToList();
                var evalRes = EvaluateRules(rules, profile);
                if (evalRes.EligibilityState != "Not Eligible")
                {
                    foreach(var m in evalRes.Missing) allMissing.Add(m);
                }
            }

            var questions = new List<object>();

            if (allMissing.Contains("ProjectCostMax") || allMissing.Contains("ProjectCostMin") || allMissing.Contains("projectCost"))
            {
                questions.Add(new { id = "ProjectCostMax", type = "currency", label = "Estimated Project Cost (₹)", helpText = "Total amount needed for your business or project" });
            }
            if (allMissing.Contains("BusinessActivity") || allMissing.Contains("businessActivity"))
            {
                questions.Add(new { id = "BusinessActivity", type = "taxonomy", label = "Select Business / Economic Activity" });
            }
            
            if (allMissing.Contains("Gender") && string.IsNullOrEmpty(profile.Gender))
            {
                questions.Add(new { id = "Gender", type = "single_choice", label = "Gender", options = new[] { new {label="Female", value="Female"}, new {label="Male", value="Male"}, new {label="Other", value="Other"} } });
            }
            if (allMissing.Contains("IsPwD") && !profile.IsPwD.HasValue)
            {
                questions.Add(new { id = "IsPwD", type = "single_choice", label = "Are you a Person with Disability (PwD)?", options = new[] { new {label="Yes", value="true"}, new {label="No", value="false"} } });
            }
            if (allMissing.Contains("DisabilityPercentageMin") && !profile.DisabilityPercentage.HasValue && profile.IsPwD != false)
            {
                questions.Add(new { id = "DisabilityPercentage", type = "numeric", label = "Disability Percentage (%)" });
            }
            if (allMissing.Contains("ApplicantType") && string.IsNullOrEmpty(profile.ApplicantType))
            {
                questions.Add(new { id = "ApplicantType", type = "single_choice", label = "Applicant Type", options = new[] { new {label="Individual", value="Individual"}, new {label="Self-Help Group (SHG)", value="SHG"} } });
            }
            
            // Legacy Education questions
            if (allMissing.Contains("educationLevel"))
                questions.Add(new { id = "educationLevel", type = "single_choice", label = "Education Level", options = new[] { new {label="Undergraduate", value="Undergraduate"}, new {label="Postgraduate", value="Postgraduate"} } });
            if (allMissing.Contains("course"))
                questions.Add(new { id = "course", type = "text", label = "Course Name (e.g. B.Tech)" });
            if (allMissing.Contains("institutionRecognition"))
                questions.Add(new { id = "institutionRecognition", type = "single_choice", label = "Is your Institution officially Recognized?", options = new[] { new {label="Yes", value="Recognized"}, new {label="No", value="Unrecognized"} } });
            if (allMissing.Contains("class12Marks"))
                questions.Add(new { id = "class12Marks", type = "numeric", label = "Class XII Marks / Percentile" });

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

            foreach (var rule in rules)
            {
                // Skip scheme output/info fields — not user eligibility criteria
                if (rule.Field == "InterestRate") continue;
                if (rule.Field == "MaximumLoan") continue;
                if (rule.Field == "Tenure") continue;

                var userValue = ResolveValue(profile, rule.Field);
                if (userValue == null)
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

                if (isPass)
                    passed.Add(new { ruleName = rule.Field, userValue = userValueStr, schemeCondition = expectedCondition, status = "Matched" });
                else
                    failed.Add(new { ruleName = rule.Field, userValue = userValueStr, schemeCondition = expectedCondition, status = "Failed" });
            }

            string state = "Eligible";
            if (failed.Any()) state = "Not Eligible";
            else if (missing.Any()) state = "More Information Needed";

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
        [HttpGet("districts")]
        public IActionResult GetDistricts([FromQuery] string state)
        {
            if (state == "UP") return Ok(new[] { 
                new { code="AG", name="Agra" }, new { code="AL", name="Aligarh" }, new { code="AH", name="Allahabad" },
                new { code="AY", name="Ayodhya" }, new { code="AZ", name="Azamgarh" }, new { code="BR", name="Bareilly" },
                new { code="BI", name="Bijnor" }, new { code="BU", name="Budaun" }, new { code="BS", name="Bulandshahr" },
                new { code="GA", name="Ghaziabad" }, new { code="GO", name="Gorakhpur" }, new { code="JH", name="Jhansi" },
                new { code="KA", name="Kanpur Nagar" }, new { code="LU", name="Lucknow" }, new { code="ME", name="Meerut" },
                new { code="MO", name="Moradabad" }, new { code="NO", name="Noida" }, new { code="PR", name="Prayagraj" },
                new { code="VA", name="Varanasi" }
            });
            if (state == "MH") return Ok(new[] { new { code="MU", name="Mumbai" }, new { code="PU", name="Pune" } });
            if (state == "DL") return Ok(new[] { new { code="ND", name="New Delhi" } });
            return Ok(new[] { new { code = $"{state}-01", name = $"{state} District 1" }, new { code = $"{state}-02", name = $"{state} District 2" } });
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








