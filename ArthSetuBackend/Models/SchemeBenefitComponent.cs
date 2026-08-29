namespace ArthSetuBackend.Models
{
    public class SchemeBenefitComponent
    {
        public int Id { get; set; }
        public string SchemeId { get; set; } = string.Empty;
        public Scheme? Scheme { get; set; }
        
        public string BenefitType { get; set; } = string.Empty; // e.g. "LOAN", "SCHOLARSHIP", "SUBSIDY", etc.
        public string? ComponentDescription { get; set; }
    }
}
