using System.Threading.Tasks;
using NTiersP4.Domain.Model;
using NTiersP4.Infrastructure;
using NTiersP4.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace NTiersP4.Infrastructure.Tests.Repositories
{
    public class GameRepositoryTests
    {
        private readonly DatabaseContext _dbContext;
        private readonly GameRepository _repository;

        public GameRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase("TestDatabase")
                .Options;

            _dbContext = new DatabaseContext(options);
            _repository = new GameRepository(_dbContext);

            ResetDatabase();
        }

        private void ResetDatabase()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Database.EnsureCreated();
        }

        [Fact]
        public async Task AddAsync_Should_Add_Game_To_Database()
        {
            // Arrange
            var grid = new Grid { Rows = 6, Columns = 7 };
            var player = new Player { Login = "testuser", Password = "testpassword" };
            var game = new Game { Grid = grid, Host = player, Status = GameStatus.AwaitingGuest };

            await _dbContext.Grids.AddAsync(grid);
            await _dbContext.Players.AddAsync(player);
            await _dbContext.SaveChangesAsync();

            // Act
            await _repository.AddAsync(game);

            // Assert
            var dbGame = await _dbContext.Games.FirstOrDefaultAsync();
            Assert.NotNull(dbGame);
            Assert.Equal(GameStatus.AwaitingGuest, dbGame.Status);
        }

        [Fact]
        public async Task GetByIdAsync_Should_Return_Correct_Game()
        {
            // Arrange
            var grid = new Grid { Rows = 6, Columns = 7 };
            var player = new Player { Login = "testuser", Password = "testpassword" };
            var game = new Game { Grid = grid, Host = player, Status = GameStatus.AwaitingGuest };

            await _dbContext.Grids.AddAsync(grid);
            await _dbContext.Players.AddAsync(player);
            await _dbContext.Games.AddAsync(game);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _repository.GetByIdAsync(game.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(game.Id, result.Id);
        }
    }
}
