using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NTiersP4.API.v1.Controllers;
using NTiersP4.Domain.Model;
using NTiersP4.Domain.Model.Dtos;
using NTiersP4.Domain.Services;
using Xunit;

namespace NTiersP4.API.Tests.v1.Controllers
{
    public class AuthControllerTests
    {
        private readonly Mock<PlayerService> _mockPlayerService;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _mockPlayerService = new Mock<PlayerService>();
            _controller = new AuthController(_mockPlayerService.Object);
        }

        [Fact]
        public async Task LoginAsync_Should_Return_Ok_When_Credentials_Are_Valid()
        {
            // Arrange
            var loginRequest = new LoginRequest { Login = "testuser", Password = "testpassword" };
            var loginResponse = new ServiceResponse<Player>
            {
                Success = true,
                Data = new Player { Id = 1, Login = "testuser" }, // Utilisation de Player au lieu de PlayerDto
                Message = "Login successful"
            };

            _mockPlayerService.Setup(s => s.AuthenticateAsync(loginRequest.Login, loginRequest.Password))
                .ReturnsAsync(loginResponse); // Retourne un ServiceResponse<Player>

            // Act
            var result = await _controller.LoginAsync(loginRequest) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            var response = result.Value as dynamic;
            Assert.Equal("Login successful", response.Message);
            Assert.Equal(1, response.PlayerId);
        }

        [Fact]
        public async Task LoginAsync_Should_Return_BadRequest_When_Credentials_Are_Empty()
        {
            // Arrange
            var loginRequest = new LoginRequest { Login = "", Password = "" };

            // Act
            var result = await _controller.LoginAsync(loginRequest) as BadRequestObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(400, result.StatusCode);
            Assert.Contains("Login and Password are required", result.Value.ToString());
        }
    }
}
