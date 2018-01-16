using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace dcp.DDD.DomainModel.SuperTypes
{
    public static class RepositoryExtenssions
    {
        #region Sync
        /// <summary>
        /// Find single entity by single key value, eager load related entities
        /// </summary>
        /// <param name="repository">Repository</param>
        /// <param name="keyValue">Key value</param>
        /// <param name="includePaths">Paths of related entites</param>
        /// <returns>Entity</returns>
        public static T Find<T>(this IRepository<T> repository, object keyValue, params Expression<Func<T, object>>[] includePaths) where T : class
        {
            return repository.Find(keyValue, includePaths);
        }

        /// <summary>
        /// Find single entity by key values, eager load related entities
        /// </summary>
        /// <param name="repository">Repository</param>
        /// <param name="keyValues">Key values</param>
        /// <param name="includePaths">Paths of related entites</param>
        /// <returns>Entity</returns>
        public static T Find<T>(this IRepository<T> repository, object[] keyValues, params Expression<Func<T, object>>[] includePaths) where T : class
        {
            return repository.Find(keyValues, includePaths);
        }

        /// <summary>
        /// More useful method when eager loads several related entities
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="repository">Repository</param>
        /// <param name="keyValue">Key</param>
        /// <param name="includePaths">Include paths</param>
        /// <returns></returns>
        [Obsolete("Use Find extension method. This method will be deleted")]
        public static T FindOnlyWithIncludes<T>(this IRepository<T> repository, object keyValue, params Expression<Func<T, object>>[] includePaths) where T : class
        {
            return Find(repository, keyValue, includePaths);
        }

        /// <summary>
        /// Find single entity by single key value, eager load related entities and return enity projection
        /// </summary>
        /// <typeparam name="T">Entity</typeparam>
        /// <typeparam name="TR">Entity projection</typeparam>
        /// <param name="repository">Repository</param>
        /// <param name="keyValue">Key value</param>
        /// <param name="projection">Factory of entity projection</param>
        /// <param name="includePaths">Paths of related entites</param>
        /// <returns>Entity projection</returns>
        [Obsolete("Use method with includePaths factory as Anonymous type")]
        public static TR Find<T, TR>(this IRepository<T> repository, object keyValue, Expression<Func<T, TR>> projection, params Expression<Func<T, object>>[] includePaths) where T : class
        {
            return repository.Find(keyValue, projection, includePaths);
        }

        /// <summary>
        /// Find single entity by key values, eager load related entities and return entity projection
        /// </summary>
        /// <typeparam name="T">Entity</typeparam>
        /// <typeparam name="TR">Entity projection</typeparam>
        /// <param name="repository">Repository</param>
        /// <param name="keyValues">Key values</param>
        /// <param name="projection">Factory of entity projection</param>
        /// <param name="includePaths">Paths of related entites</param>
        /// <returns>Entity projection</returns>
        [Obsolete("Use method with includePaths factory as Anonymous type")]
        public static TR Find<T, TR>(this IRepository<T> repository, object[] keyValues, Expression<Func<T, TR>> projection, params Expression<Func<T, object>>[] includePaths) where T : class
        {
            return repository.Find(keyValues, projection, includePaths);
        }

        /// <summary>
        /// Find entities satisfied with query command, eager load related entities and return entity projections
        /// </summary>
        /// <typeparam name="T">Entity</typeparam>
        /// <typeparam name="TR">Entity projection</typeparam>
        /// <param name="repository">Repository</param>
        /// <param name="queryObject">Query command</param>
        /// <param name="projection">Factory of entity projection</param>
        /// <param name="includePaths">Paths of related entites</param>
        /// <returns>Entity projections</returns>
        [Obsolete("Use method with includePaths factory as Anonymous type")]
        public static IEnumerable<TR> FindBy<T, TR>(this IRepository<T> repository, IQueryCommand<T> queryObject, Expression<Func<T, TR>> projection, params Expression<Func<T, object>>[] includePaths)
            where T : class
        {
            return repository.FindBy(queryObject, projection, includePaths);
        }

        /// <summary>
        /// Find entities satisfied with predicate, eager loads related entities and return entity projections
        /// </summary>
        /// <typeparam name="T">Entity</typeparam>
        /// <typeparam name="TR">Entity projection</typeparam>
        /// <param name="repository">Repository</param>
        /// <param name="predicate">Query predicate</param>
        /// <param name="projection">Factory of entity projection</param>
        /// <param name="includePaths">Paths of related entites</param>
        /// <returns>Entites projetions</returns>
        [Obsolete("Use method with includePaths factory as Anonymous type")]
        public static IEnumerable<TR> FindBy<T, TR>(this IRepository<T> repository, Expression<Func<T, bool>> predicate, Expression<Func<T, TR>> projection,
            params Expression<Func<T, object>>[] includePaths) where T : class
        {
            return repository.FindBy(predicate, projection, includePaths.AsEnumerable());
        }

        #endregion

        #region Async

        /// <summary>
        /// More useful method when eager loads several related entities
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="repository"></param>
        /// <param name="keyValue"></param>
        /// <param name="includePaths"></param>
        /// <returns></returns>
        public static Task<T> FindAsync<T>(this IAsyncRepository<T> repository, object keyValue, params Expression<Func<T, object>>[] includePaths) where T : class
        {
            return repository.FindAsync(keyValue, includePaths);
        }


        /// <summary>
        /// More useful method when eager loads several related entities
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="repository"></param>
        /// <param name="keyValue"></param>
        /// <param name="includePaths"></param>
        /// <returns></returns>
        [Obsolete("Use FindAsync extension method")]
        public static Task<T> FindOnlyWithIncludesAsync<T>(this IAsyncRepository<T> repository, object keyValue, params Expression<Func<T, object>>[] includePaths) where T : class
        {
            return FindAsync(repository, keyValue, includePaths);
        }

        /// <summary>
        /// Find single entity and eager load related entities
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="repository">Repository</param>
        /// <param name="keyValues">Key values</param>
        /// <param name="includePaths">Include paths to related entities</param>
        /// <returns></returns>
        public static Task<T> FindAsync<T>(this IAsyncRepository<T> repository, object[] keyValues, params Expression<Func<T, object>>[] includePaths) where T : class
        {
            return repository.FindAsync(keyValues, includePaths);
        }

        /// <summary>
        /// Find asynchronously single entity by single key value, eager load related entities and return enity projection
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <typeparam name="TR">Entity projection</typeparam>
        /// <param name="repository">Repository</param>
        /// <param name="keyValue">Key value</param>
        /// <param name="projection">Factory of entity projection</param>
        /// <param name="includePaths">Paths of related entites</param>
        /// <returns>Entity projection</returns>
        [Obsolete("Use method with includePaths factory as Anonymous type")]
        public static Task<TR> FindAsync<T, TR>(this IAsyncRepository<T> repository, object keyValue, Expression<Func<T, TR>> projection,
            params Expression<Func<T, object>>[] includePaths) where T : class
        {
            return repository.FindAsync(keyValue, projection, includePaths);
        }

        /// <summary>
        /// Find single entity by key values, eager load related entities and return entity projection
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <typeparam name="TR">Entity projection</typeparam>
        /// <param name="repository">Repository</param>
        /// <param name="keyValues">Key values</param>
        /// <param name="projection">Factory of entity projection</param>
        /// <param name="includePaths">Paths of related entites</param>
        /// <returns>Entity projection</returns>
        [Obsolete("Use method with includePaths factory as Anonymous type")]
        public static Task<TR> FindAsync<T, TR>(this IAsyncRepository<T> repository, object[] keyValues, Expression<Func<T, TR>> projection,
            params Expression<Func<T, object>>[] includePaths) where T : class
        {
            return repository.FindAsync(keyValues, projection, includePaths);
        }

        /// <summary>
        /// Find asynchronously entities satisfied with query command, eager load related entities and return entity projections
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <typeparam name="TR">Entity projection</typeparam>
        /// <param name="repository">Repository</param>
        /// <param name="queryObject">Query command</param>
        /// <param name="projection">Factory of entity projection</param>
        /// <param name="includePaths">Paths of related entites</param>
        /// <returns>Entity projections</returns>
        [Obsolete("Use method with includePaths factory as Anonymous type")]
        public static Task<IEnumerable<TR>> FindByAsync<T, TR>(this IAsyncRepository<T> repository, IQueryCommand<T> queryObject, Expression<Func<T, TR>> projection,
            params Expression<Func<T, object>>[] includePaths) where T : class
        {
            return repository.FindByAsync(queryObject, projection, includePaths);
        }

        /// <summary>
        /// Find asynchronously entities satisfied with predicate, eager loads related entities and returns entity projections
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <typeparam name="TR">Entity projection</typeparam>
        /// <param name="repository">Repository</param>
        /// <param name="predicate">Query predicate</param>
        /// <param name="projection">Factory of entity projection</param>
        /// <param name="includePaths">Paths of related entites</param>
        /// <returns>Entites projetions</returns>
        [Obsolete("Use method with includePaths factory as Anonymous type")]
        public static Task<IEnumerable<TR>> FindByAsync<T, TR>(this IAsyncRepository<T> repository, Expression<Func<T, bool>> predicate, Expression<Func<T, TR>> projection,
            params Expression<Func<T, object>>[] includePaths) where T : class
        {
            return repository.FindByAsync(predicate, projection, includePaths);
        }

        #endregion
    }
}