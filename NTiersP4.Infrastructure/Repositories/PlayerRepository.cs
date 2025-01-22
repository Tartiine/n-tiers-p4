using NTiersP4.Domain.Model;
using NTiersP4.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace NTiersP4.Infrastructure.Repositories;

public class PlayerRepository : IPlayerRepository
{
    private readonly DatabaseContext _context;

    public PlayerRepository(DatabaseContext context)
    {
        _context = context;
    }

    //IRepository
    public async Task AddAsync(Player entity)
    {
        await _context.Players.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Player entity)
    {
        _context.Players.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Player entity)
    {
        _context.Players.Remove(entity);
        await _context.SaveChangesAsync();
    }

    public async Task<Player> GetByIdAsync(int id)
    {
        return await _context.Players.FindAsync(id);
    }

    public async Task<IEnumerable<Player>> GetAllAsync()
    {
        return await _context.Players.ToListAsync();
    }

    //IPlayerRepository
    public async Task<Player> GetPlayerByCredentialsAsync(string login, string password)
    {
        return await _context.Players
            .FirstOrDefaultAsync(p => p.Login == login && p.Password == password);
    }
}