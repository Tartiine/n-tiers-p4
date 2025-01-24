using System.Threading.Tasks;
using NTiersP4.Domain.Model;
using NTiersP4.Infrastructure;
using NTiersP4.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace NTiersP4.Infrastructure.Tests.Repositories
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

            ResetDatabase();
        }

        private void ResetDatabase()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Database.EnsureCreated();
        }

        [Fact]
        public async Task AddAsync_Should_Add_Player_To_Database()
        {
            // Arrange
            var player = new Player { Login = "testuser", Password = "testpassword" };

            // Act
            await _repository.AddAsync(player);

            // Assert
            var dbPlayer = await _dbContext.Players.FirstOrDefaultAsync();
            Assert.NotNull(dbPlayer);
            Assert.Equal("testuser", dbPlayer.Login);
        }
    }
}