using StudentManagementSystemApp.Interfaces;
using StudentManagementSystemApp.Models;

namespace StudentManagementSystemApp.Services;

public class BookIssueService
{
    private readonly IBookIssueRepository _repository;
    private readonly IAuditService _auditService;
    private readonly INotificationService _notificationService;

    public BookIssueService(
        IBookIssueRepository repository,
        IAuditService auditService,
        INotificationService notificationService)
    {
        _repository = repository;
        _auditService = auditService;
        _notificationService = notificationService;
    }

    #region Get

    public async Task<IEnumerable<BookIssue>> GetIssuedBooksAsync()
    {
        return await _repository.GetIssuedBooksAsync();
    }

    public async Task<IEnumerable<BookIssue>> GetHistoryAsync()
    {
        return await _repository.GetHistoryAsync();
    }

    #region Pagination

    public async Task<PagedResult<BookIssue>> GetPagedIssuedBooksAsync(
        int page,
        int pageSize,
        string? search = null)
    {
        return await _repository.GetPagedIssuedAsync(
            page,
            pageSize,
            search);
    }

    #endregion

    #region History Pagination

    public async Task<PagedResult<BookIssue>> GetPagedHistoryAsync(
        int page,
        int pageSize,
        string? search = null)
    {
        return await _repository.GetPagedHistoryAsync(
            page,
            pageSize,
            search);
    }

    #endregion

    public async Task<BookIssue?> GetBookIssueByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    #endregion

    #region Student Search

    public async Task<Student?> GetStudentByRegistrationNoAsync(string registrationNo)
    {
        if (string.IsNullOrWhiteSpace(registrationNo))
            throw new Exception("Please enter Registration Number.");

        var student =
            await _repository.GetStudentByRegistrationNoAsync(registrationNo);

        if (student == null)
            throw new Exception("Student not found.");

        if (!student.IsActive)
            throw new Exception("Student is inactive.");

        return student;
    }

    #endregion

    #region Available Books

    public async Task<IEnumerable<Book>> GetAvailableBooksAsync()
    {
        return await _repository.GetAvailableBooksAsync();
    }

    #endregion

    #region Issue Book

    public async Task<int> IssueBookAsync(BookIssue issue)
    {
        if (issue.StudentId <= 0)
            throw new Exception("Please select a student.");

        if (issue.BookId <= 0)
            throw new Exception("Please select a book.");

        int availableCopies =
            await _repository.GetAvailableCopiesAsync(issue.BookId);

        if (availableCopies <= 0)
            throw new Exception("Selected book is currently unavailable.");

        bool alreadyIssued =
            await _repository.IsBookAlreadyIssuedAsync(
                issue.StudentId,
                issue.BookId);

        if (alreadyIssued)
            throw new Exception("This student already has this book issued.");

        if (issue.IssueDate == default)
            issue.IssueDate = DateTime.Today;

        if (issue.DueDate == default)
            issue.DueDate = DateTime.Today.AddDays(15);

        issue.Status = "Issued";

        int id =
            await _repository.IssueBookAsync(issue);

        if (id > 0)
        {
            issue.Id = id;

            // Reload complete record
            var issuedBook =
                await _repository.GetByIdAsync(id);

            await _auditService.LogAsync(
                module: AuditModule.BookIssue,
                action: AuditAction.Add,
                entityId: id,
                entityName: $"{issuedBook?.StudentName} - {issuedBook?.BookName}",
                details: $"Book '{issuedBook?.BookName}' issued to '{issuedBook?.StudentName}'.",
                oldValue: null,
                newValue: issuedBook);

            // Send confirmation email
            if (issuedBook != null)
            {
                await _notificationService.SendBookIssuedAsync(
                    issuedBook);
            }
        }

        return id;
    }

    #endregion

    #region Return Book

    public async Task<bool> ReturnBookAsync(int issueId)
    {
        if (issueId <= 0)
            throw new Exception("Invalid Book Issue.");

        var oldIssue =
            await _repository.GetByIdAsync(issueId);

        bool result =
            await _repository.ReturnBookAsync(issueId);

        if (result)
        {
            var newIssue =
                await _repository.GetByIdAsync(issueId);

            await _auditService.LogAsync(
                module: AuditModule.BookIssue,
                action: AuditAction.Update,
                entityId: issueId,
                entityName: $"{newIssue?.StudentName} - {newIssue?.BookName}",
                details: $"Book '{newIssue?.BookName}' returned by '{newIssue?.StudentName}'.",
                oldValue: oldIssue,
                newValue: newIssue);

            // Send return confirmation email
            if (newIssue != null)
            {
                await _notificationService.SendBookReturnedAsync(
                    newIssue);
            }
        }

        return result;
    }

    #endregion
}