using StudentManagementSystemApp.Enums;
using StudentManagementSystemApp.Interfaces;
using StudentManagementSystemApp.Models;

namespace StudentManagementSystemApp.Services;

public class StudentService
{
    private readonly IStudentRepository _repository;
    private readonly IAuditService _auditService;

    public StudentService(
        IStudentRepository repository,
        IAuditService auditService)
    {
        _repository = repository;
        _auditService = auditService;
    }

    #region Get

    public async Task<IEnumerable<Student>> GetAllStudentsAsync()
    {
        return await _repository.GetActiveAsync();
    }

    public async Task<PagedResult<Student>> GetPagedStudentsAsync(
        int page,
        int pageSize,
        string? search = null)
    {
        return await _repository.GetPagedAsync(
            page,
            pageSize,
            search);
    }

    public async Task<PagedResult<Student>> GetPagedStudentHistoryAsync(
        int page,
        int pageSize,
        string? search = null)
    {
        return await _repository.GetHistoryPagedAsync(
            page,
            pageSize,
            search);
    }

    public async Task<IEnumerable<Student>> GetStudentHistoryAsync()
    {
        return await _repository.GetInactiveAsync();
    }

    public async Task<Student?> GetStudentByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    #endregion

    #region Add

    public async Task<int> AddStudentAsync(Student student)
    {
        if (await _repository.RegistrationExistsAsync(student.RegistrationNo))
            throw new Exception("Registration Number already exists.");

        if (await _repository.EmailExistsAsync(student.Email))
            throw new Exception("Email already exists.");

        int id = await _repository.AddAsync(student);

        if (id > 0)
        {
            student.Id = id;

            await _auditService.LogAsync(
                module: AuditModule.Student,
                action: AuditAction.Add,
                entityId: id,
                entityName: $"{student.FirstName} {student.LastName}",
                details: $"Student '{student.FirstName} {student.LastName}' created.",
                oldValue: null,
                newValue: student);
        }

        return id;
    }

    #endregion

    #region Update

    public async Task<bool> UpdateStudentAsync(Student student)
    {
        if (await _repository.RegistrationExistsAsync(
            student.RegistrationNo,
            student.Id))
            throw new Exception("Registration Number already exists.");

        if (await _repository.EmailExistsAsync(
            student.Email,
            student.Id))
            throw new Exception("Email already exists.");

        var oldStudent =
            await _repository.GetByIdAsync(student.Id);

        bool result =
            await _repository.UpdateAsync(student);

        if (result)
        {
            await _auditService.LogAsync(
                module: AuditModule.Student,
                action: AuditAction.Update,
                entityId: student.Id,
                entityName: $"{student.FirstName} {student.LastName}",
                details: $"Student '{student.FirstName} {student.LastName}' updated.",
                oldValue: oldStudent,
                newValue: student);
        }

        return result;
    }

    #endregion

    #region Inactivate

    public async Task<bool> InactivateStudentAsync(int id)
    {
        var student =
            await _repository.GetByIdAsync(id);

        bool result =
            await _repository.InactivateAsync(id);

        if (result && student != null)
        {
            await _auditService.LogAsync(
                module: AuditModule.Student,
                action: AuditAction.Delete,
                entityId: id,
                entityName: $"{student.FirstName} {student.LastName}",
                details: $"Student '{student.FirstName} {student.LastName}' removed.",
                oldValue: student,
                newValue: null);
        }

        return result;
    }

    #endregion

    #region Restore

    public async Task<bool> RestoreStudentAsync(int id)
    {
        var student =
            await _repository.GetByIdAsync(id);

        bool result =
            await _repository.RestoreAsync(id);

        if (result && student != null)
        {
            await _auditService.LogAsync(
                module: AuditModule.Student,
                action: AuditAction.Restore,
                entityId: id,
                entityName: $"{student.FirstName} {student.LastName}",
                details: $"Student '{student.FirstName} {student.LastName}' restored.",
                oldValue: null,
                newValue: student);
        }

        return result;
    }

    #endregion
}