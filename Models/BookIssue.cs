using System.ComponentModel.DataAnnotations;

using StudentManagementSystemApp.Models;

public class BookIssue : BaseEntity
{
    [Required]
    public int BookId { get; set; }

    [Required]
    public int StudentId { get; set; }

    public string? RegistrationNo { get; set; }

    public string? StudentName { get; set; }

    public string? BookCode { get; set; }

    public string? BookName { get; set; }

    public DateTime IssueDate { get; set; }

    public DateTime DueDate { get; set; }

    public DateTime? ReturnDate { get; set; }

    public string Status { get; set; } = "Issued";

    public string? Remarks { get; set; }
}