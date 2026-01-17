using BuildingBlocks.CQRS;

namespace BuildingBlocks.Pagination;

public interface IPaginatedQuery<TResponse> : IQuery<TResponse>
    where TResponse : notnull
{
    public int PageIndex { get; }
    public int PageSize { get; }
}
