using StudentManagementSystemApp.Models;

namespace StudentManagementSystemApp.Interfaces;

public interface IUserManagementRepository
{
    Task<IEnumerable<UserManagementViewModel>> GetUsersAsync();

    Task<PagedResult<UserManagementViewModel>> GetPagedUsersAsync(
    int page,
    int pageSize,
    string? search = null);

    Task<UserManagementViewModel?> GetUserByIdAsync(string id);

    Task<bool> UpdateRoleAsync(string userId, string newRole);
}