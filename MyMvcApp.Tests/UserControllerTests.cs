using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyMvcApp.Controllers;
using MyMvcApp.Models;
using Xunit;

namespace MyMvcApp.Tests;

public class UserControllerTests : TestBase
{
    private readonly UserController _controller;

    public UserControllerTests() : base()
    {
        _controller = new UserController(_context);
    }

    [Fact]
    public async Task Index_ReturnsAllUsers_WhenSearchStringIsEmpty()
    {
        // Act
        var result = await _controller.Index(null) as ViewResult;
        var users = result?.Model as List<User>;

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(users);
        Assert.Equal(3, users!.Count);
        Assert.Null(result.ViewBag.CurrentSearch);
    }

    [Theory]
    [InlineData("john", 1)]
    [InlineData("JANE", 1)]
    [InlineData("234", 1)]
    [InlineData("example.com", 3)]
    [InlineData("xyz", 0)]
    public async Task Index_FiltersUsers_WhenSearchStringProvided(string searchString, int expectedCount)
    {
        // Act
        var result = await _controller.Index(searchString) as ViewResult;
        var users = result?.Model as List<User>;

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(users);
        Assert.Equal(expectedCount, users!.Count);
        Assert.Equal(searchString, result.ViewBag.CurrentSearch);
    }

    [Fact]
    public async Task Details_ReturnsUser_WhenUserExists()
    {
        // Act
        var result = await _controller.Details(1) as ViewResult;
        var user = result?.Model as User;

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(user);
        Assert.Equal("John Doe", user!.Name);
    }

    [Fact]
    public async Task Details_ReturnsNotFound_WhenUserDoesNotExist()
    {
        // Act
        var result = await _controller.Details(999);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public void Create_GET_ReturnsView()
    {
        // Act
        var result = _controller.Create();

        // Assert
        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public async Task Create_POST_RedirectsToIndex_WhenModelStateIsValid()
    {
        // Arrange
        var newUser = new User
        {
            Name = "Test User",
            Email = "test@example.com",
            Phone = "555-555-5555"
        };

        // Act
        var result = await _controller.Create(newUser);

        // Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);

        // Verify the user was added to the database
        var addedUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == "test@example.com");
        Assert.NotNull(addedUser);
        Assert.Equal("Test User", addedUser.Name);
    }

    [Fact]
    public async Task Create_POST_ReturnsView_WhenModelStateIsInvalid()
    {
        // Arrange
        _controller.ModelState.AddModelError("Name", "Name is required");
        var invalidUser = new User();

        // Act
        var result = await _controller.Create(invalidUser) as ViewResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(invalidUser, result.Model);
    }

    [Fact]
    public async Task Edit_GET_ReturnsView_WhenUserExists()
    {
        // Act
        var result = await _controller.Edit(1) as ViewResult;
        var user = result?.Model as User;

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(user);
        Assert.Equal("John Doe", user!.Name);
    }

    [Fact]
    public async Task Edit_GET_ReturnsNotFound_WhenUserDoesNotExist()
    {
        // Act
        var result = await _controller.Edit(999);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Edit_POST_RedirectsToIndex_WhenModelStateIsValid()
    {
        // Arrange
        var user = await _context.Users.FindAsync(1);
        user!.Name = "Updated Name";

        // Act
        var result = await _controller.Edit(1, user);

        // Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);

        // Verify the user was updated in the database
        var updatedUser = await _context.Users.FindAsync(1);
        Assert.Equal("Updated Name", updatedUser!.Name);
    }

    [Fact]
    public async Task Edit_POST_ReturnsBadRequest_WhenIdMismatch()
    {
        // Act
        var result = await _controller.Edit(999, new User { Id = 1 });

        // Assert
        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task Edit_POST_ReturnsView_WhenModelStateIsInvalid()
    {
        // Arrange
        _controller.ModelState.AddModelError("Name", "Name is required");
        var invalidUser = new User { Id = 1 };

        // Act
        var result = await _controller.Edit(1, invalidUser) as ViewResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(invalidUser, result.Model);
    }

    [Fact]
    public async Task Delete_GET_ReturnsView_WhenUserExists()
    {
        // Act
        var result = await _controller.Delete(1) as ViewResult;
        var user = result?.Model as User;

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(user);
        Assert.Equal("John Doe", user!.Name);
    }

    [Fact]
    public async Task Delete_GET_ReturnsNotFound_WhenUserDoesNotExist()
    {
        // Act
        var result = await _controller.Delete(999);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Delete_POST_RedirectsToIndex_WhenUserExists()
    {
        // Act
        var result = await _controller.DeleteConfirmed(1);

        // Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);

        // Verify the user was deleted from the database
        var deletedUser = await _context.Users.FindAsync(1);
        Assert.Null(deletedUser);
    }

    [Fact]
    public async Task Delete_POST_ReturnsNotFound_WhenUserDoesNotExist()
    {
        // Act
        var result = await _controller.DeleteConfirmed(999);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }
}
