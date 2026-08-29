using ArthSetuBackend.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace ArthSetuBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SchemeController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SchemeController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetSchemes([FromQuery] string? category, [FromQuery] string? purpose)
        {
            var query = _context.Schemes
                .Include(s => s.Source)
                .Where(s => s.IsActive);

            if (!string.IsNullOrEmpty(category))
                query = query.Where(s => s.SchemeCategory == category);

            if (!string.IsNullOrEmpty(purpose))
                query = query.Where(s => s.Purpose == purpose);

            var schemes = await query.ToListAsync();
            return Ok(schemes);
        }

        [HttpGet("loans")]
        public async Task<IActionResult> GetLoanSchemes()
        {
            var schemes = await _context.Schemes
                .Include(s => s.Source)
                .Where(s => s.IsActive && s.VerificationStatus == "Verified" && (s.LifecycleStatus == null || s.LifecycleStatus != "SUPERSEDED") && s.DataOrigin != "LEGACY_REFERENCE")
                .ToListAsync();

            var schemeIds = schemes.Select(s => s.Id).ToList();

            var benefitComponents = await _context.SchemeBenefitComponents
                .Where(bc => schemeIds.Contains(bc.SchemeId) && (bc.BenefitType == "Loan" || bc.BenefitType == "Interest Subvention" || bc.BenefitType == "Subsidy" || bc.BenefitType == "Education Loan" || bc.BenefitType == "Interest Subsidy"))
                .ToListAsync();

            var provenances = await _context.FieldProvenance
                .Where(fp => schemeIds.Contains(fp.EntityId) && fp.VerificationStatus == "Verified")
                .ToListAsync();

            return Ok(new {
                schemes = schemes,
                benefitComponents = benefitComponents,
                provenances = provenances
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSchemeById(string id)
        {
            var scheme = await _context.Schemes
                .Include(s => s.Source)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (scheme == null) return NotFound();

            return Ok(scheme);
        }
    }
}
