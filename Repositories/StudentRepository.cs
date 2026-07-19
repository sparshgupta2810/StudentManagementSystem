using Dapper;
using StudentManagementSystemApp.Data;
using StudentManagementSystemApp.Extensions;
using StudentManagementSystemApp.Helpers;
using StudentManagementSystemApp.Interfaces;
using StudentManagementSystemApp.Models;

namespace StudentManagementSystemApp.Repositories;

public class StudentRepository : IStudentRepository
{
    private readonly DapperContext _context;

    public StudentRepository(DapperContext context)
    {
        _context = context;
    }

    #region Active

    public async Task<IEnumerable<Student>> GetActiveAsync()
    {
        using var connection = _context.CreateConnection();

        const string sql = """
        SELECT
            s.*,
            d.DepartmentName
        FROM Students s
        INNER JOIN Departments d
            ON s.DepartmentId = d.Id
        WHERE s.IsActive = 1
        AND d.IsActive = 1
        ORDER BY s.RegistrationNo
        """;

        return await connection.QueryAsync<Student>(sql);
    }

    #endregion

    #region History

    public async Task<IEnumerable<Student>> GetInactiveAsync()
    {
        using var connection = _context.CreateConnection();

        const string sql = """
        SELECT
            s.*,
            d.DepartmentName
        FROM Students s
        INNER JOIN Departments d
            ON s.DepartmentId = d.Id
        WHERE s.IsActive = 0
        AND d.IsActive = 1
        ORDER BY s.RegistrationNo
        """;

        return await connection.QueryAsync<Student>(sql);
    }

    #endregion

    #region Pagination

    public Task<PagedResult<Student>> GetPagedAsync(
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

    public Task<PagedResult<Student>> GetHistoryPagedAsync(
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

    private async Task<PagedResult<Student>> GetPagedInternalAsync(
        bool active,
        int page,
        int pageSize,
        string? search)
    {
        using var connection = _context.CreateConnection();

        var filter = new SqlFilterBuilder()

            .Where($"s.IsActive = {(active ? 1 : 0)}")

            .Where("d.IsActive = 1")

            .Search(
                search,
                "s.RegistrationNo",
                "s.FirstName",
                "s.MiddleName",
                "s.LastName",
                "s.Email",
                "d.DepartmentName");

        return await connection.GetPagedAsync<Student>(

            selectSql:
            """
            SELECT
                s.*,
                d.DepartmentName
            """,

            fromSql:
            """
            FROM Students s
            INNER JOIN Departments d
                ON s.DepartmentId = d.Id
            """,

            whereSql:
            filter.BuildWhereClause(),

            orderBy:
            "s.RegistrationNo",

            page:
            page,

            pageSize:
            pageSize,

            parameters:
            filter.Parameters);
    }

    #endregion

    #region Get By Id

    public async Task<Student?> GetByIdAsync(int id)
    {
        using var connection = _context.CreateConnection();

        const string sql = """
        SELECT
            s.*,
            d.DepartmentName
        FROM Students s
        INNER JOIN Departments d
            ON s.DepartmentId = d.Id
        WHERE s.Id=@Id
        """;

        return await connection.QueryFirstOrDefaultAsync<Student>(
            sql,
            new { Id = id });
    }

    #endregion

    #region Add

    public async Task<int> AddAsync(Student student)
    {
        using var connection = _context.CreateConnection();

        const string sql = """
        INSERT INTO Students
        (
            RegistrationNo,
            FirstName,
            MiddleName,
            LastName,
            Email,
            PhoneNumber,
            Gender,
            DateOfBirth,
            Address,
            DepartmentId,
            AdmissionDate,
            IsActive
        )
        VALUES
        (
            @RegistrationNo,
            @FirstName,
            @MiddleName,
            @LastName,
            @Email,
            @PhoneNumber,
            @Gender,
            @DateOfBirth,
            @Address,
            @DepartmentId,
            @AdmissionDate,
            @IsActive
        );

        SELECT CAST(SCOPE_IDENTITY() AS INT);
        """;

        return await connection.ExecuteScalarAsync<int>(sql, student);
    }

    #endregion

    #region Update

    public async Task<bool> UpdateAsync(Student student)
    {
        using var connection = _context.CreateConnection();

        const string sql = """
        UPDATE Students
        SET
            RegistrationNo = @RegistrationNo,
            FirstName = @FirstName,
            MiddleName = @MiddleName,
            LastName = @LastName,
            Email = @Email,
            PhoneNumber = @PhoneNumber,
            Gender = @Gender,
            DateOfBirth = @DateOfBirth,
            Address = @Address,
            DepartmentId = @DepartmentId,
            AdmissionDate = @AdmissionDate,
            IsActive = @IsActive,
            ModifiedDate = GETDATE()
        WHERE Id = @Id
        """;

        return await connection.ExecuteAsync(sql, student) > 0;
    }

    #endregion

    #region Inactivate

    public async Task<bool> InactivateAsync(int id)
    {
        using var connection = _context.CreateConnection();

        const string sql = """
        UPDATE Students
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
        UPDATE Students
        SET
            IsActive = 1,
            ModifiedDate = GETDATE()
        WHERE Id = @Id
        """;

        return await connection.ExecuteAsync(sql, new { Id = id }) > 0;
    }

    #endregion

    #region Exists

    public async Task<bool> RegistrationExistsAsync(
        string registrationNo,
        int? excludeId = null)
    {
        using var connection = _context.CreateConnection();

        const string sql = """
        SELECT COUNT(*)
        FROM Students
        WHERE RegistrationNo = @RegistrationNo
        AND (@ExcludeId IS NULL OR Id <> @ExcludeId)
        """;

        return await connection.ExecuteScalarAsync<int>(
            sql,
            new
            {
                RegistrationNo = registrationNo,
                ExcludeId = excludeId
            }) > 0;
    }

    public async Task<bool> EmailExistsAsync(
        string email,
        int? excludeId = null)
    {
        using var connection = _context.CreateConnection();

        const string sql = """
        SELECT COUNT(*)
        FROM Students
        WHERE Email = @Email
        AND (@ExcludeId IS NULL OR Id <> @ExcludeId)
        """;

        return await connection.ExecuteScalarAsync<int>(
            sql,
            new
            {
                Email = email,
                ExcludeId = excludeId
            }) > 0;
    }

    public async Task<string?> GetEmailByIdAsync(int id)
    {
        using var connection = _context.CreateConnection();

        return await connection.ExecuteScalarAsync<string>(
        """
        SELECT Email
        FROM Students
        WHERE Id = @id
        """,
        new { id });
    }

    #endregion
}