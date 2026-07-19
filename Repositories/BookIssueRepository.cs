using Dapper;
using StudentManagementSystemApp.Data;
using StudentManagementSystemApp.Extensions;
using StudentManagementSystemApp.Helpers;
using StudentManagementSystemApp.Interfaces;
using StudentManagementSystemApp.Models;

namespace StudentManagementSystemApp.Repositories;

public class BookIssueRepository : IBookIssueRepository
{
    private readonly DapperContext _context;

    public BookIssueRepository(DapperContext context)
    {
        _context = context;
    }

    #region Get Current Issued Books

    public async Task<IEnumerable<BookIssue>> GetIssuedBooksAsync()
    {
        using var connection = _context.CreateConnection();

        const string sql = """
        SELECT
            bi.*,
            s.RegistrationNo,
            CONCAT(s.FirstName,' ',ISNULL(s.MiddleName,''),' ',s.LastName) AS StudentName,
            b.BookCode,
            b.BookName
        FROM BookIssues bi
        INNER JOIN Students s ON bi.StudentId=s.Id
        INNER JOIN Books b ON bi.BookId=b.Id
        WHERE bi.Status='Issued' AND b.isActive=1 AND s.IsActive=1
        ORDER BY bi.IssueDate DESC
        """;

        return await connection.QueryAsync<BookIssue>(sql);
    }

    #endregion

    #region Get History

    public async Task<IEnumerable<BookIssue>> GetHistoryAsync()
    {
        using var connection = _context.CreateConnection();

        const string sql = """
        SELECT
            bi.*,
            s.RegistrationNo,
            CONCAT(s.FirstName,' ',ISNULL(s.MiddleName,''),' ',s.LastName) AS StudentName,
            b.BookCode,
            b.BookName
        FROM BookIssues bi
        INNER JOIN Students s ON bi.StudentId=s.Id
        INNER JOIN Books b ON bi.BookId=b.Id
        WHERE bi.Status<>'Issued'
        ORDER BY bi.ModifiedDate DESC
        """;

        return await connection.QueryAsync<BookIssue>(sql);
    }

    #endregion

    #region Pagination

    public Task<PagedResult<BookIssue>> GetPagedIssuedAsync(
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

    public Task<PagedResult<BookIssue>> GetPagedHistoryAsync(
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

    private async Task<PagedResult<BookIssue>> GetPagedInternalAsync(
        bool issued,
        int page,
        int pageSize,
        string? search)
    {
        using var connection = _context.CreateConnection();

        var filter = new SqlFilterBuilder()

            .Where(issued
                ? "bi.Status='Issued'"
                : "bi.Status<>'Issued'")

            .Search(
                search,
                "s.RegistrationNo",
                "CONCAT(s.FirstName,' ',ISNULL(s.MiddleName,''),' ',s.LastName)",
                "b.BookCode",
                "b.BookName");

        return await connection.GetPagedAsync<BookIssue>(

            selectSql:
    """
SELECT
    bi.*,
    s.RegistrationNo,
    CONCAT(s.FirstName,' ',ISNULL(s.MiddleName,''),' ',s.LastName) AS StudentName,
    b.BookCode,
    b.BookName
""",

            fromSql:
    """
FROM BookIssues bi
INNER JOIN Students s
    ON bi.StudentId=s.Id
INNER JOIN Books b
    ON bi.BookId=b.Id
""",

            whereSql:
            filter.BuildWhereClause(),

            orderBy:
            issued
                ? """
CASE
    WHEN bi.DueDate < CAST(GETDATE() AS DATE) THEN 0
    ELSE 1
END,
bi.DueDate
"""
                : "bi.ReturnDate DESC, bi.Id DESC",

            page:
            page,

            pageSize:
            pageSize,

            parameters:
            filter.Parameters);
    }

    #endregion

    #region Get By Id

    public async Task<BookIssue?> GetByIdAsync(int id)
    {
        using var connection = _context.CreateConnection();

        const string sql = """
    SELECT
        bi.*,

        s.RegistrationNo,

        s.Email,

        CONCAT(
            s.FirstName,
            ' ',
            ISNULL(s.MiddleName,''),
            ' ',
            s.LastName
        ) AS StudentName,

        b.BookCode,

        b.BookName

    FROM BookIssues bi

    INNER JOIN Students s
        ON s.Id = bi.StudentId

    INNER JOIN Books b
        ON b.Id = bi.BookId

    WHERE bi.Id = @Id
    """;

        return await connection.QueryFirstOrDefaultAsync<BookIssue>(
            sql,
            new { Id = id });
    }

    #endregion

    #region Search Student

    public async Task<Student?> GetStudentByRegistrationNoAsync(string registrationNo)
    {
        using var connection = _context.CreateConnection();

        const string sql = """
        SELECT *
        FROM Students
        WHERE RegistrationNo=@RegistrationNo
        AND IsActive=1
        """;

        return await connection.QueryFirstOrDefaultAsync<Student>(
            sql,
            new { RegistrationNo = registrationNo });
    }

    #endregion

    #region Available Books

    public async Task<IEnumerable<Book>> GetAvailableBooksAsync()
    {
        using var connection = _context.CreateConnection();

        const string sql = """
        SELECT *
        FROM Books
        WHERE IsActive=1
        AND AvailableCopies>0
        ORDER BY BookName
        """;

        return await connection.QueryAsync<Book>(sql);
    }

    #endregion

    #region Available Copies

    public async Task<int> GetAvailableCopiesAsync(int bookId)
    {
        using var connection = _context.CreateConnection();

        const string sql = """
        SELECT AvailableCopies
        FROM Books
        WHERE Id=@BookId
        """;

        return await connection.ExecuteScalarAsync<int>(
            sql,
            new { BookId = bookId });
    }

    #endregion

    #region Already Issued

    public async Task<bool> IsBookAlreadyIssuedAsync(int studentId, int bookId)
    {
        using var connection = _context.CreateConnection();

        const string sql = """
        SELECT COUNT(*)
        FROM BookIssues
        WHERE StudentId=@StudentId
        AND BookId=@BookId
        AND Status='Issued'
        """;

        return await connection.ExecuteScalarAsync<int>(
            sql,
            new
            {
                StudentId = studentId,
                BookId = bookId
            }) > 0;
    }

    #endregion

    #region Issue Book

    public async Task<int> IssueBookAsync(BookIssue issue)
    {
        using var connection = _context.CreateConnection();

        connection.Open();

        using var transaction = connection.BeginTransaction();

        try
        {
            const string insertSql = """
            INSERT INTO BookIssues
            (
                BookId,
                StudentId,
                IssueDate,
                DueDate,
                Status,
                Remarks
            )
            VALUES
            (
                @BookId,
                @StudentId,
                @IssueDate,
                @DueDate,
                @Status,
                @Remarks
            );

            SELECT CAST(SCOPE_IDENTITY() AS INT);
            """;

            int issueId = await connection.ExecuteScalarAsync<int>(
                insertSql,
                issue,
                transaction);

            const string updateBook = """
            UPDATE Books
            SET
                AvailableCopies = AvailableCopies - 1,
                ModifiedDate = GETDATE()
            WHERE Id=@BookId
            """;

            await connection.ExecuteAsync(
                updateBook,
                new { issue.BookId },
                transaction);

            transaction.Commit();

            return issueId;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    #endregion

    #region Return Book

    public async Task<bool> ReturnBookAsync(int issueId)
    {
        using var connection = _context.CreateConnection();

        connection.Open();

        using var transaction = connection.BeginTransaction();

        try
        {
            const string getBook = """
            SELECT BookId
            FROM BookIssues
            WHERE Id=@IssueId
            """;

            int bookId = await connection.ExecuteScalarAsync<int>(
                getBook,
                new { IssueId = issueId },
                transaction);

            const string updateIssue = """
            UPDATE BookIssues
            SET
                Status='Returned',
                ReturnDate=GETDATE(),
                ModifiedDate=GETDATE()
            WHERE Id=@IssueId
            """;

            await connection.ExecuteAsync(
                updateIssue,
                new { IssueId = issueId },
                transaction);

            const string updateBook = """
            UPDATE Books
            SET
                AvailableCopies = AvailableCopies + 1,
                ModifiedDate = GETDATE()
            WHERE Id=@BookId
            """;

            await connection.ExecuteAsync(
                updateBook,
                new { BookId = bookId },
                transaction);

            transaction.Commit();

            return true;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    #endregion

    public async Task<IEnumerable<BookIssue>> GetOverdueBooksAsync()
    {
        using var connection = _context.CreateConnection();

        const string sql =
        """
    SELECT
        bi.*,
        s.RegistrationNo,
        CONCAT(
            s.FirstName,
            ' ',
            ISNULL(s.MiddleName,''),
            ' ',
            s.LastName
        ) AS StudentName,
        b.BookCode,
        b.BookName
    FROM BookIssues bi

    INNER JOIN Students s
        ON s.Id = bi.StudentId

    INNER JOIN Books b
        ON b.Id = bi.BookId

    WHERE
        bi.Status = 'Issued'

        AND CAST(bi.DueDate AS DATE) < CAST(GETDATE() AS DATE)

        AND
        (
            bi.LastReminderSent IS NULL

            OR

            CAST(bi.LastReminderSent AS DATE)
                < CAST(GETDATE() AS DATE)
        )
    """;

        return await connection.QueryAsync<BookIssue>(sql);
    }

    public async Task UpdateLastReminderSentAsync(int issueId)
    {
        using var connection = _context.CreateConnection();

        const string sql =
        """
    UPDATE BookIssues
    SET LastReminderSent = GETDATE()
    WHERE Id = @issueId
    """;

        await connection.ExecuteAsync(sql, new { issueId });
    }
}