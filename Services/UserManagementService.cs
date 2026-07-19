using StudentManagementSystemApp.Enums;
using StudentManagementSystemApp.Interfaces;
using StudentManagementSystemApp.Models;

namespace StudentManagementSystemApp.Services;

public class UserManagementService
{
    private readonly IUserManagementRepository _repository;
    private readonly IAuditService _auditService;

    public UserManagementService(
        IUserManagementRepository repository,
        IAuditService auditService)
    {
        _repository = repository;
        _auditService = auditService;
    }

    #region Get

    public async Task<IEnumerable<UserManagementViewModel>> GetUsersAsync()
    {
        return await _repository.GetUsersAsync();
    }

    #endregion

    #region Pagination

    public async Task<PagedResult<UserManagementViewModel>> GetPagedUsersAsync(
        int page,
        int pageSize,
        string? search = null)
    {
        return await _repository.GetPagedUsersAsync(
            page,
            pageSize,
            search);
    }

    #endregion

    #region Get By Id

    public async Task<UserManagementViewModel?> GetUserByIdAsync(string id)
    {
        return await _repository.GetUserByIdAsync(id);
    }

    #endregion

    #region Update Role

    public async Task<bool> UpdateRoleAsync(
        string userId,
        string newRole)
    {
        var oldUser =
            await _repository.GetUserByIdAsync(userId);

        bool result =
            await _repository.UpdateRoleAsync(
                userId,
                newRole);

        if (result)
        {
            var newUser =
                await _repository.GetUserByIdAsync(userId);

            if (oldUser != null && newUser != null)
            {
                await _auditService.LogAsync(
                    module: AuditModule.UserManagement,
                    action: AuditAction.Update,
                    entityId: userId,
                    entityName: newUser.FullName,
                    details: $"Role updated for '{newUser.FullName}'.",
                    oldValue: oldUser,
                    newValue: newUser);
            }
        }

        return result;
    }

    #endregion
}