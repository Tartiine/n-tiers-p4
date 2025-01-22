using NTiersP4.Domain.Model;
using NTiersP4.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace NTiersP4.Infrastructure.Repositories;

public class GameRepository : IGameRepository
{
    private readonly DatabaseContext _context;

    public GameRepository(DatabaseContext context)
    {
        _context = context;
    }

    //IRepository
    public async Task AddAsync(Game entity)
    {
        await _context.Games.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Game entity)
    {
        _context.Games.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Game entity)
    {
        _context.Games.Remove(entity);
        await _context.SaveChangesAsync();
    }

    public async Task<Game> GetByIdAsync(int id)
    {
        return await _context.Games
            .Include(g => g.Grid)
                .ThenInclude(grid => grid.Cells)
                    .ThenInclude(cell => cell.Token)
            .Include(g => g.Host)
            .Include(g => g.Guest)
            .FirstOrDefaultAsync(g => g.Id == id);
    }

    public async Task<IEnumerable<Game>> GetAllAsync()
    {
        return await _context.Games
            .Include(g => g.Host)
            .Include(g => g.Guest)
            .ToListAsync();
    }

    //IGameRepository
    public async Task<IEnumerable<Game>> GetByStatusAsync(string status)
    {
        if (!Enum.TryParse<GameStatus>(status, true, out var gameStatus))
            return Enumerable.Empty<Game>();

        return await _context.Games
            .Include(g => g.Host)
            .Include(g => g.Guest)
            .Where(g => g.Status == gameStatus)
            .ToListAsync();
    }
}