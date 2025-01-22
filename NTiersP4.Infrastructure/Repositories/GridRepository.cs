using NTiersP4.Domain.Model;
using NTiersP4.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace NTiersP4.Infrastructure.Repositories;

public class GridRepository : IGridRepository
{
    private readonly DatabaseContext _context;

    public GridRepository(DatabaseContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Grid entity)
    {
        await _context.Grids.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Grid entity)
    {
        _context.Grids.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Grid entity)
    {
        _context.Grids.Remove(entity);
        await _context.SaveChangesAsync();
    }

    public async Task<Grid> GetByIdAsync(int id)
    {
        return await _context.Grids.FindAsync(id);
    }

    public async Task<IEnumerable<Grid>> GetAllAsync()
    {
        return await _context.Grids.ToListAsync();
    }
}