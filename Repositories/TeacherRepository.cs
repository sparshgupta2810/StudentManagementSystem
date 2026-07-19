using Dapper;
using StudentManagementSystemApp.Data;
using StudentManagementSystemApp.Extensions;
using StudentManagementSystemApp.Helpers;
using StudentManagementSystemApp.Interfaces;
using StudentManagementSystemApp.Models;

namespace StudentManagementSystemApp.Repositories;

public class TeacherRepository : ITeacherRepository
{
    private readonly DapperContext _context;

    public TeacherRepository(DapperContext context)
    {
        _context = context;
    }

    #region Get Active

    public async Task<IEnumerable<Teacher>> GetActiveAsync()
    {
        using var connection = _context.CreateConnection();

        const string sql = """
        SELECT
            t.*,
            d.DepartmentName
        FROM Teachers t
        INNER JOIN Departments d
            ON t.DepartmentId = d.Id
        WHERE t.IsActive = 1
        AND d.IsActive = 1
        ORDER BY t.FirstName, t.LastName
        """;

        return await connection.QueryAsync<Teacher>(sql);
    }

    #endregion

    #region Get History

    public async Task<IEnumerable<Teacher>> GetInactiveAsync()
    {
        using var connection = _context.CreateConnection();

        const string sql = """
        SELECT
            t.*,
            d.DepartmentName
        FROM Teachers t
        INNER JOIN Departments d
            ON t.DepartmentId = d.Id
        WHERE t.IsActive = 0
        AND d.IsActive = 1
        ORDER BY t.FirstName, t.LastName
        """;

        return await connection.QueryAsync<Teacher>(sql);
    }

    #endregion

    #region Pagination

    public Task<PagedResult<Teacher>> GetPagedAsync(
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

    public Task<PagedResult<Teacher>> GetHistoryPagedAsync(
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

    private async Task<PagedResult<Teacher>> GetPagedInternalAsync(
        bool active,
        int page,
        int pageSize,
        string? search)
    {
        using var connection = _context.CreateConnection();

        var filter = new SqlFilterBuilder()

            .Where($"t.IsActive = {(active ? 1 : 0)}")

            .Where("d.IsActive = 1")

            .Search(
                search,
                "t.FirstName",
                "t.MiddleName",
                "t.LastName",
                "t.Email",
                "t.PhoneNumber",
                "t.Qualification",
                "d.DepartmentName");

        return await connection.GetPagedAsync<Teacher>(

            selectSql:
            """
            SELECT
                t.*,
                d.DepartmentName
            """,

            fromSql:
            """
            FROM Teachers t
            INNER JOIN Departments d
                ON t.DepartmentId = d.Id
            """,

            whereSql:
            filter.BuildWhereClause(),

            orderBy:
            "t.FirstName, t.LastName",

            page:
            page,

            pageSize:
            pageSize,

            parameters:
            filter.Parameters);
    }

    #endregion

    #region Get By Id

    public async Task<Teacher?> GetByIdAsync(int id)
    {
        using var connection = _context.CreateConnection();

        const string sql = """
        SELECT
            t.*,
            d.DepartmentName
        FROM Teachers t
        INNER JOIN Departments d
            ON t.DepartmentId = d.Id
        WHERE t.Id = @Id
        """;

        return await connection.QueryFirstOrDefaultAsync<Teacher>(
            sql,
            new { Id = id });
    }

    #endregion

    #region Add

    public async Task<int> AddAsync(Teacher teacher)
    {
        using var connection = _context.CreateConnection();

        const string sql = """
        INSERT INTO Teachers
        (
            FirstName,
            MiddleName,
            LastName,
            Email,
            PhoneNumber,
            Gender,
            DateOfBirth,
            Address,
            DepartmentId,
            JoiningDate,
            Qualification,
            Experience,
            IsActive
        )
        VALUES
        (
            @FirstName,
            @MiddleName,
            @LastName,
            @Email,
            @PhoneNumber,
            @Gender,
            @DateOfBirth,
            @Address,
            @DepartmentId,
            @JoiningDate,
            @Qualification,
            @Experience,
            @IsActive
        );

        SELECT CAST(SCOPE_IDENTITY() AS INT);
        """;

        return await connection.ExecuteScalarAsync<int>(
            sql,
            teacher);
    }

    #endregion

    #region Update

    public async Task<bool> UpdateAsync(Teacher teacher)
    {
        using var connection = _context.CreateConnection();

        const string sql = """
        UPDATE Teachers
        SET
            FirstName=@FirstName,
            MiddleName=@MiddleName,
            LastName=@LastName,
            Email=@Email,
            PhoneNumber=@PhoneNumber,
            Gender=@Gender,
            DateOfBirth=@DateOfBirth,
            Address=@Address,
            DepartmentId=@DepartmentId,
            JoiningDate=@JoiningDate,
            Qualification=@Qualification,
            Experience=@Experience,
            IsActive=@IsActive,
            ModifiedDate=GETDATE()
        WHERE Id=@Id
        """;

        return await connection.ExecuteAsync(
            sql,
            teacher) > 0;
    }

    #endregion

    #region Inactivate

    public async Task<bool> InactivateAsync(int id)
    {
        using var connection = _context.CreateConnection();

        const string sql = """
        UPDATE Teachers
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
        UPDATE Teachers
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

    #region Email Exists

    public async Task<bool> EmailExistsAsync(
        string email,
        int? excludeId = null)
    {
        using var connection = _context.CreateConnection();

        const string sql = """
        SELECT COUNT(*)
        FROM Teachers
        WHERE Email=@Email
        AND (@ExcludeId IS NULL OR Id<>@ExcludeId)
        """;

        return await connection.ExecuteScalarAsync<int>(
            sql,
            new
            {
                Email = email,
                ExcludeId = excludeId
            }) > 0;
    }

    #endregion

    #region Phone Exists

    public async Task<bool> PhoneExistsAsync(
        string phoneNumber,
        int? excludeId = null)
    {
        using var connection = _context.CreateConnection();

        const string sql = """
        SELECT COUNT(*)
        FROM Teachers
        WHERE PhoneNumber=@PhoneNumber
        AND (@ExcludeId IS NULL OR Id<>@ExcludeId)
        """;

        return await connection.ExecuteScalarAsync<int>(
            sql,
            new
            {
                PhoneNumber = phoneNumber,
                ExcludeId = excludeId
            }) > 0;
    }

    #endregion
}