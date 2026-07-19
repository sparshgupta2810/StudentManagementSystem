using StudentManagementSystemApp.Interfaces;
using StudentManagementSystemApp.Models;

namespace StudentManagementSystemApp.Services;

public class DashboardDtoService
{
    private readonly IDashboardDtoRepository _repository;

    public DashboardDtoService(IDashboardDtoRepository repository)
    {
        _repository = repository;
    }

    public async Task<DashboardDto> GetDashboardAsync()
    {
        return await _repository.GetDashboardDataAsync();
    }
}