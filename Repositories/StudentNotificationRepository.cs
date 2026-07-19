using Dapper;
using StudentManagementSystemApp.Data;
using StudentManagementSystemApp.Interfaces;

namespace StudentManagementSystemApp.Repositories;

public class StudentNotificationRepository
    : IStudentNotificationRepository
{
    private readonly DapperContext _context;

    public StudentNotificationRepository(
        DapperContext context)
    {
        _context = context;
    }

    public async Task<string?> GetStudentEmailAsync(
        int studentId)
    {
        using var connection =
            _context.CreateConnection();

        return await connection.ExecuteScalarAsync<string>(
        """
        SELECT Email

        FROM Students

        WHERE Id = @studentId
        """,
        new { studentId });
    }
}