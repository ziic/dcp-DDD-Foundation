using System;
using System.Linq.Expressions;

namespace dcp.DDD.DomainModel.SuperTypes
{
    public interface IQueryCommand<T> where T : class
    {
        Expression<Func<T, bool>> Predicate { get; }
    }
}