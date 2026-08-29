using System;
using System.ComponentModel.DataAnnotations;

namespace ArthSetu.Models
{
    public class SchemeComponent
    {
        [Key]
        public Guid ComponentId { get; set; }
        public Guid SchemeId { get; set; }
        public Scheme Scheme { get; set; }
        
        public string ComponentName { get; set; }
        public string BenefitType { get; set; }
        public string Description { get; set; }
    }
}
