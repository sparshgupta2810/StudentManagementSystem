using StudentManagementSystemApp.Models;

namespace StudentManagementSystemApp.Interfaces;

public interface IAuditComparer
{
    List<AuditChange> Compare<T>(
        T? oldObject,
        T? newObject);
}