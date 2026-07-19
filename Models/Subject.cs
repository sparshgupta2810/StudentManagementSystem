using System.ComponentModel.DataAnnotations;

namespace StudentManagementSystemApp.Models;

public class Subject : BaseEntity
{
    [Required(ErrorMessage = "Subject Code is required.")]
    [StringLength(20)]
    public string SubjectCode { get; set; } = string.Empty;

    [Required(ErrorMessage = "Subject Name is required.")]
    [StringLength(150)]
    public string SubjectName { get; set; } = string.Empty;
}