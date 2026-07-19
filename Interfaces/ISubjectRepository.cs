using StudentManagementSystemApp.Models;

namespace StudentManagementSystemApp.Interfaces;

public interface ISubjectRepository
{
    Task<IEnumerable<Subject>> GetActiveAsync();

    Task<IEnumerable<Subject>> GetInactiveAsync();

    Task<PagedResult<Subject>> GetPagedAsync(
    int page,
    int pageSize,
    string? search = null);

    Task<PagedResult<Subject>> GetPagedHistoryAsync(
        int page,
        int pageSize,
        string? search = null);

    Task<Subject?> GetByIdAsync(int id);

    Task<int> AddAsync(Subject subject);

    Task<bool> UpdateAsync(Subject subject);

    Task<bool> InactivateAsync(int id);

    Task<bool> RestoreAsync(int id);

    Task<bool> SubjectCodeExistsAsync(string code, int? excludeId = null);

    Task<bool> SubjectNameExistsAsync(string name, int? excludeId = null);
}