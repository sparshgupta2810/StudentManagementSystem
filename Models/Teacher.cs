using System.ComponentModel.DataAnnotations;

namespace StudentManagementSystemApp.Models;

public class Teacher : BaseEntity
{
    [Required]
    public string FirstName { get; set; } = string.Empty;

    public string? MiddleName { get; set; }

    [Required]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required]
    public string Gender { get; set; } = string.Empty;

    public DateTime? DateOfBirth { get; set; }

    public string? Address { get; set; }

    [Required]
    public int DepartmentId { get; set; }

    // For JOIN display
    public string? DepartmentName { get; set; }

    [Required]
    public DateTime JoiningDate { get; set; }

    public string? Qualification { get; set; }

    public int? Experience { get; set; }
}