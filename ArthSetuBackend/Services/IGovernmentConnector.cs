using System.Threading.Tasks;
using ArthSetuBackend.Models;

namespace ArthSetuBackend.Services
{
    public interface IGovernmentConnector
    {
        Task<SourceSyncLog> RunSyncAsync();
    }
}
