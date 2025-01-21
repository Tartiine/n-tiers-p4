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


        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateGameStatus(int id, [FromBody] UpdateGameStatusRequest request)
        {
            var response = await _gameService.UpdateGameStatusAsync(id, request.Status);
            if (!response.Success)
                return BadRequest(new { Message = response.Message });

            return Ok(new { Message = response.Data });
        }

        [HttpPost("{id}/playTurn")]
        public async Task<IActionResult> PlayTurn(int id, [FromBody] PlayTurnRequest request)
        {

            if (request.PlayerId <= 0)
            {
                return BadRequest(new { Success = false, Message = "Invalid PlayerId." });
            }

            if (request.Column < 0)
            {
                return BadRequest(new { Success = false, Message = "Invalid column." });
            }

            try
            {

                var response = await _gameService.PlayTurnAsync(id, request.PlayerId, request.Column);

                if (!response.Success)
                {
                    return BadRequest(new { Success = false, Message = response.Message });
                }

                return Ok(new { Success = true, Data = response.Data });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in PlayTurn for Game ID {id}, Player ID {request.PlayerId}, Column {request.Column}: {ex.Message}");
                return StatusCode(500, new { Success = false, Message = "An error occurred.", Details = ex.Message });
            }
        }


        [HttpPost("create")]
        public async Task<IActionResult> CreateGame(int hostId)
        {
            var response = await _gameService.CreateGameAsync(hostId);

            if (!response.Success)
            {
                return BadRequest(new { response.Message });
            }
            return Ok(new {Success = true, response.Message, data = response.Data });
        }


        [HttpPost("join")]
        public async Task<IActionResult> JoinGame([FromQuery] int gameId, [FromQuery] int guestId)
        {
            var response = await _gameService.JoinGameAsync(gameId, guestId);

            if (!response.Success)
            {
                return BadRequest(new { response.Message });
            }

            return Ok(new { Success = true, response.Message }); 
        }


        /*[HttpPost("leave")]
        public async Task<IActionResult> LeaveGame([FromQuery] int gameId, [FromQuery] int playerId)
        {
            try
            {
                var response = await _gameService.HandlePlayerLeavingAsync(gameId, playerId);

                if (!response.Success)
                {
                    return BadRequest(new
                    {
                        Success = false,
                        Message = response.Message
                    });
                }

                return Ok(new
                {
                    Success = true,
                    Data = response.Data
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "An unexpected error occurred."
                });
            }
        }*/

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

    public class PlayTurnRequest
    {
        public int PlayerId { get; set; }
        public int Column { get; set; }
    }
}
