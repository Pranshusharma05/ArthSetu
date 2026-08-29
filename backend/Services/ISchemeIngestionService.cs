using System;
using System.Threading.Tasks;
using ArthSetu.Models;

namespace ArthSetu.Services
{
    public interface ISchemeIngestionService
    {
        Task ProcessSnapshotAsync(SourceSnapshot snapshot);
        Task CreateOrUpdateCandidateSchemeAsync(Scheme parsedSchemeData);
    }
}
