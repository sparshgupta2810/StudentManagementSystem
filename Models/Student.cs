using System.ComponentModel.DataAnnotations;

namespace StudentManagementSystemApp.Models;

public class Student : BaseEntity
{
    [Required]
    [StringLength(20)]
    public string RegistrationNo { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [StringLength(100)]
    public string? MiddleName { get; set; }

    [Required]
    [StringLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(15)]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required]
    public string Gender { get; set; } = string.Empty;

    [Required]
    public DateTime DateOfBirth { get; set; }

    public string? Address { get; set; }

    [Required]
    public int DepartmentId { get; set; }

    // Used only for display after JOIN
    public string? DepartmentName { get; set; }

    [Required]
    public DateTime AdmissionDate { get; set; }
}