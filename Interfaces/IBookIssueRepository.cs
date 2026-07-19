using StudentManagementSystemApp.Models;

namespace StudentManagementSystemApp.Interfaces;

public interface IBookIssueRepository
{
    // Current Issued Books
    Task<IEnumerable<BookIssue>> GetIssuedBooksAsync();

    // History (Returned/Lost/Damaged)
    Task<IEnumerable<BookIssue>> GetHistoryAsync();

    Task<PagedResult<BookIssue>> GetPagedIssuedAsync(
    int page,
    int pageSize,
    string? search = null);

    Task<PagedResult<BookIssue>> GetPagedHistoryAsync(
        int page,
        int pageSize,
        string? search = null);

    // Get Single Issue
    Task<BookIssue?> GetByIdAsync(int id);

    // Search Student by Registration No
    Task<Student?> GetStudentByRegistrationNoAsync(string registrationNo);

    // Get Books Available for Issue
    Task<IEnumerable<Book>> GetAvailableBooksAsync();

    // Issue Book
    Task<int> IssueBookAsync(BookIssue issue);

    // Return Book
    Task<bool> ReturnBookAsync(int issueId);

    // Check Student already has same book
    Task<bool> IsBookAlreadyIssuedAsync(int studentId, int bookId);

    // Check Available Copies
    Task<int> GetAvailableCopiesAsync(int bookId);

    Task<IEnumerable<BookIssue>> GetOverdueBooksAsync();

    Task UpdateLastReminderSentAsync(int issueId);
}