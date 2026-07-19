namespace StudentManagementSystemApp.Models;

public class DashboardCard
{
    public string Title { get; set; } = string.Empty;

    public int Count { get; set; }

    public string Icon { get; set; } = string.Empty;

    public string Color { get; set; } = "primary";

    public string Url { get; set; } = "#";
}