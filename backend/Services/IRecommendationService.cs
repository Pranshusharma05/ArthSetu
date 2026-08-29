using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ArthSetu.Models;

namespace ArthSetu.Services
{
    public interface IRecommendationService
    {
        // Takes evaluated candidate schemes and ranks them
        Task<RecommendationResult> RankEligibleSchemesAsync(List<SchemeEligibilityEvaluation> evaluatedSchemes);
    }
    
    public class SchemeEligibilityEvaluation
    {
        public Scheme Scheme { get; set; }
        public EligibilityEvaluationResult Evaluation { get; set; }
    }
    
    public class RecommendationResult
    {
        public List<SchemeEligibilityEvaluation> Recommended { get; set; }
        public List<SchemeEligibilityEvaluation> OtherEligibleOptions { get; set; }
        public List<SchemeEligibilityEvaluation> MoreInformationNeeded { get; set; }
    }
}
