using Microsoft.AspNetCore.Identity;

namespace StudentManagementSystemApp.Models;

public class UserManagementViewModel
{
    public string Id { get; set; } = string.Empty;

    public string UserName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public string CurrentRole { get; set; } = string.Empty;

    public bool IsAdmin =>
        CurrentRole == "Admin";
}