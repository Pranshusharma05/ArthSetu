using ArthSetuBackend.Data;
using ArthSetuBackend.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ArthSetuBackend.Services
{
    public class RecommendationService
    {
        private readonly ApplicationDbContext _context;

        public RecommendationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Scheme>> GetRecommendationsAsync(object userProfile)
        {
            // Placeholder: Real rules logic to be executed when SQL Server is available
            return await _context.Schemes.ToListAsync();
        }
    }
}
