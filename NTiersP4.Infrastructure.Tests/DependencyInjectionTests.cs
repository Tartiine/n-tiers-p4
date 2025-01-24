using Microsoft.Extensions.DependencyInjection;
using NTiersP4.Domain.Repositories;
using NTiersP4.Infrastructure;
using Xunit;

namespace NTiersP4.Infrastructure.Tests
{
    public class DependencyInjectionTests
    {
        [Fact]
        public void AddInfrastructure_Should_Register_Services()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            services.AddInfrastructure("TestDatabase.db");
            var provider = services.BuildServiceProvider();

            // Assert
            Assert.NotNull(provider.GetService<IGameRepository>());
            Assert.NotNull(provider.GetService<IPlayerRepository>());
            Assert.NotNull(provider.GetService<IGridRepository>());
        }
    }
}