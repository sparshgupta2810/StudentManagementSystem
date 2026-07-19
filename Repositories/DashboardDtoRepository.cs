using Dapper;
using StudentManagementSystemApp.Data;
using StudentManagementSystemApp.Interfaces;
using StudentManagementSystemApp.Models;

namespace StudentManagementSystemApp.Repositories;

public class DashboardDtoRepository : IDashboardDtoRepository
{
    private readonly DapperContext _context;

    public DashboardDtoRepository(DapperContext context)
    {
        _context = context;
    }

    public async Task<DashboardDto> GetDashboardDataAsync()
    {
        using var connection = _context.CreateConnection();

        var dashboard = new DashboardDto();

        #region Summary Cards

        dashboard.TotalStudents =
            await connection.ExecuteScalarAsync<int>(
            """
            SELECT COUNT(*)
            FROM Students
            WHERE IsActive = 1
            """);

        dashboard.TotalTeachers =
            await connection.ExecuteScalarAsync<int>(
            """
            SELECT COUNT(*)
            FROM Teachers
            WHERE IsActive = 1
            """);

        dashboard.TotalDepartments =
            await connection.ExecuteScalarAsync<int>(
            """
            SELECT COUNT(*)
            FROM Departments
            WHERE IsActive = 1
            """);

        dashboard.TotalBooks =
            await connection.ExecuteScalarAsync<int>(
            """
            SELECT COUNT(*)
            FROM Books
            WHERE IsActive = 1
            """);

        #endregion

        #region Library Utilization

        int totalCopies =
            await connection.ExecuteScalarAsync<int>(
            """
            SELECT ISNULL(SUM(TotalCopies),0)
            FROM Books
            WHERE IsActive = 1
            """);

        int availableCopies =
            await connection.ExecuteScalarAsync<int>(
            """
            SELECT ISNULL(SUM(AvailableCopies),0)
            FROM Books
            WHERE IsActive = 1
            """);

        dashboard.TotalBooks = totalCopies;

        dashboard.AvailableBooks = availableCopies;

        dashboard.LibraryUtilization =
            totalCopies == 0
                ? 0
                : Math.Round(
                    ((double)(totalCopies - availableCopies) /
                    totalCopies) * 100,
                    2);

        #endregion

        #region Students By Department

        dashboard.DepartmentStudents =
        (
            await connection.QueryAsync<DepartmentStudentCount>(
            """
            SELECT
                d.DepartmentName AS Department,
                COUNT(s.Id) AS TotalStudents
            FROM Departments d

            LEFT JOIN Students s
                ON d.Id = s.DepartmentId
                AND s.IsActive = 1

            WHERE d.IsActive = 1

            GROUP BY d.DepartmentName

            ORDER BY TotalStudents DESC
            """)
        ).ToList();

        #endregion

        #region Yearly Admissions

        dashboard.YearlyAdmissions =
        (
            await connection.QueryAsync<YearlyAdmission>(
            """
            SELECT
                YEAR(AdmissionDate) AS Year,
                COUNT(*) AS TotalStudents
            FROM Students

            WHERE IsActive = 1

            GROUP BY YEAR(AdmissionDate)

            ORDER BY Year
            """)
        ).ToList();

        #endregion

        #region Monthly Book Issues

        dashboard.MonthlyBookIssues =
        (
            await connection.QueryAsync<MonthlyBookIssue>(
            """
            SELECT
                DATENAME(MONTH, IssueDate) AS Month,
                COUNT(*) AS TotalBooks
            FROM BookIssues

            GROUP BY
                MONTH(IssueDate),
                DATENAME(MONTH, IssueDate)

            ORDER BY MONTH(IssueDate)
            """)
        ).ToList();

        #endregion

        return dashboard;
    }
}