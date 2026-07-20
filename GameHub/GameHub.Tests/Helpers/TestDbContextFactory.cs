using GameHub.API.Data;
using Microsoft.EntityFrameworkCore;

namespace GameHub.Tests.Helpers;

public static class TestDbContextFactory
{
    public static GameHubDbContext Create(string? databaseName = null)
    {
        var options = new DbContextOptionsBuilder<GameHubDbContext>()
            .UseInMemoryDatabase(
                databaseName ?? Guid.NewGuid().ToString())
            .Options;

        var context = new GameHubDbContext(options);

        context.Database.EnsureCreated();

        return context;
    }
}
