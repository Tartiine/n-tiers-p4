using System.Threading.Tasks;
using NTiersP4.Domain.Model;
using NTiersP4.Infrastructure;
using NTiersP4.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace NTiersP4.Infrastructure.Test.Repositories
{
    public class GameRepositoryTests
    {
        private readonly DatabaseContext _dbContext;
        private readonly GameRepository _repository;

        public GameRepositoryTests()
        {
            // Configuration d'une base de données en mémoire
            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase("TestDatabase")
                .Options;

            _dbContext = new DatabaseContext(options);
            _repository = new GameRepository(_dbContext);
        }

        [Fact]
        public async Task AddAsync_Should_Add_Game_To_Database()
        {
            // Arrange
            var game = new Game { Id = 1, Status = GameStatus.AwaitingGuest };

            // Act
            await _repository.AddAsync(game);

            // Assert
            var dbGame = await _dbContext.Games.FindAsync(1);
            Assert.NotNull(dbGame);
            Assert.Equal(GameStatus.AwaitingGuest, dbGame.Status);
        }

        [Fact]
        public async Task GetByIdAsync_Should_Return_Correct_Game()
        {
            // Arrange
            var game = new Game { Id = 1, Status = GameStatus.AwaitingGuest };
            await _dbContext.Games.AddAsync(game);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _repository.GetByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal(GameStatus.AwaitingGuest, result.Status);
        }
    }
}