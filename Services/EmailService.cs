using Microsoft.Extensions.Options;
using StudentManagementSystemApp.Interfaces;
using StudentManagementSystemApp.Models;
using System.Net;
using System.Net.Mail;

namespace StudentManagementSystemApp.Services;

public class EmailService : IEmailService
{
    private readonly EmailSettings _settings;

    public EmailService(
        IOptions<EmailSettings> settings)
    {
        _settings = settings.Value;
    }

    public async Task SendEmailAsync(
        EmailMessage message)
    {
        using var smtpClient = new SmtpClient(
            _settings.SmtpServer,
            _settings.Port);

        smtpClient.EnableSsl =
            _settings.EnableSsl;

        smtpClient.Credentials =
            new NetworkCredential(
                _settings.Username,
                _settings.Password);

        using var mail = new MailMessage();

        mail.From =
            new MailAddress(
                _settings.SenderEmail,
                _settings.SenderName);

        mail.To.Add(message.To);

        mail.Subject = message.Subject;

        mail.Body = message.Body;

        mail.IsBodyHtml = message.IsHtml;

        await smtpClient.SendMailAsync(mail);
    }
}