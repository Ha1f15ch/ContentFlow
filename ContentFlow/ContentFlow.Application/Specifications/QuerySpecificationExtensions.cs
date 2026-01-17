using ContentFlow.Application.Specifications.Abstractions;

namespace ContentFlow.Application.Specifications;

public static class QuerySpecificationExtensions
{
    public static IQueryable<T> Apply<T>(
        this IQueryable<T> query,
        params IQuerySpecification<T>[] specs)
    {
        foreach (var spec in specs)
        {
            query = spec.Apply(query);
        }

        return query;
    }
}