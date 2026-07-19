using Dapper;
using StudentManagementSystemApp.Data;
using StudentManagementSystemApp.Extensions;
using StudentManagementSystemApp.Helpers;
using StudentManagementSystemApp.Models;

namespace StudentManagementSystemApp.Repositories;

public class DepartmentRepository : IDepartmentRepository
{
    private readonly DapperContext _context;

    public DepartmentRepository(DapperContext context)
    {
        _context = context;
    }

    #region Active

    public async Task<IEnumerable<Department>> GetActiveAsync()
    {
        using var connection = _context.CreateConnection();

        const string sql = """
        SELECT *
        FROM Departments
        WHERE IsActive = 1
        ORDER BY DepartmentName
        """;

        return await connection.QueryAsync<Department>(sql);
    }

    #endregion

    #region Pagination

    public Task<PagedResult<Department>> GetPagedAsync(
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

    public Task<PagedResult<Department>> GetPagedHistoryAsync(
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

    private async Task<PagedResult<Department>> GetPagedInternalAsync(
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
                "DepartmentCode",
                "DepartmentName");

        return await connection.GetPagedAsync<Department>(

            selectSql: """
            SELECT *
            """,

            fromSql: """
            FROM Departments
            """,

            whereSql: filter.BuildWhereClause(),

            orderBy: "DepartmentCode",

            page: page,

            pageSize: pageSize,

            parameters: filter.Parameters);
    }

    #endregion

    #region History

    public async Task<IEnumerable<Department>> GetInactiveAsync()
    {
        using var connection = _context.CreateConnection();

        const string sql = """
        SELECT *
        FROM Departments
        WHERE IsActive = 0
        ORDER BY DepartmentName
        """;

        return await connection.QueryAsync<Department>(sql);
    }

    #endregion

    #region Get By Id

    public async Task<Department?> GetByIdAsync(int id)
    {
        using var connection = _context.CreateConnection();

        const string sql = """
        SELECT *
        FROM Departments
        WHERE Id = @Id
        """;

        return await connection.QueryFirstOrDefaultAsync<Department>(
            sql,
            new { Id = id });
    }

    #endregion

    #region Add

    public async Task<int> AddAsync(Department department)
    {
        using var connection = _context.CreateConnection();

        const string sql = """
        INSERT INTO Departments
        (
            DepartmentCode,
            DepartmentName,
            Description,
            IsActive
        )
        VALUES
        (
            @DepartmentCode,
            @DepartmentName,
            @Description,
            1
        );

        SELECT CAST(SCOPE_IDENTITY() AS INT);
        """;

        return await connection.ExecuteScalarAsync<int>(
            sql,
            department);
    }

    #endregion

    #region Update

    public async Task<bool> UpdateAsync(Department department)
    {
        using var connection = _context.CreateConnection();

        const string sql = """
        UPDATE Departments
        SET
            DepartmentCode = @DepartmentCode,
            DepartmentName = @DepartmentName,
            Description = @Description,
            ModifiedDate = GETDATE()
        WHERE Id = @Id
        """;

        return await connection.ExecuteAsync(sql, department) > 0;
    }

    #endregion

    #region Inactivate

    public async Task<bool> InactivateAsync(int id)
    {
        using var connection = _context.CreateConnection();

        const string sql = """
        UPDATE Departments
        SET
            IsActive = 0,
            ModifiedDate = GETDATE()
        WHERE Id = @Id
        """;

        return await connection.ExecuteAsync(sql, new { Id = id }) > 0;
    }

    #endregion

    #region Restore

    public async Task<bool> RestoreAsync(int id)
    {
        using var connection = _context.CreateConnection();

        const string sql = """
        UPDATE Departments
        SET
            IsActive = 1,
            ModifiedDate = GETDATE()
        WHERE Id = @Id
        """;

        return await connection.ExecuteAsync(sql, new { Id = id }) > 0;
    }

    #endregion

    #region Exists

    public async Task<bool> DepartmentCodeExistsAsync(
        string code,
        int? excludeId = null)
    {
        using var connection = _context.CreateConnection();

        const string sql = """
        SELECT COUNT(*)
        FROM Departments
        WHERE DepartmentCode = @Code
        AND (@ExcludeId IS NULL OR Id <> @ExcludeId)
        """;

        return await connection.ExecuteScalarAsync<int>(
            sql,
            new
            {
                Code = code,
                ExcludeId = excludeId
            }) > 0;
    }

    public async Task<bool> DepartmentNameExistsAsync(
        string name,
        int? excludeId = null)
    {
        using var connection = _context.CreateConnection();

        const string sql = """
        SELECT COUNT(*)
        FROM Departments
        WHERE DepartmentName = @Name
        AND (@ExcludeId IS NULL OR Id <> @ExcludeId)
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