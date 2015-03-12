using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using dcp.DDD.DomainModel.SuperTypes;
using System.Data.Entity.Core.Metadata.Edm;

namespace dcp.DDD.Infrastructure.Data.EF.SuperTypes
{
    /// <summary>
    /// Base repository (Entity Framework)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class RepositoryBase<T> : IRepository<T>
        where T : class
    {
        private readonly IEnumerable<Expression<Func<T, object>>> _keys;
        protected readonly DbContext Context;
        protected readonly DbSet<T> Set;
        protected readonly ObjectContext ObjectContext;
        protected readonly ObjectSet<T> ObjectSet;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="unitOfWork">Unit of work</param>
        /// <param name="keys">Primary keys of entity</param>
        protected RepositoryBase(IUnitOfWork unitOfWork, params Expression<Func<T, object>>[] keys)
        {
            _keys = keys;
            Context = (DbContext)unitOfWork;

            ObjectContext = ((IObjectContextAdapter)unitOfWork).ObjectContext;

            Set = Context.Set<T>();

            ObjectSet = ObjectContext.CreateObjectSet<T>();
        }

        /// <summary>
        /// Returns all entites
        /// </summary>
        /// <returns>Entities</returns>
        public IEnumerable<T> GetAll()
        {
            return Set.ToList();
        }

        /// <summary>
        /// Finds single entity by key values
        /// </summary>
        /// <param name="keyValues">Key values</param>
        /// <returns>Entity</returns>
        public virtual T Find(params object[] keyValues)
        {
            return Set.Find(keyValues);
        }

        /// <summary>
        /// Finds single entity by key values, eager loads related entities
        /// </summary>
        /// <param name="keyValues">Key values</param>
        /// <param name="includePaths">Paths of related entites</param>
        /// <returns>Entity</returns>
        public T Find(object[] keyValues, IEnumerable<Expression<Func<T, object>>> includePaths)
        {
            ObjectQuery<T> objectQuery = ObjectSet;

            return objectQuery
                .IncludeToQuery(includePaths)
                .FilterQueryByKeys(_keys, keyValues)
                .SingleOrDefault();
        }

        /// <summary>
        /// Finds single entity by key value, eager loads related entities
        /// </summary>
        /// <param name="keyValue">Key value</param>
        /// <param name="includePaths">Paths of related entites</param>
        /// <returns>Entity</returns>
        public T Find(object keyValue, IEnumerable<Expression<Func<T, object>>> includePaths)
        {
            return Find(new[] { keyValue }, includePaths);
        }

        /// <summary>
        /// Finds single entity by key values, eager loads related entities
        /// </summary>
        /// <remarks>
        /// Use only when single include path
        /// </remarks>
        /// <param name="keyValue">Key values</param>
        /// <param name="includePaths">Paths of related entites</param>
        /// <returns>Entity</returns>
        public T Find(object keyValue, params Expression<Func<T, object>>[] includePaths)
        {
            return Find(new[] { keyValue }, includePaths.AsEnumerable());
        }

        /// <summary>
        /// Finds single entity by key values and retutns entity projection
        /// </summary>
        /// <typeparam name="TR">Entity projection</typeparam>
        /// <param name="keyValues">Key values</param>
        /// <param name="projection">Factory of entity projection</param>
        /// <returns>Entity projection</returns>
        public TR Find<TR>(object[] keyValues, Expression<Func<T, TR>> projection)
        {
            ObjectQuery<T> objectQuery = ObjectSet;

            return objectQuery
                .FilterQueryByKeys(_keys, keyValues)
                .Select(projection)
                .SingleOrDefault();
        }

        /// <summary>
        /// Finds single entity by key value and returns entity projection
        /// </summary>
        /// <typeparam name="TR">Entity projection</typeparam>
        /// <param name="keyValue">Key value</param>
        /// <param name="projection">Factory of entity projection</param>
        /// <returns>Entity projection</returns>
        public TR Find<TR>(object keyValue, Expression<Func<T, TR>> projection)
        {
            return Find(new[] { keyValue }, projection);
        }

        /// <summary>
        /// Finds single entity by key values, eager loads related entities and returns entity projection
        /// </summary>
        /// <typeparam name="TR">Entity projection</typeparam>
        /// <param name="keyValues">Key values</param>
        /// <param name="projection">Factory of entity projection</param>
        /// <param name="includePaths">Paths of related entites</param>
        /// <returns>Entity projection</returns>
        public TR Find<TR>(object[] keyValues, Expression<Func<T, TR>> projection, IEnumerable<Expression<Func<T, object>>> includePaths)
        {
            var projectionWithIncludes = ExpressionHelper.TranslateToProjectionWithIncludes(includePaths);

            var materializedResult = ObjectSet
                .IncludeToQuery(includePaths)
                .FilterQueryByKeys(_keys, keyValues).AsEnumerable()
                .Select(projectionWithIncludes.Compile()).SingleOrDefault();

            return materializedResult
                .Where(x => x.Key == "Entity")
                .Select(x => (T)x.Value)
                .Select(projection.Compile()).SingleOrDefault();

        }

        /// <summary>
        /// Finds single entity by single key value, eager loads related entities and returns enity projection
        /// </summary>
        /// <typeparam name="TR">Entity projection</typeparam>
        /// <param name="keyValue">Key value</param>
        /// <param name="projection">Factory of entity projection</param>
        /// <param name="includePaths">Paths of related entites</param>
        /// <returns>Entity projection</returns>
        public TR Find<TR>(object keyValue, Expression<Func<T, TR>> projection, params Expression<Func<T, object>>[] includePaths)
        {
            return Find(new[] { keyValue }, projection, includePaths.AsEnumerable());
        }

        /// <summary>
        /// Finds entities satisfied with query command
        /// </summary>
        /// <param name="queryObject">Query command</param>
        /// <returns>Entites</returns>
        public IEnumerable<T> FindBy(IQueryCommand<T> queryObject)
        {
            return Set
                .Where(queryObject.Predicate)
                .ToList();
        }

        /// <summary>
        /// Finds entities satisfied with predicate
        /// </summary>
        /// <typeparam name="TR">Entity projection</typeparam>
        /// <param name="queryObject">Query command</param>
        /// <param name="projection">Factory of entity projection</param>
        /// <returns>Entity projections</returns>
        public IEnumerable<TR> FindBy<TR>(IQueryCommand<T> queryObject, Expression<Func<T, TR>> projection)
        {
            return FindBy(queryObject.Predicate, projection);
        }

        /// <summary>
        /// Finds entities satisfied with query command, eager loads related entities and returns entity projections
        /// </summary>
        /// <typeparam name="TR">Entity projection</typeparam>
        /// <param name="queryObject">Query command</param>
        /// <param name="projection">Factory of entity projection</param>
        /// <param name="includePaths">Paths of related entites</param>
        /// <returns>Entity projections</returns>
        public IEnumerable<TR> FindBy<TR>(IQueryCommand<T> queryObject, Expression<Func<T, TR>> projection, IEnumerable<Expression<Func<T, object>>> includePaths)
        {
            return FindBy(queryObject.Predicate, projection, includePaths);
        }

        /// <summary>
        /// Finds entities satisfied with query command, eager loads related entities and returns entity projections
        /// </summary>
        /// <typeparam name="TR">Entity projection</typeparam>
        /// <param name="queryObject">Query command</param>
        /// <param name="projection">Factory of entity projection</param>
        /// <param name="includePaths">Paths of related entites</param>
        /// <returns>Entity projections</returns>
        public IEnumerable<TR> FindBy<TR>(IQueryCommand<T> queryObject, Expression<Func<T, TR>> projection, params Expression<Func<T, object>>[] includePaths)
        {
            return FindBy(queryObject, projection, includePaths.AsEnumerable());
        }

        /// <summary>
        /// Gets count of entities satisfied with query command
        /// </summary>
        /// <param name="queryObject">Query command</param>
        /// <returns>Count</returns>
        public int CountBy(IQueryCommand<T> queryObject)
        {
            return Set.Count(queryObject.Predicate);
        }

        /// <summary>
        /// Checks if elements satisfied with query are exists
        /// </summary>
        /// <param name="queryObject">Query</param>
        /// <returns>Is any</returns>
        public bool AnyBy(IQueryCommand<T> queryObject)
        {
            return Set.Any(queryObject.Predicate);
        }

        /// <summary>
        /// Gets count of entities satisfied with query predicate
        /// </summary>
        /// <param name="predicate">Query predicate</param>
        /// <returns>Count</returns>
        public int CountBy(Expression<Func<T, bool>> predicate)
        {
            return Set.Count(predicate);
        }

        /// <summary>
        /// Checks if entites satisfied with query predicate are exists
        /// </summary>
        /// <param name="predicate">Query predicate</param>
        /// <returns>Is Any</returns>
        public bool AnyBy(Expression<Func<T, bool>> predicate)
        {
            return Set.Any(predicate);
        }

        /// <summary>
        /// Remove entity
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>Entity</returns>
        public T Remove(T entity)
        {
            return Set.Remove(entity);
        }

        /// <summary>
        /// Remove entity by keyValues
        /// </summary>
        /// <param name="keyValues">Key values</param>
        public void Remove(params object[] keyValues)
        {
            var item = Set.Create();
            var itemType = typeof(T);
            
            var entityContainer = ObjectContext.MetadataWorkspace.GetEntityContainer(ObjectContext.DefaultContainerName, DataSpace.CSpace);
            var entitySetName = entityContainer.BaseEntitySets.First(b => b.ElementType.Name == itemType.Name).Name;
            
            var entityKey = ObjectContext.CreateEntityKey(entitySetName, item);
            var i = 0;
            foreach (var key in entityKey.EntityKeyValues)
            {
                itemType.GetProperty(key.Key).SetValue(item, keyValues[i], null);
                i++;
            }
            
            Set.Attach(item);
            ObjectContext.ObjectStateManager.ChangeObjectState(item, EntityState.Deleted);
        }

        /// <summary>
        /// Remove range of entities
        /// </summary>
        /// <param name="entities">Entities</param>
        /// <returns>Entities</returns>
        public IEnumerable<T> RemoveRange(IEnumerable<T> entities)
        {
            return Set.RemoveRange(entities);
        }

        /// <summary>
        /// Add range of entities
        /// </summary>
        /// <param name="entities">Entities</param>
        /// <returns>Entities</returns>
        public IEnumerable<T> AddRange(IEnumerable<T> entities)
        {
            return Set.AddRange(entities);
        }

        /// <summary>
        /// Finds entities satisfied with predicate
        /// </summary>
        /// <param name="predicate">Query predicate</param>
        /// <returns>Entites</returns>
        public IEnumerable<T> FindBy(Expression<Func<T, bool>> predicate)
        {
            return Set.Where(predicate).ToList();
        }

        /// <summary>
        /// Finds entities satisfied with predicate
        /// </summary>
        /// <typeparam name="TR">Entity projection</typeparam>
        /// <param name="predicate">Query predicate</param>
        /// <param name="projection">Factory of entity projection</param>
        /// <returns>Entites projetions</returns>
        public IEnumerable<TR> FindBy<TR>(Expression<Func<T, bool>> predicate, Expression<Func<T, TR>> projection)
        {
            return Set.Where(predicate).Select(projection).ToList();
        }

        /// <summary>
        /// Finds entities satisfied with predicate, eager loads related entities and returns entity projections
        /// </summary>
        /// <typeparam name="TR">Entity projection</typeparam>
        /// <param name="predicate">Query predicate</param>
        /// <param name="projection">Factory of entity projection</param>
        /// <param name="includePaths">Paths of related entites</param>
        /// <returns>Entites projetions</returns>
        public IEnumerable<TR> FindBy<TR>(Expression<Func<T, bool>> predicate, Expression<Func<T, TR>> projection, IEnumerable<Expression<Func<T, object>>> includePaths)
        {
            var projectionWithIncludes = ExpressionHelper.TranslateToProjectionWithIncludes(includePaths);

            var materializedResults = ObjectSet
                .IncludeToQuery(includePaths)
                .Where(predicate).AsEnumerable()
                .Select(projectionWithIncludes.Compile()).ToList();

            var results = materializedResults
                .Select(materializedResult => materializedResult
                    .Where(x => x.Key == "Entity")
                    .Select(x => (T)x.Value)
                    .Select(projection.Compile())
                    .SingleOrDefault())
                .ToList();

            return results;
        }

        /// <summary>
        /// Finds entities satisfied with predicate, eager loads related entities and returns entity projections
        /// </summary>
        /// <typeparam name="TR">Entity projection</typeparam>
        /// <param name="predicate">Query predicate</param>
        /// <param name="projection">Factory of entity projection</param>
        /// <param name="includePaths">Paths of related entites</param>
        /// <returns>Entites projetions</returns>
        public IEnumerable<TR> FindBy<TR>(Expression<Func<T, bool>> predicate, Expression<Func<T, TR>> projection,
            params Expression<Func<T, object>>[] includePaths)
        {
            return FindBy(predicate, projection, includePaths.AsEnumerable());
        }

        /// <summary>
        /// Adds entity to repository
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>Entity</returns>
        public T Add(T entity)
        {
            return Set.Add(entity);
        }
    }

    public static class QueryObjectsExtenssionsHelper
    {
        public static ObjectQuery<T> IncludeToQuery<T>(this ObjectQuery<T> query, IEnumerable<Expression<Func<T, object>>> includePaths)
        {
            return includePaths.Aggregate(query, (current, path) => (ObjectQuery<T>)current.Include(path));
        }

        public static IQueryable<T> IncludeToQuery<T>(this IQueryable<T> query, IEnumerable<Expression<Func<T, object>>> includePaths)
        {
            return includePaths.Aggregate(query, (current, path) => current.Include(path));
        }

        public static ObjectQuery<T> FilterQueryByKeys<T>(this ObjectQuery<T> objectQuery, IEnumerable<Expression<Func<T, object>>> keys, object[] keyValues)
        {
            var i = 0;
            foreach (var key in keys)
            {
                if (key.Body.NodeType != ExpressionType.Convert)
                    throw new ArgumentException("Entity keys are incorrect");

                var u = (UnaryExpression)key.Body;
                var member = (MemberExpression)u.Operand;
                objectQuery = objectQuery.Where("It." + member.Member.Name + "=" + keyValues[i]);
                i++;
            }

            return objectQuery;
        }
    }

    public static class ExpressionHelper
    {
        public static Expression<Func<T, Dictionary<string, object>>> TranslateToProjectionWithIncludes<T>(IEnumerable<Expression<Func<T, object>>> includePaths)
        {
            var includeFields = includePaths.Select(x => ((MemberExpression)x.Body).Member.Name).ToList();

            var entityType = typeof(T);
            var itemParam = Expression.Parameter(entityType, "x");

            var addMethod = typeof(Dictionary<string, object>).GetMethod(
                "Add", new[] { typeof(string), typeof(object) });

            var elementInits = new List<ElementInit>
            {
                Expression.ElementInit(addMethod, Expression.Constant("Entity"), itemParam)
            };

            elementInits.AddRange(includeFields.Select(field => Expression.ElementInit(addMethod, Expression.Constant(field), Expression.Convert(Expression.PropertyOrField(itemParam, field), typeof(object)))));

            var selector = Expression.ListInit(Expression.New(typeof(Dictionary<string, object>)), elementInits);
            return Expression.Lambda<Func<T, Dictionary<string, object>>>(
                selector, itemParam);

        }
    }





}
