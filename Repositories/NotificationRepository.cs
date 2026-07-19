using Dapper;
using StudentManagementSystemApp.Data;
using StudentManagementSystemApp.Extensions;
using StudentManagementSystemApp.Helpers;
using StudentManagementSystemApp.Interfaces;
using StudentManagementSystemApp.Models;
using StudentManagementSystemApp.Models.Notification;

namespace StudentManagementSystemApp.Repositories;

public class NotificationRepository
    : INotificationRepository
{
    private readonly DapperContext _context;

    public NotificationRepository(
        DapperContext context)
    {
        _context = context;
    }

    public async Task AddLogAsync(
        NotificationLog log)
    {
        using var connection =
            _context.CreateConnection();

        const string sql =
        """
        INSERT INTO NotificationLogs
        (
            StudentId,
            Email,
            Subject,
            Body,
            NotificationType,
            Status,
            ErrorMessage,
            StudentName
        )

        VALUES
        (
            @StudentId,
            @Email,
            @Subject,
            @Body,
            @NotificationType,
            @Status,
            @ErrorMessage,
            @StudentName
        )
        """;

        await connection.ExecuteAsync(sql, log);
    }

    public async Task<IEnumerable<NotificationLog>> GetLogsAsync()
    {
        using var connection =
            _context.CreateConnection();

        return await connection.QueryAsync<NotificationLog>(
        """
        SELECT *

        FROM NotificationLogs

        ORDER BY SentDate DESC
        """);
    }

    public async Task<PagedResult<NotificationLog>> GetPagedAsync(
    int page,
    int pageSize,
    string? search)
    {
        using var connection = _context.CreateConnection();

        var filter = new SqlFilterBuilder()

            .Search(
                search,
                "StudentName",
                "Email",
                "NotificationType",
                "Status");

        return await connection.GetPagedAsync<NotificationLog>(

            selectSql:
    """
SELECT
    *
""",

            fromSql:
    """
FROM NotificationLogs
""",

            whereSql:
    filter.BuildWhereClause(),

            orderBy:
    "SentDate DESC",

            page:
    page,

            pageSize:
    pageSize,

            parameters:
    filter.Parameters);
    }


}