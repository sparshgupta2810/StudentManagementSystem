namespace StudentManagementSystemApp.Models;

public class DashboardDto
{
    public int TotalStudents { get; set; }

    public int TotalTeachers { get; set; }

    public int TotalDepartments { get; set; }

    public int TotalBooks { get; set; }

    public int AvailableBooks { get; set; }

    public double LibraryUtilization { get; set; }

    public List<DepartmentStudentCount> DepartmentStudents { get; set; }
        = new();

    public List<YearlyAdmission> YearlyAdmissions { get; set; }
    = new();

    public List<MonthlyBookIssue> MonthlyBookIssues { get; set; }
        = new();

}