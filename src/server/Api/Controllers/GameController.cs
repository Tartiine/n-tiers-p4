using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Database;
using Database.Models;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GameController(DatabaseContext context) : ControllerBase
    {
        private readonly DatabaseContext _context = context;

        [HttpGet]
        public async Task<IActionResult> GetAllGames()
        {
            var games = await _context.Games.ToListAsync();
            return Ok(games);
        }

        [HttpGet("status/{status}")]
        public async Task<IActionResult> GetGamesByStatus(string status)
        {
            var games = await _context.Games.Where(g => g.Status == status).ToListAsync();
            return Ok(games);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetGameById(int id)
        {
            var game = await _context.Games.FindAsync(id);

            if (game == null)
                return NotFound();

            return Ok(game);
        }

        [HttpPost]
        public async Task<IActionResult> CreateGame([FromBody] CreateGameRequest request)
        {
            var game = new Game
            {
                HostId = request.HostId,
                Status = "AwaitingGuest"
            };

            _context.Games.Add(game);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetGameById), new { id = game.Id }, game);
        }

        [HttpPost("{id}/join")]
        public async Task<IActionResult> JoinGame(int id, [FromBody] JoinGameRequest request)
        {
            var game = await _context.Games.FindAsync(id);

            if (game == null || game.Status != "AwaitingGuest")
                return BadRequest(new { Message = "Game not available to join." });

            game.GuestId = request.GuestId;
            game.Status = "InProgress";

            await _context.SaveChangesAsync();
            return Ok(game);
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateGameStatus(int id, [FromBody] UpdateGameStatusRequest request)
        {
            var game = await _context.Games.FindAsync(id);

            if (game == null)
                return NotFound();

            game.Status = request.Status;

            await _context.SaveChangesAsync();
            return NoContent();
        }
    }

    public class CreateGameRequest
    {
        public int HostId { get; set; }
    }

    public class JoinGameRequest
    {
        public int GuestId { get; set; }
    }

    public class UpdateGameStatusRequest
    {
        public string Status { get; set; }
    }
}