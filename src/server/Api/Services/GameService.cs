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
                    GuestName = g.Guest != null ? g.Guest.Login : "None",
                    HostId = g.HostId,
                    Status = g.Status.ToString() 
                })
                .ToListAsync();
        }

        public async Task<ServiceResponse<List<GameDto>>> GetGamesByStatusAsync(string status)
        {
            if (!Enum.TryParse<GameStatus>(status, true, out var gameStatus))
            {
                return new ServiceResponse<List<GameDto>>
                {
                    Success = false,
                    Message = "Invalid game status."
                };
            }

            var games = await _context.Games
                .Include(g => g.Host)
                .Include(g => g.Guest)
                .Where(g => g.Status == gameStatus)
                .Select(g => new GameDto
                {
                    Id = g.Id,
                    HostName = g.Host.Login,
                    GuestName = g.Guest != null ? g.Guest.Login : "None",
                    Status = g.Status.ToString()
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
            if (!Enum.TryParse<GameStatus>(status, true, out var gameStatus))
            {
                return new ServiceResponse<string>
                {
                    Success = false,
                    Message = "Invalid game status."
                };
            }

            var game = await _context.Games.FindAsync(id);
            if (game == null)
            {
                return new ServiceResponse<string>
                {
                    Success = false,
                    Message = "Game not found."
                };
            }

            game.Status = gameStatus;
            await _context.SaveChangesAsync();

            return new ServiceResponse<string>
            {
                Success = true,
                Data = "Status updated successfully."
            };
        }

        public async Task<ServiceResponse<GameDto>> GetGameByIdAsync(int gameId)
        {
            // Fetch the game and related data
            var game = await _context.Games
                .Include(g => g.Grid)
                    .ThenInclude(grid => grid.Cells)
                        .ThenInclude(cell => cell.Token) // Ensure tokens are included
                .Include(g => g.Host)
                .Include(g => g.Guest)
                .FirstOrDefaultAsync(g => g.Id == gameId);

            if (game == null)
            {
                return new ServiceResponse<GameDto>
                {
                    Success = false,
                    Message = "Game not found."
                };
            }

            // Calculate total tokens and determine the current turn
            var totalTokens = game.Grid.Cells.Count(cell => cell.Token != null);
            var currentTurnPlayerId = totalTokens % 2 == 0 ? game.HostId : game.GuestId;

            // Debug logs for verification
            Console.WriteLine($"Total Tokens: {totalTokens}");
            Console.WriteLine($"Current Turn Player ID: {currentTurnPlayerId}");
            foreach (var cell in game.Grid.Cells)
            {
                Console.WriteLine($"Row: {cell.Row}, Column: {cell.Column}, Token: {cell.Token?.Color ?? "Empty"}");
            }

            // Map to GameDto and return
            return new ServiceResponse<GameDto>
            {
                Success = true,
                Data = new GameDto
                {
                    Id = game.Id,
                    HostId = game.HostId,
                    GuestId = game.GuestId,
                    HostName = game.Host?.Login ?? "No host",
                    GuestName = game.Guest?.Login ?? "Waiting for guest...",
                    CurrentTurnId = (int)currentTurnPlayerId,
                    Status = game.Status.ToString(),
                    Grid = new GridDto
                    {
                        Rows = game.Grid.Rows,
                        Columns = game.Grid.Columns,
                        Cells = game.Grid.Cells.Select(cell => new CellDto
                        {
                            Row = cell.Row,
                            Column = cell.Column,
                            Token = cell.Token != null ? new TokenDto { Color = cell.Token.Color } : null
                        }).ToList()
                    }
                }
            };
        }



        public async Task<ServiceResponse<GameDto>> PlayTurnAsync(int gameId, int playerId, int column)
        {
            var game = await _context.Games
                .Include(g => g.Grid)
                    .ThenInclude(grid => grid.Cells)
                        .ThenInclude(cell => cell.Token)
                .Include(g => g.Host)
                .Include(g => g.Guest)
                .FirstOrDefaultAsync(g => g.Id == gameId);

            if (game == null)
            {
                return new ServiceResponse<GameDto>
                {
                    Success = false,
                    Message = "Game not found."
                };
            }

            if (game.Status != GameStatus.InProgress)
            {
                return new ServiceResponse<GameDto>
                {
                    Success = false,
                    Message = "Game is finished."
                };
            }

            var player = playerId == game.HostId ? game.Host : playerId == game.GuestId ? game.Guest : null;
            if (player == null)
            {
                return new ServiceResponse<GameDto>
                {
                    Success = false,
                    Message = "Player is not part of this game."
                };
            }

            try
            {

                game.PlayTurn(player, column);

                await _context.SaveChangesAsync();

                var totalTokens = game.Grid.Cells.Count(cell => cell.Token != null);
                var currentTurnPlayerId = totalTokens % 2 == 0 ? game.HostId : game.GuestId;

                return new ServiceResponse<GameDto>
                {
                    Success = true,
                    Data = MapToGameDto(game, (int)currentTurnPlayerId)
                };
            }
            catch (InvalidOperationException ex)
            {
                return new ServiceResponse<GameDto>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in PlayTurnAsync: {ex.Message}");
                return new ServiceResponse<GameDto>
                {
                    Success = false,
                    Message = "An unexpected error occurred. Please try again."
                };
            }
        }



        public async Task<ServiceResponse<string>> JoinGameAsync(int gameId, int guestId)
        {
            var game = await _context.Games
                .Include(g => g.Grid)
                .Include(g => g.Host)
                .FirstOrDefaultAsync(g => g.Id == gameId);

            if (game == null)
            {
                return new ServiceResponse<string>
                {
                    Success = false,
                    Message = "Game not found."
                };
            }

            if (game.Status != GameStatus.AwaitingGuest)
            {
                return new ServiceResponse<string>
                {
                    Success = false,
                    Message = "Game is not available for joining."
                };
            }

            if (game.HostId == guestId)
            {
                return new ServiceResponse<string>
                {
                    Success = false,
                    Message = "Host cannot join their own game."
                };
            }

            game.GuestId = guestId;
            game.Status = GameStatus.InProgress;

            _context.Games.Update(game);
            await _context.SaveChangesAsync();

            return new ServiceResponse<string>
            {
                Success = true,
                Message = "Guest joined the game successfully."
            };
        }




        private static GameDto MapToGameDto(Game game, int currentTurnId)
        {
            return new GameDto
            {
                Id = game.Id,
                HostId = game.HostId,
                GuestId = game.GuestId,
                HostName = game.Host?.Login ?? "No host",
                GuestName = game.Guest?.Login ?? "Waiting for guest...",
                CurrentTurnId = currentTurnId,
                Status = game.Status.ToString(),
                Grid = new GridDto
                {
                    Rows = game.Grid.Rows,
                    Columns = game.Grid.Columns,
                    Cells = game.Grid.Cells.Select(cell => new CellDto
                    {
                        Row = cell.Row,
                        Column = cell.Column,
                        Token = cell.Token != null ? new TokenDto { Color = cell.Token.Color } : null
                    }).ToList()
                }
            };
        }





    public async Task<ServiceResponse<int>> CreateGameAsync(int hostId)
    {
        var grid = new Grid
        {
            Rows = 6,
            Columns = 7,
            Cells = Enumerable.Range(0, 6 * 7).Select(index => new Cell
            {
                Row = index / 7,
                Column = index % 7
            }).ToList()
        };

        _context.Grids.Add(grid);
        await _context.SaveChangesAsync();

        var game = new Game
        {
            Grid = grid,
            HostId = hostId,
            Status = GameStatus.AwaitingGuest
        };

        _context.Games.Add(game);
        await _context.SaveChangesAsync();

        return new ServiceResponse<int>
        {
            Success = true,
            Message = "Game created successfully.",
            Data = game.Id 
        };
    }

        
        /*
        public async Task<ServiceResponse<GameDto>> HandlePlayerLeavingAsync(int gameId, int playerId)
        {
            var game = await _context.Games
                .Include(g => g.Host)
                .Include(g => g.Guest)
                .FirstOrDefaultAsync(g => g.Id == gameId);

            if (game == null)
            {
                return new ServiceResponse<GameDto>
                {
                    Success = false,
                    Message = "Game not found."
                };
            }

            if (game.HostId == playerId)
            {
                game.Status = GameStatus.Finished;
            }

            else if (game.GuestId == playerId)
            {
                game.Status = GameStatus.Finished; 
            }
            else
            {
                return new ServiceResponse<GameDto>
                {
                    Success = false,
                    Message = "Player is not part of this game."
                };
            }

            await _context.SaveChangesAsync();

            return new ServiceResponse<GameDto>
            {
                Success = true,
                Data = MapToGameDto(game)
            };
        }*/

    }
    public class ServiceResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
    }
}
