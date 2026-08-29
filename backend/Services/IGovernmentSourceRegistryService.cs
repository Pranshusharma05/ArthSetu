using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ArthSetu.Models;

namespace ArthSetu.Services
{
    public interface IGovernmentSourceRegistryService
    {
        Task<GovernmentSource> RegisterSourceAsync(GovernmentSource source);
        Task<GovernmentSource> GetSourceByIdAsync(Guid sourceId);
        Task<IEnumerable<GovernmentSource>> GetAllActiveSourcesAsync();
        Task UpdateSourceStatusAsync(Guid sourceId, string status, string message = null);
        Task RecordSyncAttemptAsync(Guid sourceId, bool success);
    }
}
