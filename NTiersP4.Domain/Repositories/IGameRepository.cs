using NTiersP4.Domain.Contracts;
using NTiersP4.Domain.Model;

namespace NTiersP4.Domain.Repositories;

public interface IGameRepository : IRepository<Game>
{
    Task<IEnumerable<Game>> GetByStatusAsync(string status);
}