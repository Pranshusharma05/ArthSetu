using Microsoft.AspNetCore.Mvc;
using ArthSetuBackend.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace ArthSetuBackend.Controllers
{
    [ApiController]
    [Route("api/admin/sources")]
    public class AdminController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetSources()
        {
            var sources = await _context.GovernmentSources.Select(g => new {
                g.Id,
                g.SourceName,
                g.ConnectionStatus,
                g.LastSync,
                g.LastSuccessfulSync,
                g.LastVerified,
                VerifiedCount = _context.Schemes.Count(s => s.SourceId == g.Id && s.VerificationStatus == "Verified"),
                NeedsReviewCount = _context.Schemes.Count(s => s.SourceId == g.Id && s.VerificationStatus == "NEEDS_REVIEW"),
                ConflictsCount = 0,
                LatestSnapshot = "SN-" + (g.LastSync.HasValue ? g.LastSync.Value.ToString("yyyyMMdd") : "Unknown")
            }).ToListAsync();
            return Ok(sources);
        }
    }
}

