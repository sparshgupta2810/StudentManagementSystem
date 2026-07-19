using StudentManagementSystemApp.Interfaces;
using StudentManagementSystemApp.Models;
using StudentManagementSystemApp.Models.Notification;

namespace StudentManagementSystemApp.Services;

public class NotificationService : INotificationService
{
    private readonly IEmailService _emailService;
    private readonly INotificationRepository _notificationRepository;
    private readonly IStudentRepository _studentRepository;

    public NotificationService(
        IEmailService emailService,
        INotificationRepository notificationRepository,
        IStudentRepository studentRepository)
    {
        _emailService = emailService;
        _notificationRepository = notificationRepository;
        _studentRepository = studentRepository;
    }

    public async Task SendBookIssuedAsync(BookIssue issue)
    {
        string subject = "Book Issued Successfully";

        string body =
        $"""
        <h2>📚 Student Management System</h2>

        <p>Hello <strong>{issue.StudentName}</strong>,</p>

        <p>Your book has been issued successfully.</p>

        <table style="border-collapse:collapse;" border="1" cellpadding="8">

            <tr>
                <td><strong>Book</strong></td>
                <td>{issue.BookName}</td>
            </tr>

            <tr>
                <td><strong>Issue Date</strong></td>
                <td>{issue.IssueDate:dd MMM yyyy}</td>
            </tr>

            <tr>
                <td><strong>Due Date</strong></td>
                <td>{issue.DueDate:dd MMM yyyy}</td>
            </tr>

        </table>

        <br/>

        Please return the book before the due date.

        <br/><br/>

        Regards,<br/>
        Library Team
        """;

        await SendAsync(
            issue,
            subject,
            body,
            NotificationType.BookIssued);
    }

    public async Task SendBookReturnedAsync(BookIssue issue)
    {
        string subject = "Book Returned Successfully";

        string body =
        $"""
        <h2>📚 Student Management System</h2>

        <p>Hello <strong>{issue.StudentName}</strong>,</p>

        <p>Thank you for returning your book.</p>

        <table style="border-collapse:collapse;" border="1" cellpadding="8">

            <tr>
                <td><strong>Book</strong></td>
                <td>{issue.BookName}</td>
            </tr>

            <tr>
                <td><strong>Return Date</strong></td>
                <td>{DateTime.Now:dd MMM yyyy}</td>
            </tr>

        </table>

        <br/>

        We appreciate your cooperation.

        <br/><br/>

        Regards,<br/>
        Library Team
        """;

        await SendAsync(
            issue,
            subject,
            body,
            NotificationType.BookReturned);
    }

    public async Task SendOverdueReminderAsync(BookIssue issue)
    {
        int overdueDays =
            (DateTime.Today - issue.DueDate.Date).Days;

        int fine =
            overdueDays * 10;

        string subject =
            "⚠ Book Overdue Reminder";

        string body =
        $"""
        <h2>📚 Student Management System</h2>

        <p>Hello <strong>{issue.StudentName}</strong>,</p>

        <p>Your borrowed book is overdue.</p>

        <table style="border-collapse:collapse;" border="1" cellpadding="8">

            <tr>
                <td><strong>Book</strong></td>
                <td>{issue.BookName}</td>
            </tr>

            <tr>
                <td><strong>Due Date</strong></td>
                <td>{issue.DueDate:dd MMM yyyy}</td>
            </tr>

            <tr>
                <td><strong>Days Overdue</strong></td>
                <td>{overdueDays}</td>
            </tr>

            <tr>
                <td><strong>Fine</strong></td>
                <td>₹{fine}</td>
            </tr>

        </table>

        <br/>

        <p style="color:red;">
            Please return the book immediately to avoid additional fines.
        </p>

        <br/>

        Regards,<br/>
        Library Team
        """;

        await SendAsync(
            issue,
            subject,
            body,
            NotificationType.BookOverdue);
    }

    private async Task SendAsync(
        BookIssue issue,
        string subject,
        string body,
        string notificationType)
    {
        var email =
            await _studentRepository.GetEmailByIdAsync(issue.StudentId);

        if (string.IsNullOrWhiteSpace(email))
            return;

        var log = new NotificationLog
        {
            StudentId = issue.StudentId,
            StudentName = issue.StudentName,
            Email = email,
            Subject = subject,
            Body = body,
            NotificationType = notificationType,
            SentDate = DateTime.Now
        };

        try
        {
            await _emailService.SendEmailAsync(
                new EmailMessage
                {
                    To = email,
                    Subject = subject,
                    Body = body,
                    IsHtml = true
                });

            log.Status = NotificationStatus.Success;
        }
        catch (Exception ex)
        {
            log.Status = NotificationStatus.Failed;
            log.ErrorMessage = ex.Message;
        }

        await _notificationRepository.AddLogAsync(log);
    }

    public async Task<PagedResult<NotificationLog>>
    GetPagedLogsAsync(
        int page,
        int pageSize,
        string? search = null)
    {
        return await _notificationRepository.GetPagedAsync(
            page,
            pageSize,
            search);
    }
}