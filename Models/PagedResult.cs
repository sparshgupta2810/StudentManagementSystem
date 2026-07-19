namespace StudentManagementSystemApp.Models;

public class PagedResult<T>
{
    public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();

    public int CurrentPage { get; set; }

    public int PageSize { get; set; }

    public int TotalRecords { get; set; }

    public int TotalPages =>
        (int)Math.Ceiling((double)TotalRecords / PageSize);

    public int StartRecord =>
        TotalRecords == 0 ? 0 : ((CurrentPage - 1) * PageSize) + 1;

    public int EndRecord =>
        Math.Min(CurrentPage * PageSize, TotalRecords);
}