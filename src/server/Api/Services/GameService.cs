using Database;
using Api.Dtos;
using Microsoft.EntityFrameworkCore;

namespace Api.Services
{
    public class GameService(DatabaseContext context)
    {
        private readonly DatabaseContext _context = context;

        public async Task<List<GameDto>> GetAllGamesAsync()
        {
            return await _context.Games
                .Include(g => g.Host)
                .Include(g => g.Guest)
                .Select(g => new GameDto
                {
                    Id = g.Id,
                    HostName = g.Host.Login,
                    GuestName = g.Guest != null ? g.Guest.Login : "Awaiting Guest",
                    Status = g.Status
                })
                .ToListAsync();
        }
    }
}
