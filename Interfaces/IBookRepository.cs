using StudentManagementSystemApp.Models;

namespace StudentManagementSystemApp.Interfaces;

public interface IBookRepository
{
    Task<IEnumerable<Book>> GetActiveAsync();

    Task<IEnumerable<Book>> GetInactiveAsync();

    Task<PagedResult<Book>> GetPagedAsync(
    int page,
    int pageSize,
    string? search = null);

    Task<PagedResult<Book>> GetHistoryPagedAsync(
        int page,
        int pageSize,
        string? search = null);

    Task<Book?> GetByIdAsync(int id);

    Task<int> AddAsync(Book book);

    Task<bool> UpdateAsync(Book book);

    Task<bool> InactivateAsync(int id);

    Task<bool> RestoreAsync(int id);

    Task<bool> BookCodeExistsAsync(string code, int? excludeId = null);

    Task<bool> BookNameExistsAsync(string name, int? excludeId = null);
}