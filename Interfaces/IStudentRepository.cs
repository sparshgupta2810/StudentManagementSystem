using StudentManagementSystemApp.Models;

namespace StudentManagementSystemApp.Interfaces;

public interface IStudentRepository
{
    Task<IEnumerable<Student>> GetActiveAsync();

    Task<PagedResult<Student>> GetPagedAsync(
    int page,
    int pageSize,
    string? search = null);

    Task<PagedResult<Student>> GetHistoryPagedAsync(
    int page,
    int pageSize,
    string? search = null);

    Task<IEnumerable<Student>> GetInactiveAsync();

    Task<Student?> GetByIdAsync(int id);

    Task<int> AddAsync(Student student);

    Task<bool> UpdateAsync(Student student);

    Task<bool> InactivateAsync(int id);

    Task<bool> RestoreAsync(int id);

    Task<bool> RegistrationExistsAsync(string registrationNo, int? excludeId = null);

    Task<bool> EmailExistsAsync(string email, int? excludeId = null);

    Task<string?> GetEmailByIdAsync(int id);
}