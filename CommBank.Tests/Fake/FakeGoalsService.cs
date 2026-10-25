using CommBank.Models;
using CommBank.Services;

namespace CommBank.Tests.Fake;

public class FakeGoalsService : IGoalsService
{
    private readonly List<Goal> _goals;
    private readonly Goal _goal;

    public string? LastRequestedUserId { get; private set; }

    public FakeGoalsService(List<Goal> goals, Goal goal)
    {
        _goals = goals;
        _goal = goal;
    }

    public Task<List<Goal>> GetAsync() =>
        Task.FromResult(_goals);

    public Task<List<Goal>?> GetForUserAsync(string id)
    {
        LastRequestedUserId = id;

        List<Goal> result = _goals
            .Where(goal => goal.UserId == id)
            .ToList();

        return Task.FromResult<List<Goal>?>(result);
    }

    public Task<Goal?> GetAsync(string id) =>
        Task.FromResult<Goal?>(_goal);

    public Task CreateAsync(Goal newGoal) =>
        Task.CompletedTask;

    public Task UpdateAsync(string id, Goal updatedGoal) =>
        Task.CompletedTask;

    public Task RemoveAsync(string id) =>
        Task.CompletedTask;
}