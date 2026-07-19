using StudentManagementSystemApp.Interfaces;
using StudentManagementSystemApp.Models;

namespace StudentManagementSystemApp.Helpers;

public class AuditComparer : IAuditComparer
{
    private static readonly HashSet<string> IgnoredProperties =
    [
        "CreatedDate",
        "ModifiedDate",
        "PasswordHash",
        "SecurityStamp",
        "ConcurrencyStamp",
        "NormalizedUserName",
        "NormalizedEmail",
        "LockoutEnd",
        "AccessFailedCount"
    ];

    public List<AuditChange> Compare<T>(
        T? oldObject,
        T? newObject)
    {
        var changes = new List<AuditChange>();

        var source = oldObject ?? newObject;

        if (source == null)
            return changes;

        var properties = source.GetType().GetProperties();

        foreach (var property in properties)
        {
            if (!property.CanRead)
                continue;

            if (IgnoredProperties.Contains(property.Name))
                continue;

            var oldValue = oldObject == null
                ? null
                : property.GetValue(oldObject);

            var newValue = newObject == null
                ? null
                : property.GetValue(newObject);

            string oldText = oldValue?.ToString() ?? "";
            string newText = newValue?.ToString() ?? "";

            if (oldText == newText)
                continue;

            changes.Add(new AuditChange
            {
                Property = SplitWords(property.Name),
                OldValue = oldText,
                NewValue = newText
            });
        }

        return changes;
    }

    private static string SplitWords(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return value;

        return System.Text.RegularExpressions.Regex.Replace(
            value,
            "([a-z])([A-Z])",
            "$1 $2");
    }
}