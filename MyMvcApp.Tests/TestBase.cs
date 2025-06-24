using Microsoft.EntityFrameworkCore;
using MyMvcApp.Data;
using MyMvcApp.Models;

namespace MyMvcApp.Tests;

public class TestBase : IDisposable
{
    protected readonly UserDbContext _context;

    public TestBase()
    {
        var options = new DbContextOptionsBuilder<UserDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new UserDbContext(options);

        // Seed the database with test data
        var users = new[]
        {
            new User { Id = 1, Name = "John Doe", Email = "john@example.com", Phone = "123-456-7890" },
            new User { Id = 2, Name = "Jane Smith", Email = "jane@example.com", Phone = "234-567-8901" },
            new User { Id = 3, Name = "Bob Johnson", Email = "bob@example.com", Phone = "345-678-9012" }
        };

        _context.Users.AddRange(users);
        _context.SaveChanges();
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
