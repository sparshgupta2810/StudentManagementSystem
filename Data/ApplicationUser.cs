using Microsoft.AspNetCore.Identity;

namespace StudentManagementSystemApp.Data;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
}