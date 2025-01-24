using System.Threading.Tasks;
using NTiersP4.Domain.Model;
using NTiersP4.Infrastructure;
using NTiersP4.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace NTiersP4.Infrastructure.Test.Repositories
{
    public class PlayerRepositoryTests
    {
        private readonly DatabaseContext _dbContext;
        private readonly PlayerRepository _repository;

        public PlayerRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase("TestDatabase")
                .Options;

            _dbContext = new DatabaseContext(options);
            _repository = new PlayerRepository(_dbContext);
        }

        [Fact]
        public async Task AddAsync_Should_Add_Player_To_Database()
        {
            // Arrange
            var player = new Player { Id = 1, Login = "testuser", Password = "testpassword" };

            // Act
            await _repository.AddAsync(player);

            // Assert
            var dbPlayer = await _dbContext.Players.FindAsync(1);
            Assert.NotNull(dbPlayer);
            Assert.Equal("testuser", dbPlayer.Login);
        }

        [Fact]
        public async Task GetPlayerByCredentialsAsync_Should_Return_Correct_Player()
        {
            // Arrange
            var player = new Player { Id = 1, Login = "testuser", Password = "testpassword" };
            await _dbContext.Players.AddAsync(player);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _repository.GetPlayerByCredentialsAsync("testuser", "testpassword");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("testuser", result.Login);
        }
    }
}