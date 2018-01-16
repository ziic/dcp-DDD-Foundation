using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace dcp.DDD.DomainModel.SuperTypes
{
    /// <summary>
    /// Repository contract
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    public interface IRepository<T>
        where T : class
    {
        /// <summary>
        /// Return all entites
        /// </summary>
        /// <returns>Entities</returns>
        IEnumerable<T> GetAll();
        
        /// <summary>
        /// Find single entity by key values
        /// </summary>
        /// <param name="keyValues">Key values</param>
        /// <returns>Entity</returns>
        T Find(params object[] keyValues);

        /// <summary>
        /// Find single entity by key value, eager load related entities
        /// </summary>
        /// <param name="keyValue">Key value</param>
        /// <param name="includePaths">Paths of related entites</param>
        /// <returns>Entity</returns>
        T Find(object keyValue, IEnumerable<Expression<Func<T, object>>> includePaths);

        /// <summary>
        /// Find single entity by key values, eager load related entities
        /// </summary>
        /// <param name="keyValues">Key values</param>
        /// <param name="includePaths">Paths of related entites</param>
        /// <returns>Entity</returns>
        T Find(object[] keyValues, IEnumerable<Expression<Func<T, object>>> includePaths);

        /// <summary>
        /// Find single entity by key value and return entity projection
        /// </summary>
        /// <typeparam name="TR">Entity projection</typeparam>
        /// <param name="keyValue">Key value</param>
        /// <param name="projection">Factory of entity projection</param>
        /// <returns>Entity projection</returns>
        TR Find<TR>(object keyValue, Expression<Func<T, TR>> projection);

        /// <summary>
        /// Find single entity by key values and return entity projection
        /// </summary>
        /// <typeparam name="TR">Entity projection</typeparam>
        /// <param name="keyValues">Key values</param>
        /// <param name="projection">Factory of entity projection</param>
        /// <returns>Entity projection</returns>
        TR Find<TR>(object[] keyValues, Expression<Func<T, TR>> projection);

        /// <summary>
        /// Find single entity by single key value, eager load related entities and return enity projection
        /// </summary>
        /// <typeparam name="TR">Entity projection</typeparam>
        /// <param name="keyValue">Key value</param>
        /// <param name="projection">Factory of entity projection</param>
        /// <param name="includePathsFactory">Factory for paths of related entites</param>
        /// <returns>Entity projection</returns>
        TR Find<TR>(object keyValue, Expression<Func<T, TR>> projection, Expression<Func<T, object>> includePathsFactory);

        /// <summary>
        /// Find single entity by single key value, eager load related entities and return enity projection
        /// </summary>
        /// <typeparam name="TR">Entity projection</typeparam>
        /// <param name="keyValue">Key value</param>
        /// <param name="projection">Factory of entity projection</param>
        /// <param name="includePaths">Paths of related entites</param>
        /// <returns>Entity projection</returns>
        [Obsolete("Use method with includePaths factory as Anonymous type")]
        TR Find<TR>(object keyValue, Expression<Func<T, TR>> projection, IEnumerable<Expression<Func<T, object>>> includePaths);

        /// <summary>
        /// Find single entity by key values, eager load related entities and return entity projection
        /// </summary>
        /// <typeparam name="TR">Entity projection</typeparam>
        /// <param name="keyValues">Key values</param>
        /// <param name="projection">Factory of entity projection</param>
        /// <param name="includePathsFactory">Factory for paths of related entites</param>
        /// <returns>Entity projection</returns>
        TR Find<TR>(object[] keyValues, Expression<Func<T, TR>> projection, Expression<Func<T, object>> includePathsFactory);

        /// <summary>
        /// Find single entity by key values, eager load related entities and return entity projection
        /// </summary>
        /// <typeparam name="TR">Entity projection</typeparam>
        /// <param name="keyValues">Key values</param>
        /// <param name="projection">Factory of entity projection</param>
        /// <param name="includePaths">Paths of related entites</param>
        /// <returns>Entity projection</returns>
        [Obsolete("Use method with includePaths factory as Anonymous type")]
        TR Find<TR>(object[] keyValues, Expression<Func<T, TR>> projection, IEnumerable<Expression<Func<T, object>>> includePaths);
        
        /// <summary>
        /// Find entities satisfied with query command
        /// </summary>
        /// <param name="queryObject">Query command</param>
        /// <returns>Entites</returns>
        IEnumerable<T> FindBy(IQueryCommand<T> queryObject);

        /// <summary>
        /// Find entities satisfied with predicate
        /// </summary>
        /// <typeparam name="TR">Entity projection</typeparam>
        /// <param name="queryObject">Query command</param>
        /// <param name="projection">Factory of entity projection</param>
        /// <returns>Entity projections</returns>
        IEnumerable<TR> FindBy<TR>(IQueryCommand<T> queryObject, Expression<Func<T, TR>> projection);

        /// <summary>
        /// Find entities satisfied with query command, eager load related entities and return entity projections
        /// </summary>
        /// <typeparam name="TR">Entity projection</typeparam>
        /// <param name="queryObject">Query command</param>
        /// <param name="projection">Factory of entity projection</param>
        /// <param name="includePathsFactory">Factory of paths to related entites</param>
        /// <returns>Entity projections</returns>
        IEnumerable<TR> FindBy<TR>(IQueryCommand<T> queryObject, Expression<Func<T, TR>> projection, Expression<Func<T, object>> includePathsFactory);

        /// <summary>
        /// Find entities satisfied with query command, eager load related entities and return entity projections
        /// </summary>
        /// <typeparam name="TR">Entity projection</typeparam>
        /// <param name="queryObject">Query command</param>
        /// <param name="projection">Factory of entity projection</param>
        /// <param name="includePaths">Paths of related entites</param>
        /// <returns>Entity projections</returns>
        [Obsolete("Use method with includePaths factory as Anonymous type")]
        IEnumerable<TR> FindBy<TR>(IQueryCommand<T> queryObject, Expression<Func<T, TR>> projection, IEnumerable<Expression<Func<T, object>>> includePaths);

        /// <summary>
        /// Get count of entities satisfied with query command
        /// </summary>
        /// <param name="queryObject">Query command</param>
        /// <returns>Count</returns>
        int CountBy(IQueryCommand<T> queryObject);

        /// <summary>
        /// Check if elements satisfied with query are exists
        /// </summary>
        /// <param name="queryObject">Query</param>
        /// <returns>Is any</returns>
        bool AnyBy(IQueryCommand<T> queryObject);
        

        /// <summary>
        /// Find entities satisfied with predicate
        /// </summary>
        /// <param name="predicate">Query predicate</param>
        /// <returns>Entites</returns>
        IEnumerable<T> FindBy(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Find entities satisfied with predicate
        /// </summary>
        /// <typeparam name="TR">Entity projection</typeparam>
        /// <param name="predicate">Query predicate</param>
        /// <param name="projection">Factory of entity projection</param>
        /// <returns>Entites projetions</returns>
        IEnumerable<TR> FindBy<TR>(Expression<Func<T, bool>> predicate, Expression<Func<T, TR>> projection);

        /// <summary>
        /// Find entities satisfied with predicate, eager loads related entities and return entity projections
        /// </summary>
        /// <typeparam name="TR">Entity projection</typeparam>
        /// <param name="predicate">Query predicate</param>
        /// <param name="projection">Factory of entity projection</param>
        /// <param name="includePathsFactory">Factory for paths to related entites</param>
        /// <returns>Entites projetions</returns>
        IEnumerable<TR> FindBy<TR>(Expression<Func<T, bool>> predicate, Expression<Func<T, TR>> projection, Expression<Func<T, object>> includePathsFactory);

        /// <summary>
        /// Find entities satisfied with predicate, eager load related entities and return entity projections
        /// </summary>
        /// <typeparam name="TR">Entity projection</typeparam>
        /// <param name="predicate">Query predicate</param>
        /// <param name="projection">Factory of entity projection</param>
        /// <param name="includePaths">Paths of related entites</param>
        /// <returns>Entites projetions</returns>
        [Obsolete("Use method with includePaths factory as Anonymous type")]
        IEnumerable<TR> FindBy<TR>(Expression<Func<T, bool>> predicate, Expression<Func<T, TR>> projection, IEnumerable<Expression<Func<T, object>>> includePaths);
        
        /// <summary>
        /// Get count of entities satisfied with query predicate
        /// </summary>
        /// <param name="predicate">Query predicate</param>
        /// <returns>Count</returns>
        int CountBy(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Check if entites satisfied with query predicate are exists
        /// </summary>
        /// <param name="predicate">Query predicate</param>
        /// <returns>Is Any</returns>
        bool AnyBy(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Add entity to repository
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>Entity</returns>
        T Add(T entity);

        /// <summary>
        /// Add range of entities
        /// </summary>
        /// <param name="entities">Entities</param>
        /// <returns>Entities</returns>
        IEnumerable<T> AddRange(IEnumerable<T> entities);

        /// <summary>
        /// Remove entity
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>Entity</returns>
        T Remove(T entity);

        /// <summary>
        /// Remove entity by keyValues
        /// </summary>
        /// <param name="keyValues">Key values</param>
        void Remove(params object[] keyValues);

        /// <summary>
        /// Remove range of entities
        /// </summary>
        /// <param name="entities">Entities</param>
        /// <returns>Entities</returns>
        IEnumerable<T> RemoveRange(IEnumerable<T> entities);
    }
}