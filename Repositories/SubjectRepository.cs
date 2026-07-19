using Dapper;
using StudentManagementSystemApp.Data;
using StudentManagementSystemApp.Extensions;
using StudentManagementSystemApp.Helpers;
using StudentManagementSystemApp.Interfaces;
using StudentManagementSystemApp.Models;

namespace StudentManagementSystemApp.Repositories;

public class SubjectRepository : ISubjectRepository
{
    private readonly DapperContext _context;

    public SubjectRepository(DapperContext context)
    {
        _context = context;
    }

    #region Get Active

    public async Task<IEnumerable<Subject>> GetActiveAsync()
    {
        using var connection = _context.CreateConnection();

        const string sql = """
        SELECT *
        FROM Subjects
        WHERE IsActive = 1
        ORDER BY SubjectName
        """;

        return await connection.QueryAsync<Subject>(sql);
    }

    #endregion

    #region Get History

    public async Task<IEnumerable<Subject>> GetInactiveAsync()
    {
        using var connection = _context.CreateConnection();

        const string sql = """
        SELECT *
        FROM Subjects
        WHERE IsActive = 0
        ORDER BY SubjectName
        """;

        return await connection.QueryAsync<Subject>(sql);
    }

    #endregion

    #region Pagination

    public Task<PagedResult<Subject>> GetPagedAsync(
        int page,
        int pageSize,
        string? search = null)
    {
        return GetPagedInternalAsync(
            true,
            page,
            pageSize,
            search);
    }

    public Task<PagedResult<Subject>> GetPagedHistoryAsync(
        int page,
        int pageSize,
        string? search = null)
    {
        return GetPagedInternalAsync(
            false,
            page,
            pageSize,
            search);
    }

    private async Task<PagedResult<Subject>> GetPagedInternalAsync(
        bool active,
        int page,
        int pageSize,
        string? search)
    {
        using var connection = _context.CreateConnection();

        var filter = new SqlFilterBuilder()

            .Where($"IsActive = {(active ? 1 : 0)}")

            .Search(
                search,
                "SubjectCode",
                "SubjectName");

        return await connection.GetPagedAsync<Subject>(

            selectSql:
            """
            SELECT *
            """,

            fromSql:
            """
            FROM Subjects
            """,

            whereSql:
            filter.BuildWhereClause(),

            orderBy:
            "SubjectCode ASC",

            page:
            page,

            pageSize:
            pageSize,

            parameters:
            filter.Parameters);
    }

    #endregion

    #region Get By Id

    public async Task<Subject?> GetByIdAsync(int id)
    {
        using var connection = _context.CreateConnection();

        const string sql = """
        SELECT *
        FROM Subjects
        WHERE Id=@Id
        """;

        return await connection.QueryFirstOrDefaultAsync<Subject>(
            sql,
            new { Id = id });
    }

    #endregion

    #region Add

    public async Task<int> AddAsync(Subject subject)
    {
        using var connection = _context.CreateConnection();

        const string sql = """
        INSERT INTO Subjects
        (
            SubjectCode,
            SubjectName,
            IsActive
        )
        VALUES
        (
            @SubjectCode,
            @SubjectName,
            @IsActive
        );

        SELECT CAST(SCOPE_IDENTITY() AS INT);
        """;

        return await connection.ExecuteScalarAsync<int>(
            sql,
            subject);
    }

    #endregion

    #region Update

    public async Task<bool> UpdateAsync(Subject subject)
    {
        using var connection = _context.CreateConnection();

        const string sql = """
        UPDATE Subjects
        SET
            SubjectCode=@SubjectCode,
            SubjectName=@SubjectName,
            IsActive=@IsActive,
            ModifiedDate=GETDATE()
        WHERE Id=@Id
        """;

        return await connection.ExecuteAsync(
            sql,
            subject) > 0;
    }

    #endregion

    #region Inactivate

    public async Task<bool> InactivateAsync(int id)
    {
        using var connection = _context.CreateConnection();

        const string sql = """
        UPDATE Subjects
        SET
            IsActive=0,
            ModifiedDate=GETDATE()
        WHERE Id=@Id
        """;

        return await connection.ExecuteAsync(
            sql,
            new { Id = id }) > 0;
    }

    #endregion

    #region Restore

    public async Task<bool> RestoreAsync(int id)
    {
        using var connection = _context.CreateConnection();

        const string sql = """
        UPDATE Subjects
        SET
            IsActive=1,
            ModifiedDate=GETDATE()
        WHERE Id=@Id
        """;

        return await connection.ExecuteAsync(
            sql,
            new { Id = id }) > 0;
    }

    #endregion

    #region Subject Code Exists

    public async Task<bool> SubjectCodeExistsAsync(
        string code,
        int? excludeId = null)
    {
        using var connection = _context.CreateConnection();

        const string sql = """
        SELECT COUNT(*)
        FROM Subjects
        WHERE SubjectCode=@Code
        AND (@ExcludeId IS NULL OR Id<>@ExcludeId)
        """;

        return await connection.ExecuteScalarAsync<int>(
            sql,
            new
            {
                Code = code,
                ExcludeId = excludeId
            }) > 0;
    }

    #endregion

    #region Subject Name Exists

    public async Task<bool> SubjectNameExistsAsync(
        string name,
        int? excludeId = null)
    {
        using var connection = _context.CreateConnection();

        const string sql = """
        SELECT COUNT(*)
        FROM Subjects
        WHERE SubjectName=@Name
        AND (@ExcludeId IS NULL OR Id<>@ExcludeId)
        """;

        return await connection.ExecuteScalarAsync<int>(
            sql,
            new
            {
                Name = name,
                ExcludeId = excludeId
            }) > 0;
    }

    #endregion
}