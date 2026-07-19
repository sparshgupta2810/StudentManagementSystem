namespace StudentManagementSystemApp.Interfaces;

public interface IStudentNotificationRepository
{
    Task<string?> GetStudentEmailAsync(int studentId);
}