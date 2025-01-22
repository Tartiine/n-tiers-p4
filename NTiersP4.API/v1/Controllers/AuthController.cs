using Microsoft.AspNetCore.Mvc;
using NTiersP4.Domain.Services;
using NTiersP4.Domain.Model.Dtos;

namespace NTiersP4.API.v1.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(PlayerService playerService) : ControllerBase
{
    private readonly PlayerService _playerService = playerService;

    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync([FromBody] LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Login) || string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest(new { Message = "Login and Password are required." });
        }

        try
        {
            var response = await _playerService.AuthenticateAsync(request.Login, request.Password);
            if (!response.Success)
            {
                return Unauthorized(new { Message = response.Message });
            }

            var player = response.Data;

            Console.WriteLine($"Player {player.Login} logged in successfully with ID {player.Id}");
            return Ok(new { Message = response.Message, PlayerId = player.Id });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during login: {ex.Message}");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("{id}/login")]
    public async Task<IActionResult> GetPlayerLogin(int id)
    {
        try
        {
            var login = await _playerService.GetLoginById(id);

            if (login == null)
            {
                return NotFound(new
                {
                    Message = "Player not found"
                });
            }

            return Ok(new
            {
                Message = "Player login fetched successfully",
                Login = login
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching player login: {ex.Message}");
            return StatusCode(500, new
            {
                Message = "Internal server error",
                Details = ex.Message
            });
        }
    }
}