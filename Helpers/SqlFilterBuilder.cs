using Dapper;

namespace StudentManagementSystemApp.Helpers;

public class SqlFilterBuilder
{
    private readonly List<string> _conditions = new();

    private readonly DynamicParameters _parameters = new();

    public SqlFilterBuilder Where(string condition)
    {
        _conditions.Add(condition);

        return this;
    }

    public SqlFilterBuilder Search(
        string? search,
        params string[] columns)
    {
        if (string.IsNullOrWhiteSpace(search))
            return this;

        List<string> searchConditions = new();

        foreach (var column in columns)
        {
            searchConditions.Add($"{column} LIKE '%' + @Search + '%'");
        }

        _conditions.Add($"({string.Join(" OR ", searchConditions)})");

        _parameters.Add("Search", search);

        return this;
    }

    public string BuildWhereClause()
    {
        if (!_conditions.Any())
            return "";

        return "WHERE " + string.Join(" AND ", _conditions);
    }

    public DynamicParameters Parameters => _parameters;
}