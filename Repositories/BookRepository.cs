using Dapper;
using StudentManagementSystemApp.Data;
using StudentManagementSystemApp.Extensions;
using StudentManagementSystemApp.Helpers;
using StudentManagementSystemApp.Interfaces;
using StudentManagementSystemApp.Models;

namespace StudentManagementSystemApp.Repositories;

public class BookRepository : IBookRepository
{
    private readonly DapperContext _context;

    public BookRepository(DapperContext context)
    {
        _context = context;
    }

    #region Active

    public async Task<IEnumerable<Book>> GetActiveAsync()
    {
        using var connection = _context.CreateConnection();

        const string sql = """
        SELECT *
        FROM Books
        WHERE IsActive = 1
        ORDER BY BookName
        """;

        return await connection.QueryAsync<Book>(sql);
    }

    #endregion

    #region History

    public async Task<IEnumerable<Book>> GetInactiveAsync()
    {
        using var connection = _context.CreateConnection();

        const string sql = """
        SELECT *
        FROM Books
        WHERE IsActive = 0
        ORDER BY BookName
        """;

        return await connection.QueryAsync<Book>(sql);
    }

    #endregion

    #region Pagination

    public Task<PagedResult<Book>> GetPagedAsync(
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

    public Task<PagedResult<Book>> GetHistoryPagedAsync(
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

    private async Task<PagedResult<Book>> GetPagedInternalAsync(
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
                "BookCode",
                "BookName",
                "Author");

        return await connection.GetPagedAsync<Book>(

            selectSql:
            """
            SELECT *
            """,

            fromSql:
            """
            FROM Books
            """,

            whereSql:
            filter.BuildWhereClause(),

            orderBy:
            "BookName",

            page:
            page,

            pageSize:
            pageSize,

            parameters:
            filter.Parameters);
    }

    #endregion

    #region Get By Id

    public async Task<Book?> GetByIdAsync(int id)
    {
        using var connection = _context.CreateConnection();

        const string sql = """
        SELECT *
        FROM Books
        WHERE Id = @Id
        """;

        return await connection.QueryFirstOrDefaultAsync<Book>(
            sql,
            new { Id = id });
    }

    #endregion

    #region Add

    public async Task<int> AddAsync(Book book)
    {
        using var connection = _context.CreateConnection();

        const string sql = """
        INSERT INTO Books
        (
            BookCode,
            BookName,
            Author,
            TotalCopies,
            AvailableCopies,
            IsActive
        )
        VALUES
        (
            @BookCode,
            @BookName,
            @Author,
            @TotalCopies,
            @AvailableCopies,
            @IsActive
        );

        SELECT CAST(SCOPE_IDENTITY() AS INT);
        """;

        return await connection.ExecuteScalarAsync<int>(
            sql,
            book);
    }

    #endregion

    #region Update

    public async Task<bool> UpdateAsync(Book book)
    {
        using var connection = _context.CreateConnection();

        const string sql = """
        UPDATE Books
        SET
            BookCode = @BookCode,
            BookName = @BookName,
            Author = @Author,
            TotalCopies = @TotalCopies,

            AvailableCopies =
                @TotalCopies -
                (
                    SELECT COUNT(*)
                    FROM BookIssues
                    WHERE BookId = @Id
                    AND Status = 'Issued'
                ),

            IsActive = @IsActive,
            ModifiedDate = GETDATE()

        WHERE Id = @Id
        """;

        return await connection.ExecuteAsync(
            sql,
            book) > 0;
    }

    #endregion

    #region Inactivate

    public async Task<bool> InactivateAsync(int id)
    {
        using var connection = _context.CreateConnection();

        const string sql = """
        UPDATE Books
        SET
            IsActive = 0,
            ModifiedDate = GETDATE()
        WHERE Id = @Id
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
        UPDATE Books
        SET
            IsActive = 1,
            ModifiedDate = GETDATE()
        WHERE Id = @Id
        """;

        return await connection.ExecuteAsync(
            sql,
            new { Id = id }) > 0;
    }

    #endregion

    #region Book Code Exists

    public async Task<bool> BookCodeExistsAsync(
        string code,
        int? excludeId = null)
    {
        using var connection = _context.CreateConnection();

        const string sql = """
        SELECT COUNT(*)
        FROM Books
        WHERE BookCode = @Code
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

    #endregion

    #region Book Name Exists

    public async Task<bool> BookNameExistsAsync(
        string name,
        int? excludeId = null)
    {
        using var connection = _context.CreateConnection();

        const string sql = """
        SELECT COUNT(*)
        FROM Books
        WHERE BookName = @Name
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