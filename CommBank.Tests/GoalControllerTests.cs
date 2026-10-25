using CommBank.Controllers;
using CommBank.Services;
using CommBank.Models;
using CommBank.Tests.Fake;
using Microsoft.AspNetCore.Mvc;

namespace CommBank.Tests;

public class GoalControllerTests
{
    private readonly FakeCollections collections;

    public GoalControllerTests()
    {
        collections = new();
    }

    [Fact]
    public async Task GetAll()
    {
        // Arrange
        var goals = collections.GetGoals();
        var users = collections.GetUsers();

        IGoalsService goalsService =
            new FakeGoalsService(goals, goals[0]);

        IUsersService usersService =
            new FakeUsersService(users, users[0]);

        GoalController controller =
            new(goalsService, usersService);

        // Act
        var result = await controller.Get();

        // Assert
        Assert.Equal(goals.Count, result.Count);

        for (int i = 0; i < goals.Count; i++)
        {
            Assert.Equal(goals[i].Id, result[i].Id);
            Assert.Equal(goals[i].Name, result[i].Name);
        }
    }

    [Fact]
    public async Task Get()
    {
        // Arrange
        var goals = collections.GetGoals();
        var users = collections.GetUsers();

        IGoalsService goalsService =
            new FakeGoalsService(goals, goals[0]);

        IUsersService usersService =
            new FakeUsersService(users, users[0]);

        GoalController controller =
            new(goalsService, usersService);

        // Act
        var result = await controller.Get(goals[0].Id!);

        // Assert
        Assert.IsAssignableFrom<Goal>(result.Value);
        Assert.Equal(goals[0], result.Value);
        Assert.NotEqual(goals[1], result.Value);
    }

    [Fact]
    public async Task GetForUser_ReturnsOnlyMatchingGoals()
    {
        // Arrange
        var goals = new List<Goal>
        {
            new()
            {
                Id = "1",
                Name = "House Down Payment",
                UserId = "user-1",
                Icon = "home"
            },
            new()
            {
                Id = "2",
                Name = "Tesla Model Y",
                UserId = "user-1",
                Icon = "car"
            },
            new()
            {
                Id = "3",
                Name = "Trip to London",
                UserId = "user-2",
                Icon = "flight"
            }
        };

        var users = collections.GetUsers();

        var fakeGoalsService =
            new FakeGoalsService(goals, goals[0]);

        IUsersService usersService =
            new FakeUsersService(users, users[0]);

        GoalController controller =
            new(fakeGoalsService, usersService);

        // Act
        var result = await controller.GetForUser("user-1");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result!.Count);

        Assert.All(result, goal =>
            Assert.Equal("user-1", goal.UserId));

        Assert.Contains(result, goal =>
            goal.Name == "House Down Payment");

        Assert.Contains(result, goal =>
            goal.Name == "Tesla Model Y");

        Assert.DoesNotContain(result, goal =>
            goal.Name == "Trip to London");

        Assert.Equal(
            "user-1",
            fakeGoalsService.LastRequestedUserId);
    }

    [Fact]
    public async Task GetForUser_ReturnsEmptyListForUserWithoutGoals()
    {
        // Arrange
        var goals = new List<Goal>
        {
            new()
            {
                Id = "1",
                Name = "House Down Payment",
                UserId = "user-1"
            }
        };

        var users = collections.GetUsers();

        var fakeGoalsService =
            new FakeGoalsService(goals, goals[0]);

        IUsersService usersService =
            new FakeUsersService(users, users[0]);

        GoalController controller =
            new(fakeGoalsService, usersService);

        // Act
        var result = await controller.GetForUser("user-3");

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);

        Assert.Equal(
            "user-3",
            fakeGoalsService.LastRequestedUserId);
    }

    [Fact]
    public async Task GetForUser_PreservesGoalIconValues()
    {
        // Arrange
        var goals = new List<Goal>
        {
            new()
            {
                Id = "1",
                Name = "House Down Payment",
                UserId = "user-1",
                Icon = "🏠"
            },
            new()
            {
                Id = "2",
                Name = "Tesla Model Y",
                UserId = "user-1",
                Icon = "🚗"
            }
        };

        var users = collections.GetUsers();

        var fakeGoalsService =
            new FakeGoalsService(goals, goals[0]);

        IUsersService usersService =
            new FakeUsersService(users, users[0]);

        GoalController controller =
            new(fakeGoalsService, usersService);

        // Act
        var result = await controller.GetForUser("user-1");

        // Assert
       Assert.NotNull(result);
       Assert.Equal(2, result!.Count);

        Assert.Equal("🏠", result[0].Icon);
        Assert.Equal("🚗", result[1].Icon);
    }

    [Fact]
    public async Task GetForUser_AllowsNullGoalIcon()
    {
        // Arrange
        var goals = new List<Goal>
        {
            new()
            {
                Id = "1",
                Name = "House Down Payment",
                UserId = "user-1",
                Icon = null
            }
        };

        var users = collections.GetUsers();

        var fakeGoalsService =
            new FakeGoalsService(goals, goals[0]);

        IUsersService usersService =
            new FakeUsersService(users, users[0]);

        GoalController controller =
            new(fakeGoalsService, usersService);

        // Act
        var result = await controller.GetForUser("user-1");

        // Assert
        Assert.NotNull(result);

        var goal = Assert.Single(result);

        Assert.Equal("House Down Payment", goal.Name);
        Assert.Null(goal.Icon);
    }
}