namespace ContentFlow.Application.Specifications.Abstractions;

public interface IQuerySpecification<T>
{
    IQueryable<T> Apply(IQueryable<T> query);
}