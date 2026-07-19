using StudentManagementSystemApp.Enums;
using StudentManagementSystemApp.Interfaces;
using StudentManagementSystemApp.Models;

namespace StudentManagementSystemApp.Services;

public class SubjectService
{
    private readonly ISubjectRepository _repository;
    private readonly IAuditService _auditService;

    public SubjectService(
        ISubjectRepository repository,
        IAuditService auditService)
    {
        _repository = repository;
        _auditService = auditService;
    }

    #region Get

    public async Task<IEnumerable<Subject>> GetAllSubjectsAsync()
    {
        return await _repository.GetActiveAsync();
    }

    public async Task<IEnumerable<Subject>> GetSubjectHistoryAsync()
    {
        return await _repository.GetInactiveAsync();
    }

    #endregion

    #region Pagination

    public async Task<PagedResult<Subject>> GetPagedSubjectsAsync(
        int page,
        int pageSize,
        string? search = null)
    {
        return await _repository.GetPagedAsync(
            page,
            pageSize,
            search);
    }

    public async Task<PagedResult<Subject>> GetPagedSubjectHistoryAsync(
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

    #region Get By Id

    public async Task<Subject?> GetSubjectByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    #endregion

    #region Add

    public async Task<int> AddSubjectAsync(Subject subject)
    {
        if (await _repository.SubjectCodeExistsAsync(subject.SubjectCode))
            throw new Exception("Subject Code already exists.");

        if (await _repository.SubjectNameExistsAsync(subject.SubjectName))
            throw new Exception("Subject Name already exists.");

        int id = await _repository.AddAsync(subject);

        if (id > 0)
        {
            subject.Id = id;

            await _auditService.LogAsync(
                module: AuditModule.Subject,
                action: AuditAction.Add,
                entityId: id,
                entityName: subject.SubjectName,
                details: $"Subject '{subject.SubjectName}' created.",
                oldValue: null,
                newValue: subject);
        }

        return id;
    }

    #endregion

    #region Update

    public async Task<bool> UpdateSubjectAsync(Subject subject)
    {
        if (await _repository.SubjectCodeExistsAsync(
            subject.SubjectCode,
            subject.Id))
            throw new Exception("Subject Code already exists.");

        if (await _repository.SubjectNameExistsAsync(
            subject.SubjectName,
            subject.Id))
            throw new Exception("Subject Name already exists.");

        var oldSubject =
            await _repository.GetByIdAsync(subject.Id);

        bool result =
            await _repository.UpdateAsync(subject);

        if (result)
        {
            await _auditService.LogAsync(
                module: AuditModule.Subject,
                action: AuditAction.Update,
                entityId: subject.Id,
                entityName: subject.SubjectName,
                details: $"Subject '{subject.SubjectName}' updated.",
                oldValue: oldSubject,
                newValue: subject);
        }

        return result;
    }

    #endregion

    #region Inactivate

    public async Task<bool> InactivateSubjectAsync(int id)
    {
        var subject =
            await _repository.GetByIdAsync(id);

        bool result =
            await _repository.InactivateAsync(id);

        if (result && subject != null)
        {
            await _auditService.LogAsync(
                module: AuditModule.Subject,
                action: AuditAction.Delete,
                entityId: id,
                entityName: subject.SubjectName,
                details: $"Subject '{subject.SubjectName}' removed.",
                oldValue: subject,
                newValue: null);
        }

        return result;
    }

    #endregion

    #region Restore

    public async Task<bool> RestoreSubjectAsync(int id)
    {
        var subject =
            await _repository.GetByIdAsync(id);

        bool result =
            await _repository.RestoreAsync(id);

        if (result && subject != null)
        {
            await _auditService.LogAsync(
                module: AuditModule.Subject,
                action: AuditAction.Restore,
                entityId: id,
                entityName: subject.SubjectName,
                details: $"Subject '{subject.SubjectName}' restored.",
                oldValue: null,
                newValue: subject);
        }

        return result;
    }

    #endregion
}