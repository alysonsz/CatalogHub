namespace CatalogHub.Application.Common.Pagination;

public class PagedResponse<T>
{
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public int TotalCount { get; set; }
    public bool HasPrevious => CurrentPage > 1;
    public bool HasNext => CurrentPage < TotalPages;
    public List<T> Data { get; set; }

    public PagedResponse(List<T> data, int count, int pageNumber, int pageSize)
    {
        Data = data;
        TotalCount = count;
        PageSize = pageSize;
        CurrentPage = pageNumber;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
    }
}
