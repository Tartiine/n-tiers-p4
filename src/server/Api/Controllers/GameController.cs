using Microsoft.AspNetCore.Mvc;
using Api.Services;
using Api.Dtos;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GameController(GameService gameService) : ControllerBase
    {
        private readonly GameService _gameService = gameService;

        [HttpGet]
        public async Task<IActionResult> GetAllGames()
        {
            var games = await _gameService.GetAllGamesAsync();
            return Ok(games);
        }

        [HttpGet("status/{status}")]
        public async Task<IActionResult> GetGamesByStatus(string status)
        {
            var games = await _gameService.GetGamesByStatusAsync(status);
            return Ok(games);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetGameById(int id)
        {
            var game = await _gameService.GetGameByIdAsync(id);
            if (game == null)
                return NotFound(new { Message = "Game not found" });

            return Ok(game);
        }

        [HttpPost]
        public async Task<IActionResult> CreateGame([FromBody] CreateGameRequest request)
        {
            if (!ModelState.IsValid)
            {
                Console.WriteLine("Model state is invalid");
                return BadRequest(ModelState);
            }

            Console.WriteLine($"Attempting to create game with HostId: {request.HostId}");

            var response = await _gameService.CreateGameAsync(request.HostId);
            if (!response.Success)
            {
                Console.WriteLine($"Failed to create game: {response.Message}");
                return BadRequest(new { Message = response.Message });
            }

            Console.WriteLine($"Game created with ID: {response.Data}");
            return CreatedAtAction(nameof(GetGameById), new { id = response.Data }, response.Data);
        }


        [HttpPost("{id}/join")]
        public async Task<IActionResult> JoinGame(int id, [FromBody] JoinGameRequest request)
        {
            var response = await _gameService.JoinGameAsync(id, request.GuestId);
            if (!response.Success)
                return BadRequest(new { Message = response.Message });

            return Ok(response.Data);
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateGameStatus(int id, [FromBody] UpdateGameStatusRequest request)
        {
            var response = await _gameService.UpdateGameStatusAsync(id, request.Status);
            if (!response.Success)
                return BadRequest(new { Message = response.Message });

            return Ok(new { Message = response.Data });
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
