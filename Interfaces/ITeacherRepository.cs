using StudentManagementSystemApp.Models;

namespace StudentManagementSystemApp.Interfaces;

public interface ITeacherRepository
{
    // Get

    Task<IEnumerable<Teacher>> GetActiveAsync();

    Task<PagedResult<Teacher>> GetPagedAsync(
    int page,
    int pageSize,
    string? search = null);

    Task<PagedResult<Teacher>> GetHistoryPagedAsync(
        int page,
        int pageSize,
        string? search = null);

    Task<IEnumerable<Teacher>> GetInactiveAsync();

    Task<Teacher?> GetByIdAsync(int id);

    // Add

    Task<int> AddAsync(Teacher teacher);

    // Update

    Task<bool> UpdateAsync(Teacher teacher);

    // Inactivate

    Task<bool> InactivateAsync(int id);

    // Restore

    Task<bool> RestoreAsync(int id);

    // Validation

    Task<bool> EmailExistsAsync(string email, int? excludeId = null);

    Task<bool> PhoneExistsAsync(string phoneNumber, int? excludeId = null);
}