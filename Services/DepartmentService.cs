using StudentManagementSystemApp.Enums;
using StudentManagementSystemApp.Interfaces;
using StudentManagementSystemApp.Models;

namespace StudentManagementSystemApp.Services;

public class DepartmentService
{
    private readonly IDepartmentRepository _repository;
    private readonly IAuditService _auditService;

    public DepartmentService(
        IDepartmentRepository repository,
        IAuditService auditService)
    {
        _repository = repository;
        _auditService = auditService;
    }

    #region Get All

    public async Task<IEnumerable<Department>> GetAllDepartmentsAsync()
    {
        return await _repository.GetActiveAsync();
    }

    #endregion

    #region Pagination

    public async Task<PagedResult<Department>> GetPagedDepartmentsAsync(
        int page,
        int pageSize,
        string? search = null)
    {
        return await _repository.GetPagedAsync(
            page,
            pageSize,
            search);
    }

    public async Task<PagedResult<Department>> GetPagedDepartmentHistoryAsync(
        int page,
        int pageSize,
        string? search = null)
    {
        return await _repository.GetPagedHistoryAsync(
            page,
            pageSize,
            search);
    }

    #endregion

    #region History

    public async Task<IEnumerable<Department>> GetDepartmentHistoryAsync()
    {
        return await _repository.GetInactiveAsync();
    }

    #endregion

    #region Get By Id

    public async Task<Department?> GetDepartmentByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    #endregion

    #region Add

    public async Task<int> AddDepartmentAsync(Department department)
    {
        if (await _repository.DepartmentCodeExistsAsync(department.DepartmentCode))
            throw new Exception("Department Code already exists.");

        if (await _repository.DepartmentNameExistsAsync(department.DepartmentName))
            throw new Exception("Department Name already exists.");

        int id = await _repository.AddAsync(department);

        if (id > 0)
        {
            department.Id = id;

            await _auditService.LogAsync(
                module: AuditModule.Department,
                action: AuditAction.Add,
                entityId: id,
                entityName: department.DepartmentName,
                details: $"Department '{department.DepartmentName}' created.",
                oldValue: null,
                newValue: department);
        }

        return id;
    }

    #endregion

    #region Update

    public async Task<bool> UpdateDepartmentAsync(Department department)
    {
        if (await _repository.DepartmentCodeExistsAsync(
            department.DepartmentCode,
            department.Id))
            throw new Exception("Department Code already exists.");

        if (await _repository.DepartmentNameExistsAsync(
            department.DepartmentName,
            department.Id))
            throw new Exception("Department Name already exists.");

        var oldDepartment =
            await _repository.GetByIdAsync(department.Id);

        bool result =
            await _repository.UpdateAsync(department);

        if (result)
        {
            await _auditService.LogAsync(
                module: AuditModule.Department,
                action: AuditAction.Update,
                entityId: department.Id,
                entityName: department.DepartmentName,
                details: $"Department '{department.DepartmentName}' updated.",
                oldValue: oldDepartment,
                newValue: department);
        }

        return result;
    }

    #endregion

    #region Inactivate

    public async Task<bool> InactivateDepartmentAsync(int id)
    {
        var department =
            await _repository.GetByIdAsync(id);

        bool result =
            await _repository.InactivateAsync(id);

        if (result && department != null)
        {
            await _auditService.LogAsync(
                module: AuditModule.Department,
                action: AuditAction.Delete,
                entityId: id,
                entityName: department.DepartmentName,
                details: $"Department '{department.DepartmentName}' removed.",
                oldValue: department,
                newValue: null);
        }

        return result;
    }

    #endregion

    #region Restore

    public async Task<bool> RestoreDepartmentAsync(int id)
    {
        var department =
            await _repository.GetByIdAsync(id);

        bool result =
            await _repository.RestoreAsync(id);

        if (result && department != null)
        {
            await _auditService.LogAsync(
                module: AuditModule.Department,
                action: AuditAction.Restore,
                entityId: id,
                entityName: department.DepartmentName,
                details: $"Department '{department.DepartmentName}' restored.",
                oldValue: null,
                newValue: department);
        }

        return result;
    }

    #endregion
}