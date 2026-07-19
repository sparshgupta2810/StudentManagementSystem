using StudentManagementSystemApp.Models;

namespace StudentManagementSystemApp.Interfaces;

public interface ICurrentUserService
{
    Task<CurrentUser> GetCurrentUserAsync();
}