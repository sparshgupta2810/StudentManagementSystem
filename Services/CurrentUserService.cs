using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using StudentManagementSystemApp.Data;
using StudentManagementSystemApp.Interfaces;
using StudentManagementSystemApp.Models;

namespace StudentManagementSystemApp.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly UserManager<ApplicationUser> _userManager;

    public CurrentUserService(
        IHttpContextAccessor httpContextAccessor,
        UserManager<ApplicationUser> userManager)
    {
        _httpContextAccessor = httpContextAccessor;
        _userManager = userManager;
    }

    public async Task<CurrentUser> GetCurrentUserAsync()
    {
        var principal = _httpContextAccessor.HttpContext?.User;

        if (principal?.Identity?.IsAuthenticated != true)
        {
            return new CurrentUser();
        }

        var user = await _userManager.GetUserAsync(principal);

        if (user == null)
        {
            return new CurrentUser();
        }

        var roles = await _userManager.GetRolesAsync(user);

        return new CurrentUser
        {
            IsAuthenticated = true,

            UserId = user.Id,

            UserName = user.UserName ?? "",

            FullName = user.FullName ?? "",

            Email = user.Email ?? "",

            Role = roles.FirstOrDefault() ?? "User"
        };
    }
}