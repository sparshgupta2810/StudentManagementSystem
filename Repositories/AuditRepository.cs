using Dapper;
using StudentManagementSystemApp.Data;
using StudentManagementSystemApp.Extensions;
using StudentManagementSystemApp.Helpers;
using StudentManagementSystemApp.Interfaces;
using StudentManagementSystemApp.Models;

namespace StudentManagementSystemApp.Repositories;

public class AuditRepository : IAuditRepository
{
    private readonly DapperContext _context;

    public AuditRepository(DapperContext context)
    {
        _context = context;
    }

    #region Add

    public async Task AddAsync(AuditLog log)
    {
        using var connection = _context.CreateConnection();

        const string sql = """
        INSERT INTO AuditLogs
        (
            UserId,
            UserName,
            FullName,
            Email,
            RoleName,
            ModuleName,
            ActionName,
            EntityId,
            EntityName,
            Details,
            OldValue,
            NewValue
        )
        VALUES
        (
            @UserId,
            @UserName,
            @FullName,
            @Email,
            @RoleName,
            @ModuleName,
            @ActionName,
            @EntityId,
            @EntityName,
            @Details,
            @OldValue,
            @NewValue
        )
        """;

        await connection.ExecuteAsync(sql, log);
    }

    #endregion

    #region Pagination

    public async Task<PagedResult<AuditLog>> GetPagedAsync(
        int page,
        int pageSize,
        string? search = null)
    {
        using var connection = _context.CreateConnection();

        var filter = new SqlFilterBuilder()

            .Search(
                search,
                "UserName",
                "FullName",
                "Email",
                "RoleName",
                "ModuleName",
                "ActionName",
                "EntityName",
                "Details");

        return await connection.GetPagedAsync<AuditLog>(

            selectSql:
            """
            SELECT *
            """,

            fromSql:
            """
            FROM AuditLogs
            """,

            whereSql:
            filter.BuildWhereClause(),

            orderBy:
            "CreatedDate DESC",

            page:
            page,

            pageSize:
            pageSize,

            parameters:
            filter.Parameters);
    }

    #endregion
}