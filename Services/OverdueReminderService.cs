using StudentManagementSystemApp.Interfaces;

namespace StudentManagementSystemApp.Services;

public class OverdueReminderService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public OverdueReminderService(
        IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await CheckOverdueBooksAsync();

            await Task.Delay(
                TimeSpan.FromHours(24),
                stoppingToken);
        }
    }

    private async Task CheckOverdueBooksAsync()
    {
        using var scope =
            _scopeFactory.CreateScope();

        var repository =
            scope.ServiceProvider
                .GetRequiredService<IBookIssueRepository>();

        var notification =
            scope.ServiceProvider
                .GetRequiredService<INotificationService>();

        var overdueBooks =
            await repository.GetOverdueBooksAsync();

        foreach (var issue in overdueBooks)
        {
            await notification.SendOverdueReminderAsync(issue);

            await repository.UpdateLastReminderSentAsync(issue.Id);
        }
    }
}