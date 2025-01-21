using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Database;
using Database.Models;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(DatabaseContext context) : ControllerBase
    {
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Login) || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(new { Message = "Login and Password are required." });
            }

            try
            {
                var player = await context.Players
                    .FirstOrDefaultAsync(p => p.Login == request.Login && p.Password == request.Password);

                if (player == null)
                {
                    return Unauthorized(new { Message = "Invalid credentials" });
                }

                Console.WriteLine($"Player {player.Login} logged in successfully with ID {player.Id}");
                return Ok(new
                {
                    Message = "Login successful",
                    PlayerId = player.Id
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during login: {ex.Message}");
                return StatusCode(500, new { Message = "Internal server error", Details = ex.Message });
            }
        }

        [HttpGet("{id}/login")]
        public async Task<IActionResult> GetPlayerLogin(int id)
        {
            try
            {
                var player = await context.Players
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (player == null)
                {
                    return NotFound(new
                    {
                        Message = "Player not found"
                    });
                }

                return Ok(new
                {
                    Message = "Player login fetched successfully",
                    Login = player.Login
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
    public class LoginRequest
    {
        public string Login { get; set; }
        public string Password { get; set; }
    }
}
