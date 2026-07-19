using System.ComponentModel.DataAnnotations;

namespace StudentManagementSystemApp.Models;

public class Department : BaseEntity
{
    [Required(ErrorMessage = "Department Code is required.")]
    [StringLength(20)]
    public string DepartmentCode { get; set; } = string.Empty;

    [Required(ErrorMessage = "Department Name is required.")]
    [StringLength(150)]
    public string DepartmentName { get; set; } = string.Empty;

    [StringLength(300)]
    public string? Description { get; set; }
}