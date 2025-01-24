using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NTiersP4.API.v1.Controllers;
using NTiersP4.Domain.Model.Dtos;
using NTiersP4.Domain.Services;
using Xunit;

namespace NTiersP4.API.Tests.v1.Controllers
{
    public class GameControllerTests
    {
        private readonly Mock<GameService> _mockGameService;
        private readonly GameController _controller;

        public GameControllerTests()
        {
            _mockGameService = new Mock<GameService>();
            _controller = new GameController(_mockGameService.Object);
        }

        [Fact]
        public async Task GetAllGames_Should_Return_List_Of_Games()
        {
            // Arrange
            var games = new List<GameDto>
            {
                new GameDto { Id = 1, Status = "Active" },
                new GameDto { Id = 2, Status = "AwaitingPlayer" }
            };

            _mockGameService.Setup(s => s.GetAllGamesAsync()).ReturnsAsync(games);

            // Act
            var result = await _controller.GetAllGames() as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            var response = result.Value as List<GameDto>;
            Assert.Equal(2, response.Count);
        }

        [Fact]
        public async Task GetGameById_Should_Return_NotFound_When_Game_Does_Not_Exist()
        {
            // Arrange
            var serviceResponse = new ServiceResponse<GameDto>
            {
                Success = false,
                Data = null,
                Message = "Game not found"
            };

            _mockGameService.Setup(s => s.GetGameByIdAsync(99)).ReturnsAsync(serviceResponse);

            // Act
            var result = await _controller.GetGameById(99) as NotFoundObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(404, result.StatusCode);
            Assert.Contains("Game not found", result.Value.ToString());
        }

    }
}