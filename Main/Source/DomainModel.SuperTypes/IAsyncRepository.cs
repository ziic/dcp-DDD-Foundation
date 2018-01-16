using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace dcp.DDD.DomainModel.SuperTypes
{
    /// <summary>
    /// Default repository contract with async operations
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    public interface IAsyncRepository<T>
        where T : class
    {
        /// <summary>
        /// Return asynchronously all entites
        /// </summary>
        /// <returns>Entities</returns>
        Task<IEnumerable<T>> GetAllAsync();

        /// <summary>
        /// Find asynchronously single entity by key values
        /// </summary>
        /// <param name="keyValues">Key values</param>
        /// <returns>Entity</returns>
        Task<T> FindAsync(params object[] keyValues);

        ///// <summary>
        ///// Find asynchronously single entity by single key value, eager load one related entity (short method)
        ///// </summary>
        ///// <param name="keyValue">Key value</param>
        ///// <param name="includePaths">Paths of related entites</param>
        ///// <returns>Entity</returns>
        ///// <remarks>Use only when single include path</remarks>
        //Task<T> FindAsync(object keyValue, params Expression<Func<T, object>>[] includePaths);

        /// <summary>
        /// Find asynchronously single entity by key value, eager loads related entities
        /// </summary>
        /// <param name="keyValue">Key value</param>
        /// <param name="includePaths">Paths of related entites</param>
        /// <returns>Entity</returns>
        Task<T> FindAsync(object keyValue, IEnumerable<Expression<Func<T, object>>> includePaths);

        /// <summary>
        /// Find asynchronously single entity by key values, eager loads related entities
        /// </summary>
        /// <param name="keyValues">Key values</param>
        /// <param name="includePaths">Paths of related entites</param>
        /// <returns>Entity</returns>
        Task<T> FindAsync(object[] keyValues, IEnumerable<Expression<Func<T, object>>> includePaths);

        /// <summary>
        /// Find asynchronously single entity by key value and return entity projection
        /// </summary>
        /// <typeparam name="TR">Entity projection</typeparam>
        /// <param name="keyValue">Key value</param>
        /// <param name="projection">Factory of entity projection</param>
        /// <returns>Entity projection</returns>
        Task<TR> FindAsync<TR>(object keyValue, Expression<Func<T, TR>> projection);

        /// <summary>
        /// Find asynchronously single entity  by key values and return entity projection
        /// </summary>
        /// <typeparam name="TR">Entity projection</typeparam>
        /// <param name="keyValues">Key values</param>
        /// <param name="projection">Factory of entity projection</param>
        /// <returns>Entity projection</returns>
        Task<TR> FindAsync<TR>(object[] keyValues, Expression<Func<T, TR>> projection);

        /// <summary>
        /// Find asynchronously single entity by single key value, eager load related entities and return enity projection
        /// </summary>
        /// <typeparam name="TR">Entity projection</typeparam>
        /// <param name="keyValue">Key value</param>
        /// <param name="projection">Factory of entity projection</param>
        /// <param name="includePathsFactory">Factory for paths of related entites</param>
        /// <returns>Entity projection</returns>
        Task<TR> FindAsync<TR>(object keyValue, Expression<Func<T, TR>> projection, Expression<Func<T, object>> includePathsFactory);
        
        /// <summary>
        /// Find asynchronously single entity by single key value, eager load related entities and return enity projection
        /// </summary>
        /// <typeparam name="TR">Entity projection</typeparam>
        /// <param name="keyValue">Key value</param>
        /// <param name="projection">Factory of entity projection</param>
        /// <param name="includePaths">Paths of related entites</param>
        /// <returns>Entity projection</returns>
        [Obsolete("Use method with includePaths factory as Anonymous type")]
        Task<TR> FindAsync<TR>(object keyValue, Expression<Func<T, TR>> projection, IEnumerable<Expression<Func<T, object>>> includePaths);

        /// <summary>
        /// Find asynchronously single entity by key values, eager load related entities and return entity projection
        /// </summary>
        /// <typeparam name="TR">Entity projection</typeparam>
        /// <param name="keyValues">Key values</param>
        /// <param name="projection">Factory of entity projection</param>
        /// <param name="includePathsFactory">Factory for paths of related entites</param>
        /// <returns>Entity projection</returns>
        Task<TR> FindAsync<TR>(object[] keyValues, Expression<Func<T, TR>> projection, Expression<Func<T, object>> includePathsFactory);

        /// <summary>
        /// Find asynchronously single entity by key values, eager load related entities and return entity projection
        /// </summary>
        /// <typeparam name="TR">Entity projection</typeparam>
        /// <param name="keyValues">Key values</param>
        /// <param name="projection">Factory of entity projection</param>
        /// <param name="includePaths">Paths of related entites</param>
        /// <returns>Entity projection</returns>
        [Obsolete("Use method with includePaths factory as Anonymous type")]
        Task<TR> FindAsync<TR>(object[] keyValues, Expression<Func<T, TR>> projection, IEnumerable<Expression<Func<T, object>>> includePaths);
        
        /// <summary>
        /// Find asynchronously entities satisfied with query command
        /// </summary>
        /// <param name="queryObject">Query command</param>
        /// <returns>Entites</returns>
        Task<IEnumerable<T>> FindByAsync(IQueryCommand<T> queryObject);

        /// <summary>
        /// Find asynchronously entities satisfied with predicate
        /// </summary>
        /// <typeparam name="TR">Entity projection</typeparam>
        /// <param name="queryObject">Query command</param>
        /// <param name="projection">Factory of entity projection</param>
        /// <returns>Entity projections</returns>
        Task<IEnumerable<TR>> FindByAsync<TR>(IQueryCommand<T> queryObject, Expression<Func<T, TR>> projection);

        /// <summary>
        /// Find entities satisfied with query command, eager load related entities and return entity projections
        /// </summary>
        /// <typeparam name="TR">Entity projection</typeparam>
        /// <param name="queryObject">Query command</param>
        /// <param name="projection">Factory of entity projection</param>
        /// <param name="includePathsFactory">Factory of paths to related entites</param>
        /// <returns>Entity projections</returns>
        Task<IEnumerable<TR>> FindByAsync<TR>(IQueryCommand<T> queryObject, Expression<Func<T, TR>> projection, Expression<Func<T, object>> includePathsFactory);

        /// <summary>
        /// Find asynchronously entities satisfied with query command, eager load related entities and return entity projections
        /// </summary>
        /// <typeparam name="TR">Entity projection</typeparam>
        /// <param name="queryObject">Query command</param>
        /// <param name="projection">Factory of entity projection</param>
        /// <param name="includePaths">Paths of related entites</param>
        /// <returns>Entity projections</returns>
        [Obsolete("Use method with includePaths factory as Anonymous type")]
        Task<IEnumerable<TR>> FindByAsync<TR>(IQueryCommand<T> queryObject, Expression<Func<T, TR>> projection, IEnumerable<Expression<Func<T, object>>> includePaths);

        /// <summary>
        /// Get asynchronously count of entities satisfied with query command
        /// </summary>
        /// <param name="queryObject">Query command</param>
        /// <returns>Count</returns>
        Task<int> CountByAsync(IQueryCommand<T> queryObject);

        /// <summary>
        /// Check asynchronously if elements satisfied with query are exists
        /// </summary>
        /// <param name="queryObject">Query</param>
        /// <returns>Is any</returns>
        Task<bool> AnyByAsync(IQueryCommand<T> queryObject);

        /// <summary>
        /// Find asynchronously entities satisfied with predicate
        /// </summary>
        /// <param name="predicate">Query predicate</param>
        /// <returns>Entites</returns>
        Task<IEnumerable<T>> FindByAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Find asynchronously entities satisfied with predicate
        /// </summary>
        /// <typeparam name="TR">Entity projection</typeparam>
        /// <param name="predicate">Query predicate</param>
        /// <param name="projection">Factory of entity projection</param>
        /// <returns>Entites projetions</returns>
        Task<IEnumerable<TR>> FindByAsync<TR>(Expression<Func<T, bool>> predicate, Expression<Func<T, TR>> projection);

        /// <summary>
        /// Find asynchronously entities satisfied with predicate, eager loads related entities and returns entity projections
        /// </summary>
        /// <typeparam name="TR">Entity projection</typeparam>
        /// <param name="predicate">Query predicate</param>
        /// <param name="projection">Factory of entity projection</param>
        /// <param name="includePathsFactory">Factory for paths to related entites</param>
        /// <returns>Entites projetions</returns>
        Task<IEnumerable<TR>> FindByAsync<TR>(Expression<Func<T, bool>> predicate, Expression<Func<T, TR>> projection, Expression<Func<T, object>> includePathsFactory);

        /// <summary>
        /// Find asynchronously entities satisfied with predicate, eager loads related entities and returns entity projections
        /// </summary>
        /// <typeparam name="TR">Entity projection</typeparam>
        /// <param name="predicate">Query predicate</param>
        /// <param name="projection">Factory of entity projection</param>
        /// <param name="includePaths">Paths of related entites</param>
        /// <returns>Entites projetions</returns>
        [Obsolete("Use method with includePaths factory as Anonymous type")]
        Task<IEnumerable<TR>> FindByAsync<TR>(Expression<Func<T, bool>> predicate, Expression<Func<T, TR>> projection, IEnumerable<Expression<Func<T, object>>> includePaths);

        /// <summary>
        /// Get asynchronously count of entities satisfied with query predicate
        /// </summary>
        /// <param name="predicate">Query predicate</param>
        /// <returns>Count</returns>
        Task<int> CountByAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Check asynchronously if entites satisfied with query predicate are exists
        /// </summary>
        /// <param name="predicate">Query predicate</param>
        /// <returns>Is Any</returns>
        Task<bool> AnyByAsync(Expression<Func<T, bool>> predicate);
    }
}