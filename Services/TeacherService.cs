using StudentManagementSystemApp.Enums;
using StudentManagementSystemApp.Interfaces;
using StudentManagementSystemApp.Models;

namespace StudentManagementSystemApp.Services;

public class TeacherService
{
    private readonly ITeacherRepository _repository;
    private readonly IAuditService _auditService;

    public TeacherService(
        ITeacherRepository repository,
        IAuditService auditService)
    {
        _repository = repository;
        _auditService = auditService;
    }

    #region Get

    public async Task<IEnumerable<Teacher>> GetAllTeachersAsync()
    {
        return await _repository.GetActiveAsync();
    }

    public async Task<PagedResult<Teacher>> GetPagedTeachersAsync(
        int page,
        int pageSize,
        string? search = null)
    {
        return await _repository.GetPagedAsync(
            page,
            pageSize,
            search);
    }

    public async Task<PagedResult<Teacher>> GetPagedTeacherHistoryAsync(
        int page,
        int pageSize,
        string? search = null)
    {
        return await _repository.GetHistoryPagedAsync(
            page,
            pageSize,
            search);
    }

    public async Task<IEnumerable<Teacher>> GetTeacherHistoryAsync()
    {
        return await _repository.GetInactiveAsync();
    }

    public async Task<Teacher?> GetTeacherByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    #endregion

    #region Add

    public async Task<int> AddTeacherAsync(Teacher teacher)
    {
        if (await _repository.EmailExistsAsync(teacher.Email))
            throw new Exception("Email already exists.");

        if (await _repository.PhoneExistsAsync(teacher.PhoneNumber))
            throw new Exception("Phone Number already exists.");

        int id = await _repository.AddAsync(teacher);

        if (id > 0)
        {
            teacher.Id = id;

            await _auditService.LogAsync(
                module: AuditModule.Teacher,
                action: AuditAction.Add,
                entityId: id,
                entityName: $"{teacher.FirstName} {teacher.LastName}",
                details: $"Teacher '{teacher.FirstName} {teacher.LastName}' created.",
                oldValue: null,
                newValue: teacher);
        }

        return id;
    }

    #endregion

    #region Update

    public async Task<bool> UpdateTeacherAsync(Teacher teacher)
    {
        if (await _repository.EmailExistsAsync(
            teacher.Email,
            teacher.Id))
            throw new Exception("Email already exists.");

        if (await _repository.PhoneExistsAsync(
            teacher.PhoneNumber,
            teacher.Id))
            throw new Exception("Phone Number already exists.");

        var oldTeacher =
            await _repository.GetByIdAsync(teacher.Id);

        bool result =
            await _repository.UpdateAsync(teacher);

        if (result)
        {
            await _auditService.LogAsync(
                module: AuditModule.Teacher,
                action: AuditAction.Update,
                entityId: teacher.Id,
                entityName: $"{teacher.FirstName} {teacher.LastName}",
                details: $"Teacher '{teacher.FirstName} {teacher.LastName}' updated.",
                oldValue: oldTeacher,
                newValue: teacher);
        }

        return result;
    }

    #endregion

    #region Inactivate

    public async Task<bool> InactivateTeacherAsync(int id)
    {
        var teacher =
            await _repository.GetByIdAsync(id);

        bool result =
            await _repository.InactivateAsync(id);

        if (result && teacher != null)
        {
            await _auditService.LogAsync(
                module: AuditModule.Teacher,
                action: AuditAction.Delete,
                entityId: id,
                entityName: $"{teacher.FirstName} {teacher.LastName}",
                details: $"Teacher '{teacher.FirstName} {teacher.LastName}' removed.",
                oldValue: teacher,
                newValue: null);
        }

        return result;
    }

    #endregion

    #region Restore

    public async Task<bool> RestoreTeacherAsync(int id)
    {
        var teacher =
            await _repository.GetByIdAsync(id);

        bool result =
            await _repository.RestoreAsync(id);

        if (result && teacher != null)
        {
            await _auditService.LogAsync(
                module: AuditModule.Teacher,
                action: AuditAction.Restore,
                entityId: id,
                entityName: $"{teacher.FirstName} {teacher.LastName}",
                details: $"Teacher '{teacher.FirstName} {teacher.LastName}' restored.",
                oldValue: null,
                newValue: teacher);
        }

        return result;
    }

    #endregion
}