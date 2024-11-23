using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Api.Data;
using Api.Models;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(DatabaseContext context) : ControllerBase
    {
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            try
            {
                var player = context.Players
                    .FirstOrDefault(p => p.Login == request.Login && p.Password == request.Password);
                if (player == null)
                {
                    return Unauthorized(new { Message = "Invalid credentials" });
                }
                return Ok(new { Message = "Login successful", PlayerId = player.Id });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error accessing database: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

    }

    public class LoginRequest
    {
        public string Login { get; set; }
        public string Password { get; set; }
    }
}
