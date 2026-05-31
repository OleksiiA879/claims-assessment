namespace ClaimsModule.Application.Common.Models;

public class PaginatedList<T>
{
    public IReadOnlyList<T> Items { get; init; } = [];
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
    public int TotalCount { get; init; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPrevious => PageNumber > 1;
    public bool HasNext => PageNumber < TotalPages;

    public static PaginatedList<T> Create(IEnumerable<T> source, int count, int pageNumber, int pageSize) =>
        new()
        {
            Items = source.ToList(),
            TotalCount = count,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
}
