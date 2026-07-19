using StudentManagementSystemApp.Models;

namespace StudentManagementSystemApp.Interfaces;

public interface IDashboardDtoRepository
{
    Task<DashboardDto> GetDashboardDataAsync();
}