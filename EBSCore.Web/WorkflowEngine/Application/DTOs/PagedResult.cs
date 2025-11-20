namespace EBSCore.Web.WorkflowEngine.Application.DTOs;

public class PagedResult<T>
{
    public IReadOnlyCollection<T> Items { get; }
    public int TotalCount { get; }

    public PagedResult(IReadOnlyCollection<T> items, int totalCount)
    {
        Items = items;
        TotalCount = totalCount;
    }
}
