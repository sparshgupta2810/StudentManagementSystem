using Dapper;
using StudentManagementSystemApp.Enums;
using StudentManagementSystemApp.Models;
using System.Data;

namespace StudentManagementSystemApp.Extensions;

public static class DapperExtensions
{
    public static async Task<PagedResult<T>> GetPagedAsync<T>(
        this IDbConnection connection,

        string selectSql,

        string fromSql,

        string whereSql,

        string orderBy,

        int page,

        int pageSize,

        object? parameters = null,

        SortDirection direction = SortDirection.Asc)
    {
        int skip = (page - 1) * pageSize;

        string countSql = $"""
            SELECT COUNT(*)
            {fromSql}
            {whereSql}
            """;

        string sql = $"""
            {selectSql}
            {fromSql}
            {whereSql}
            ORDER BY {orderBy}
            OFFSET @Skip ROWS
            FETCH NEXT @PageSize ROWS ONLY
            """;

        var dynamic = new DynamicParameters(parameters);

        dynamic.Add("Skip", skip);

        dynamic.Add("PageSize", pageSize);

        int total = await connection.ExecuteScalarAsync<int>(
            countSql,
            dynamic);

        var data = await connection.QueryAsync<T>(
            sql,
            dynamic);

        return new PagedResult<T>
        {
            Items = data,
            CurrentPage = page,
            PageSize = pageSize,
            TotalRecords = total
        };
    }
}