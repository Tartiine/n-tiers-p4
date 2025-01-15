using Database;
using Api.Dtos;
using Microsoft.EntityFrameworkCore;
using Database.Models;

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

        public async Task<ServiceResponse<int>> CreateGameAsync(int hostId)
        {
            var host = await _context.Players.FindAsync(hostId);
            if (host == null)
            {
                return new ServiceResponse<int> { Success = false, Message = "Host not found." };
            }

            var game = new Game
            {
                Host = host,
                Status = "AwaitingGuest"
            };

            _context.Games.Add(game);
            await _context.SaveChangesAsync(); // This generates the Id

            return new ServiceResponse<int>
            {
                Success = true,
                Data = game.Id // Return the generated Id
            };
        }


        public async Task<ServiceResponse<GameDto>> JoinGameAsync(int gameId, int guestId)
        {
            var game = await _context.Games.Include(g => g.Host).Include(g => g.Guest).FirstOrDefaultAsync(g => g.Id == gameId);
            if (game == null || game.Status != "AwaitingGuest")
            {
                return new ServiceResponse<GameDto>
                {
                    Success = false,
                    Message = "Game not available to join."
                };
            }

            var guest = await _context.Players.FindAsync(guestId);
            if (guest == null)
            {
                return new ServiceResponse<GameDto>
                {
                    Success = false,
                    Message = "Guest not found."
                };
            }

            game.Guest = guest;
            game.Status = "InProgress";

            await _context.SaveChangesAsync();

            return new ServiceResponse<GameDto>
            {
                Success = true,
                Data = new GameDto
                {
                    HostName = game.Host.Login,
                    GuestName = game.Guest.Login,
                    Status = game.Status
                }
            };
        }

        public async Task<ServiceResponse<List<GameDto>>> GetGamesByStatusAsync(string status)
        {
            var games = await _context.Games
                .Include(g => g.Host)
                .Include(g => g.Guest)
                .Where(g => g.Status == status)
                .Select(g => new GameDto
                {
                    HostName = g.Host.Login,
                    GuestName = g.Guest != null ? g.Guest.Login : "Awaiting Guest",
                    Status = g.Status
                })
                .ToListAsync();

            return new ServiceResponse<List<GameDto>>
            {
                Success = true,
                Data = games
            };
        }

        public async Task<ServiceResponse<string>> UpdateGameStatusAsync(int id, string status)
        {
            var game = await _context.Games.FindAsync(id);
            if (game == null)
            {
                return new ServiceResponse<string>
                {
                    Success = false,
                    Message = "Game not found."
                };
            }

            game.Status = status;
            await _context.SaveChangesAsync();

            return new ServiceResponse<string>
            {
                Success = true,
                Data = "Status updated successfully."
            };
        }

        public async Task<ServiceResponse<GameDto>> GetGameByIdAsync(int id)
        {
            var game = await _context.Games
                .Include(g => g.Host)
                .Include(g => g.Guest)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (game == null)
            {
                return new ServiceResponse<GameDto>
                {
                    Success = false,
                    Message = "Game not found."
                };
            }

            return new ServiceResponse<GameDto>
            {
                Success = true,
                Data = new GameDto
                {
                    HostName = game.Host.Login,
                    GuestName = game.Guest?.Login ?? "Awaiting Guest",
                    Status = game.Status
                }
            };
        }

    }

    public class ServiceResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
    }
}