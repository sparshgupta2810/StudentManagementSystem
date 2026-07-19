using StudentManagementSystemApp.Models;

namespace StudentManagementSystemApp.Interfaces;

public interface IEmailService
{
    Task SendEmailAsync(EmailMessage message);
}