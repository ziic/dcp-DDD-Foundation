using System;
using System.Linq.Expressions;

namespace dcp.DDD.DomainModel.SuperTypes
{
    /// <summary>
    /// Contract for query data
    /// </summary>
    /// <typeparam name="T">Entity</typeparam>
    public interface IQueryCommand<T> where T : class
    {
        /// <summary>
        /// Predicate that should be satisfied
        /// </summary>
        Expression<Func<T, bool>> Predicate { get; }
    }
}