using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StudentManagementSystemApp.Data;
using StudentManagementSystemApp.Interfaces;
using StudentManagementSystemApp.Models;

namespace StudentManagementSystemApp.Repositories;

public class UserManagementRepository : IUserManagementRepository
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UserManagementRepository(
        UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IEnumerable<UserManagementViewModel>> GetUsersAsync()
    {
        var users = await _userManager.Users.ToListAsync();

        var list = new List<UserManagementViewModel>();

        foreach (var user in users)
        {
            var roles =
                await _userManager.GetRolesAsync(user);

            list.Add(new UserManagementViewModel
            {
                Id = user.Id,
                UserName = user.UserName ?? "",
                Email = user.Email ?? "",
                FullName = user.FullName ?? "",
                CurrentRole = roles.FirstOrDefault() ?? "User"
            });
        }

        return list.OrderBy(x => x.UserName);
    }

    public async Task<PagedResult<UserManagementViewModel>> GetPagedUsersAsync(
    int page,
    int pageSize,
    string? search = null)
    {
        var users = await _userManager.Users.ToListAsync();

        var list = new List<UserManagementViewModel>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);

            list.Add(new UserManagementViewModel
            {
                Id = user.Id,
                UserName = user.UserName ?? "",
                Email = user.Email ?? "",
                FullName = user.FullName ?? "",
                CurrentRole = roles.FirstOrDefault() ?? "User",
            });
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            list = list.Where(x =>
                    x.UserName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    x.Email.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    x.FullName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    x.CurrentRole.Contains(search, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        int totalRecords = list.Count;

        var items = list
            .OrderBy(x => x.UserName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return new PagedResult<UserManagementViewModel>
        {
            Items = items,
            CurrentPage = page,
            PageSize = pageSize,
            TotalRecords = totalRecords
        };
    }

    public async Task<UserManagementViewModel?> GetUserByIdAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);

        if (user == null)
            return null;

        var roles = await _userManager.GetRolesAsync(user);

        return new UserManagementViewModel
        {
            Id = user.Id,
            UserName = user.UserName ?? "",
            Email = user.Email ?? "",
            FullName = user.FullName ?? "",
            CurrentRole = roles.FirstOrDefault() ?? "User"
        };
    }

    public async Task<bool> UpdateRoleAsync(string userId, string newRole)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
            return false;

        var currentRoles = await _userManager.GetRolesAsync(user);

        // Protect Admin accounts
        if (currentRoles.Contains("Admin"))
            return false;

        if (currentRoles.Any())
        {
            var removeResult =
                await _userManager.RemoveFromRolesAsync(user, currentRoles);

            if (!removeResult.Succeeded)
                return false;
        }

        var addResult =
            await _userManager.AddToRoleAsync(user, newRole);

        return addResult.Succeeded;
    }
}