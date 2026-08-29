using Microsoft.AspNetCore.Mvc;
using ArthSetuBackend.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace ArthSetuBackend.Controllers
{
    [ApiController]
    [Route("api/partners")]
    public class PartnerController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PartnerController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetPartners([FromQuery] string? schemeId)
        {
            var query = _context.ChannelPartners.Where(p => p.IsActive);
            if (!string.IsNullOrEmpty(schemeId)) {
                query = query.Where(p => p.PartnerSchemes.Any(ps => ps.SchemeId == schemeId));
            }
            var partners = await query.Select(p => new {
                p.Id,
                p.Name,
                p.PartnerType,
                p.RegisteredAddress,
                p.State,
                p.Pincode,
                p.VerificationStatus,
                p.SourceSnapshot,
                p.LastVerifiedAt
            }).ToListAsync();
            return Ok(partners);
        }
    }
}
