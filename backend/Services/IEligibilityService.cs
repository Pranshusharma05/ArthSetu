using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ArthSetu.Models;

namespace ArthSetu.Services
{
    public interface IEligibilityService
    {
        // The structured beneficiary profile is passed in, evaluates mandatory rules
        Task<EligibilityEvaluationResult> EvaluateEligibilityAsync(Guid schemeId, BeneficiaryProfile profile);
    }
    
    public class EligibilityEvaluationResult
    {
        public string EligibilityState { get; set; } // Eligible, Not Eligible, More Information Needed
        public List<string> PassedConditions { get; set; }
        public List<string> FailedConditions { get; set; }
        public List<string> MissingInputs { get; set; }
    }
    
    // A placeholder for BeneficiaryProfile
    public class BeneficiaryProfile
    {
        public string Purpose { get; set; }
        public string State { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Category { get; set; }
        public decimal? Income { get; set; }
        public string BusinessActivity { get; set; }
        public string EducationLevel { get; set; }
        public decimal? ProjectCost { get; set; }
        // ...
    }
}
