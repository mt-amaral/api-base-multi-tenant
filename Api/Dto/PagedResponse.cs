namespace Api.Dto;

public class PagedResponse<TData> : Response<TData>
{
    public PagedResponse(
        TData? data,
        int totalCount,
        int currentPage,
        int pageSize,
        string? message = null)
        : base(data, message)
    {
        TotalCount = totalCount;
        CurrentPage = currentPage;
        PageSize = pageSize;
    }

    public int CurrentPage { get; set; }

    public int PageSize { get; set; }

    public int TotalCount { get; set; }

    public int TotalPages => PageSize <= 0
        ? 0
        : (int)Math.Ceiling(TotalCount / (double)PageSize);
}