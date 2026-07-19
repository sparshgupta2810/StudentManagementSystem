using System.ComponentModel.DataAnnotations;

namespace StudentManagementSystemApp.Models;

public class Book : BaseEntity
{
    [Required]
    [StringLength(20)]
    public string BookCode { get; set; } = string.Empty;

    [Required]
    [StringLength(200)]
    public string BookName { get; set; } = string.Empty;

    [Required]
    [StringLength(150)]
    public string Author { get; set; } = string.Empty;

    [Range(1, 100000)]
    public int TotalCopies { get; set; }

    public int AvailableCopies { get; set; }
}