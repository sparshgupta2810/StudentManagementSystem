using StudentManagementSystemApp.Models;

public interface IDepartmentRepository
{
    Task<IEnumerable<Department>> GetActiveAsync();
    Task<PagedResult<Department>> GetPagedAsync(
    int page,
    int pageSize,
    string? search = null);

    Task<PagedResult<Department>> GetPagedHistoryAsync(
    int page,
    int pageSize,
    string? search = null);

    Task<IEnumerable<Department>> GetInactiveAsync();

    Task<Department?> GetByIdAsync(int id);

    Task<int> AddAsync(Department department);

    Task<bool> UpdateAsync(Department department);

    Task<bool> InactivateAsync(int id);

    Task<bool> RestoreAsync(int id);

    Task<bool> DepartmentCodeExistsAsync(string code, int? excludeId = null);

    Task<bool> DepartmentNameExistsAsync(string name, int? excludeId = null);
}