using NTiersP4.Domain.Contracts;
using NTiersP4.Domain.Model;

namespace NTiersP4.Domain.Repositories;

public interface IPlayerRepository : IRepository<Player>
{
     Task<Player> GetPlayerByCredentialsAsync(string login, string password);
}