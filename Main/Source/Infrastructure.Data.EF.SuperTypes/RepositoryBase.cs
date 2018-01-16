using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using dcp.DDD.DomainModel.SuperTypes;
using System.Data.Entity.Core.Metadata.Edm;
using System.Reflection;
using System.Threading.Tasks;

namespace dcp.DDD.Infrastructure.Data.EF.SuperTypes
{
    /// <summary>
    /// Base repository (Entity Framework)
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    public abstract class RepositoryBase<T> : IRepository<T>, IAsyncRepository<T>
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

        #region IRepository

        /// <inheritdoc />
        public IEnumerable<T> GetAll()
        {
            return Set.ToList();
        }

        /// <inheritdoc />
        public virtual T Find(params object[] keyValues)
        {
            return Set.Find(keyValues);
        }

        /// <inheritdoc />
        public T Find(object keyValue, IEnumerable<Expression<Func<T, object>>> includePaths)
        {
            return Find(new[] { keyValue }, includePaths);
        }

        /// <inheritdoc />
        public T Find(object[] keyValues, IEnumerable<Expression<Func<T, object>>> includePaths)
        {
            ObjectQuery<T> objectQuery = ObjectSet;

            return objectQuery
                .IncludeToQuery(includePaths)
                .FilterQueryByKeys(_keys, keyValues)
                .SingleOrDefault();
        }

        /// <inheritdoc />
        public TR Find<TR>(object keyValue, Expression<Func<T, TR>> projection)
        {
            return Find(new[] { keyValue }, projection);
        }

        /// <inheritdoc />
        public TR Find<TR>(object[] keyValues, Expression<Func<T, TR>> projection)
        {
            ObjectQuery<T> objectQuery = ObjectSet;

            return objectQuery
                .FilterQueryByKeys(_keys, keyValues)
                .Select(projection)
                .SingleOrDefault();
        }

        /// <inheritdoc />
        public TR Find<TR>(object keyValue, Expression<Func<T, TR>> projection, Expression<Func<T, object>> includePathsFactory)
        {
            return Find(new[] { keyValue }, projection, includePathsFactory);
        }

        /// <inheritdoc />
        [Obsolete("Bad performance as uses a projection on objects in memory. Use method with includePaths as Anonymous type")]
        public TR Find<TR>(object keyValue, Expression<Func<T, TR>> projection, IEnumerable<Expression<Func<T, object>>> includePaths)
        {
            return Find(new[] { keyValue }, projection, includePaths);
        }

        /// <inheritdoc />
        public TR Find<TR>(object[] keyValues, Expression<Func<T, TR>> projection, Expression<Func<T, object>> includePathsFactory)
        {
            var includePaths = ExpressionHelper.ToStringsIncludePaths(includePathsFactory);
            var projectionWithIncludes = ExpressionHelper.TranslateToProjectionWithIncludesAsAnonymous(projection, includePathsFactory);

            var materializedResult = ObjectSet
                .IncludeToQuery(includePaths)
                .FilterQueryByKeys(_keys, keyValues)
                .Select(projectionWithIncludes).SingleOrDefault();

            if (materializedResult == null)
                return default(TR);

            return materializedResult.Data;
        }

        /// <inheritdoc />
        [Obsolete("Bad performance as uses a projection on objects in memory. Use method with includePaths as Anonymous type")]
        public TR Find<TR>(object[] keyValues, Expression<Func<T, TR>> projection, IEnumerable<Expression<Func<T, object>>> includePaths)
        {
            var includePathsArray = includePaths as Expression<Func<T, object>>[] ?? includePaths.ToArray();
            //improve performance when there is only one include
            if (includePathsArray.Length == 1)
            {
                return Find(keyValues, projection, includePathsArray.First());
            }

            //eager works only when "include" in "projection"
            var projectionWithIncludes = ExpressionHelper.TranslateToProjectionWithIncludes(includePathsArray);

            //TODO: not good performance as projection is not in SQL
            var materializedResult = ObjectSet
                .IncludeToQuery(includePathsArray)
                .FilterQueryByKeys(_keys, keyValues).AsEnumerable()
                .Select(projectionWithIncludes.Compile()).SingleOrDefault();

            if (materializedResult == null)
                return default(TR);

            return materializedResult
                .Where(x => x.Key == "Entity")
                .Select(x => (T)x.Value)
                .Select(projection.Compile()).SingleOrDefault();
        }

        /// <inheritdoc />
        public IEnumerable<T> FindBy(IQueryCommand<T> queryObject)
        {
            return Set
                .Where(queryObject.Predicate)
                .ToList();
        }

        /// <inheritdoc />
        public IEnumerable<TR> FindBy<TR>(IQueryCommand<T> queryObject, Expression<Func<T, TR>> projection)
        {
            return FindBy(queryObject.Predicate, projection);
        }

        /// <inheritdoc />
        public IEnumerable<TR> FindBy<TR>(IQueryCommand<T> queryObject, Expression<Func<T, TR>> projection, Expression<Func<T, object>> includePathsFactory)
        {
            return FindBy(queryObject.Predicate, projection, includePathsFactory);
        }

        /// <inheritdoc />
        [Obsolete("Bad performance as uses a projection on objects in memory. Use method with includePaths as Anonymous type")]
        public IEnumerable<TR> FindBy<TR>(IQueryCommand<T> queryObject, Expression<Func<T, TR>> projection, IEnumerable<Expression<Func<T, object>>> includePaths)
        {
            return FindBy(queryObject.Predicate, projection, includePaths);
        }

        /// <inheritdoc />
        public int CountBy(IQueryCommand<T> queryObject)
        {
            return Set.Count(queryObject.Predicate);
        }

        /// <inheritdoc />
        public bool AnyBy(IQueryCommand<T> queryObject)
        {
            return Set.Any(queryObject.Predicate);
        }

        /// <inheritdoc />
        public IEnumerable<T> FindBy(Expression<Func<T, bool>> predicate)
        {
            return Set.Where(predicate).ToList();
        }

        /// <inheritdoc/>
        public IEnumerable<TR> FindBy<TR>(Expression<Func<T, bool>> predicate, Expression<Func<T, TR>> projection)
        {
            return Set.Where(predicate).Select(projection).ToList();
        }

        /// <inheritdoc/>
        public IEnumerable<TR> FindBy<TR>(Expression<Func<T, bool>> predicate, Expression<Func<T, TR>> projection, Expression<Func<T, object>> includePathsFactory)
        {
            var includePaths = ExpressionHelper.ToStringsIncludePaths(includePathsFactory);
            var projectionWithIncludes = ExpressionHelper.TranslateToProjectionWithIncludesAsAnonymous(projection, includePathsFactory);

            var materializedResult = ObjectSet
                .IncludeToQuery(includePaths)
                .Where(predicate)
                .Select(projectionWithIncludes).ToList();
            
            return materializedResult.Select(x => x.Data).ToList();
        }

        /// <inheritdoc/>
        [Obsolete("Bad performance as uses a projection on objects in memory. Use method with includePaths as Anonymous type")]
        public IEnumerable<TR> FindBy<TR>(Expression<Func<T, bool>> predicate, Expression<Func<T, TR>> projection, IEnumerable<Expression<Func<T, object>>> includePaths)
        {
            var includePathsArray = includePaths as Expression<Func<T, object>>[] ?? includePaths.ToArray();
            //improve performance when there is only one include
            if (includePathsArray.Length == 1)
            {
                return FindBy(predicate, projection, includePathsArray.First());
            }

            //eager works only when "include" in "projection"
            var projectionWithIncludes = ExpressionHelper.TranslateToProjectionWithIncludes(includePathsArray);

            //TODO: not good performance as projection is not in SQL
            var materializedResults = ObjectSet
                .IncludeToQuery(includePathsArray)
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

        /// <inheritdoc />
        public int CountBy(Expression<Func<T, bool>> predicate)
        {
            return Set.Count(predicate);
        }

        /// <inheritdoc />
        public bool AnyBy(Expression<Func<T, bool>> predicate)
        {
            return Set.Any(predicate);
        }

        /// <inheritdoc/>
        public T Add(T entity)
        {
            return Set.Add(entity);
        }

        /// <inheritdoc />
        public IEnumerable<T> AddRange(IEnumerable<T> entities)
        {
            return Set.AddRange(entities);
        }

        /// <inheritdoc />
        public T Remove(T entity)
        {
            return Set.Remove(entity);
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
        public IEnumerable<T> RemoveRange(IEnumerable<T> entities)
        {
            return Set.RemoveRange(entities);
        }

        #endregion

        #region IAsyncRepository

        /// <inheritdoc />
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await Set.ToListAsync();
        }

        /// <inheritdoc />
        public Task<T> FindAsync(params object[] keyValues)
        {
            return Set.FindAsync(keyValues);
        }
       
        /// <inheritdoc />
        public Task<T> FindAsync(object keyValue, IEnumerable<Expression<Func<T, object>>> includePaths)
        {
            return FindAsync(new[] { keyValue }, includePaths);
        }

        /// <inheritdoc />
        public Task<T> FindAsync(object[] keyValues, IEnumerable<Expression<Func<T, object>>> includePaths)
        {
            ObjectQuery<T> objectQuery = ObjectSet;

            return objectQuery
                .IncludeToQuery(includePaths)
                .FilterQueryByKeys(_keys, keyValues)
                .SingleOrDefaultAsync();
        }

        /// <inheritdoc />
        public Task<TR> FindAsync<TR>(object keyValue, Expression<Func<T, TR>> projection)
        {
            return FindAsync(new[] { keyValue }, projection);
        }

        /// <inheritdoc />
        public Task<TR> FindAsync<TR>(object[] keyValues, Expression<Func<T, TR>> projection)
        {
            ObjectQuery<T> objectQuery = ObjectSet;

            return objectQuery
                .FilterQueryByKeys(_keys, keyValues)
                .Select(projection)
                .SingleOrDefaultAsync();
        }

        /// <inheritdoc />
        public Task<TR> FindAsync<TR>(object keyValue, Expression<Func<T, TR>> projection, Expression<Func<T, object>> includePathsFactory)
        {
            return FindAsync(new[] { keyValue }, projection, includePathsFactory);
        }

        /// <inheritdoc />
        [Obsolete("Bad performance as uses a projection on objects in memory. Use method with includePaths as Anonymous type")]
        public Task<TR> FindAsync<TR>(object keyValue, Expression<Func<T, TR>> projection, IEnumerable<Expression<Func<T, object>>> includePaths)
        {
            return FindAsync(new[] { keyValue }, projection, includePaths);
        }

        /// <inheritdoc />
        public async Task<TR> FindAsync<TR>(object[] keyValues, Expression<Func<T, TR>> projection, Expression<Func<T, object>> includePathsFactory)
        {
            var includePaths = ExpressionHelper.ToStringsIncludePaths(includePathsFactory);
            var projectionWithIncludes = ExpressionHelper.TranslateToProjectionWithIncludesAsAnonymous(projection, includePathsFactory);

            var materializedResult = await ObjectSet
                .IncludeToQuery(includePaths)
                .FilterQueryByKeys(_keys, keyValues)
                .Select(projectionWithIncludes).SingleOrDefaultAsync();

            if (materializedResult == null)
                return default(TR);

            return materializedResult.Data;
        }
        

        /// <inheritdoc />
        [Obsolete("Bad performance as uses a projection on objects in memory. Use method with includePaths as Anonymous type")]
        public async Task<TR> FindAsync<TR>(object[] keyValues, Expression<Func<T, TR>> projection, IEnumerable<Expression<Func<T, object>>> includePaths)
        {
            var includePathsArray = includePaths as Expression<Func<T, object>>[] ?? includePaths.ToArray();
            //improve performance when there i only one include
            if (includePathsArray.Length == 1)
            {
                return await FindAsync(keyValues, projection, includePathsArray.First());
            }

            //eager works only when "include" in "projection"
            var projectionWithIncludes = ExpressionHelper.TranslateToProjectionWithIncludes(includePathsArray);

            //TODO: not good performance as projection is not in SQL
            var rawResult = await ObjectSet
                .IncludeToQuery(includePathsArray)
                .FilterQueryByKeys(_keys, keyValues).ToListAsync();

            var materializedResult = rawResult
                .Select(projectionWithIncludes.Compile()).SingleOrDefault();

            if (materializedResult == null)
                return default(TR);

            var res = materializedResult
                .Where(x => x.Key == "Entity")
                .Select(x => (T)x.Value)
                .Select(projection.Compile()).SingleOrDefault();

            return res;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<T>> FindByAsync(IQueryCommand<T> queryObject)
        {
            return await Set
                .Where(queryObject.Predicate)
                .ToListAsync();
        }

        /// <inheritdoc />
        public Task<IEnumerable<TR>> FindByAsync<TR>(IQueryCommand<T> queryObject, Expression<Func<T, TR>> projection)
        {
            return FindByAsync(queryObject.Predicate, projection);
        }

        /// <inheritdoc />
        public Task<IEnumerable<TR>> FindByAsync<TR>(IQueryCommand<T> queryObject, Expression<Func<T, TR>> projection, Expression<Func<T, object>> includePathsFactory)
        {
            return FindByAsync(queryObject.Predicate, projection, includePathsFactory);
        }

        /// <inheritdoc />
        [Obsolete("Bad performance as uses a projection on objects in memory. Use method with includePaths as Anonymous type")]
        public Task<IEnumerable<TR>> FindByAsync<TR>(IQueryCommand<T> queryObject, Expression<Func<T, TR>> projection, IEnumerable<Expression<Func<T, object>>> includePaths)
        {
            return FindByAsync(queryObject.Predicate, projection, includePaths);
        }

        /// <inheritdoc />
        public Task<int> CountByAsync(IQueryCommand<T> queryObject)
        {
            return Set.CountAsync(queryObject.Predicate);
        }

        /// <inheritdoc />
        public Task<bool> AnyByAsync(IQueryCommand<T> queryObject)
        {
            return Set.AnyAsync(queryObject.Predicate);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<T>> FindByAsync(Expression<Func<T, bool>> predicate)
        {
            return await Set.Where(predicate).ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<TR>> FindByAsync<TR>(Expression<Func<T, bool>> predicate, Expression<Func<T, TR>> projection)
        {
            return await Set.Where(predicate).Select(projection).ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<TR>> FindByAsync<TR>(Expression<Func<T, bool>> predicate, Expression<Func<T, TR>> projection, Expression<Func<T, object>> includePathsFactory)
        {
            var includePaths = ExpressionHelper.ToStringsIncludePaths(includePathsFactory);
            var projectionWithIncludes = ExpressionHelper.TranslateToProjectionWithIncludesAsAnonymous(projection, includePathsFactory);

            var materializedResult = await ObjectSet
                .IncludeToQuery(includePaths)
                .Where(predicate)
                .Select(projectionWithIncludes).ToListAsync();

            return materializedResult.Select(x => x.Data).ToList();
        }

        /// <inheritdoc/>
        [Obsolete("Bad performance as uses a projection on objects in memory. Use method with includePaths as Anonymous type")]
        public async Task<IEnumerable<TR>> FindByAsync<TR>(Expression<Func<T, bool>> predicate, Expression<Func<T, TR>> projection, IEnumerable<Expression<Func<T, object>>> includePaths)
        {
            var includePathsArray = includePaths as Expression<Func<T, object>>[] ?? includePaths.ToArray();
            //improve performance when there is only one include
            if (includePathsArray.Length == 1)
            {
                return await FindByAsync(predicate, projection, includePathsArray.First());
            }
            
            //eager works only when "include" in "projection"
            var projectionWithIncludes = ExpressionHelper.TranslateToProjectionWithIncludes(includePathsArray);

            var rawResults = await ObjectSet
                .IncludeToQuery(includePathsArray)
                .Where(predicate).ToListAsync();

            //TODO: not good performance as projection is not in SQL
            var materializedResults = rawResults
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

        /// <inheritdoc />
        public Task<int> CountByAsync(Expression<Func<T, bool>> predicate)
        {
            return Set.CountAsync(predicate);
        }

        /// <inheritdoc />
        public Task<bool> AnyByAsync(Expression<Func<T, bool>> predicate)
        {
            return Set.AnyAsync(predicate);
        }

        #endregion
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

        public static ObjectQuery<T> IncludeToQuery<T>(this ObjectQuery<T> query, IEnumerable<string> includePaths)
        {
            return includePaths.Aggregate(query, (current, path) => current.Include(path));
        }

        public static IQueryable<T> IncludeToQuery<T>(this IQueryable<T> query, IEnumerable<string> includePaths)
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
                objectQuery = objectQuery.Where("It." + member.Member.Name + "=@Key" + i);
                objectQuery.Parameters.Add(new ObjectParameter("Key" + i, keyValues[i]));

                i++;
            }

            return objectQuery;
        }
    }

    internal static class ExpressionHelper
    {
        internal static Expression<Func<T, Dictionary<string, object>>> TranslateToProjectionWithIncludes<T>(IEnumerable<Expression<Func<T, object>>> includePaths)
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

        internal static Expression<Func<T, ProjectionWithIncludes<TR>>> TranslateToProjectionWithIncludesAsAnonymous<T, TR>(Expression<Func<T, TR>> projection, Expression<Func<T, object>> includePathsFactory)
        {
            var entityType = typeof(T);
            var itemParam = Expression.Parameter(entityType, "x");
            
            var projectionType = typeof(ProjectionWithIncludes<TR>);
            var newExp = Expression.New(projectionType);
           
            //replace "x" to use same paramter in two lambda expressions
            var fixedProjection = ParameterReplacer.Replace<Func<T, TR>, Func<T, TR>>(projection, itemParam);
            var fixedIncludePathsFactory = ParameterReplacer.Replace<Func<T, object>, Func<T, object>>(includePathsFactory, itemParam);
            var selector = Expression.MemberInit(newExp,
                Expression.Bind(projectionType.GetProperty("Data"), fixedProjection.Body),
                Expression.Bind(projectionType.GetProperty("Includes"), fixedIncludePathsFactory.Body)
            );

            return Expression.Lambda<Func<T, ProjectionWithIncludes<TR>>>(
                selector, itemParam);
        }

        internal static IEnumerable<string> ToStringsIncludePaths<T>(Expression<Func<T, object>> includePathsFactory)
        {
            if (includePathsFactory.Body is NewExpression)
            {
                var newExp = includePathsFactory.Body as NewExpression;
                var includePaths = new List<string>();
                foreach (var argumentExp in newExp.Arguments)
                {
                    var memberExp = argumentExp as MemberExpression;
                    if (memberExp == null)
                        throw new NotSupportedException("Supported only create instance of anonymous type");
                    includePaths.Add(GetIncludePath(memberExp));
                }

                return includePaths;
            }

            if (includePathsFactory.Body is MemberExpression)
            {
                var memberExp = includePathsFactory.Body as MemberExpression;
                return new[] {GetIncludePath(memberExp)};
            }
            
            throw new NotSupportedException("Supported only create instance of anonymous type or single access member");
        }

        private static string GetIncludePath(MemberExpression memberExpression)
        {
            var path = "";
            if (memberExpression.Expression is MemberExpression)
            {
                path = GetIncludePath((MemberExpression)memberExpression.Expression) + ".";
            }
            var propertyInfo = (PropertyInfo)memberExpression.Member;
            return path + propertyInfo.Name;
        }
    }

    internal class ProjectionWithIncludes<T>
    {
        public T Data { get; set; }
        public object Includes { get; set; }
    }

    internal static class ParameterReplacer
    {
        // Produces an expression identical to 'expression'
        // except with 'source' parameter replaced with 'target' expression.     
        public static Expression<TOutput> Replace<TInput, TOutput>
        (Expression<TInput> expression,
            ParameterExpression target)
        {
            return new ParameterReplacerVisitor<TOutput>(expression.Parameters[0], target)
                .VisitAndConvert(expression);
        }

        private class ParameterReplacerVisitor<TOutput> : ExpressionVisitor
        {
            private readonly ParameterExpression _source;
            private readonly ParameterExpression _target;

            public ParameterReplacerVisitor(ParameterExpression source, ParameterExpression target)
            {
                _source = source;
                _target = target;
            }

            internal Expression<TOutput> VisitAndConvert<T>(Expression<T> root)
            {
                return (Expression<TOutput>)VisitLambda(root);
            }

            protected override Expression VisitLambda<T>(Expression<T> node)
            {
                return Expression.Lambda<TOutput>(Visit(node.Body), _target);
            }

            protected override Expression VisitParameter(ParameterExpression node)
            {
                // Replace the source with the target, visit other params as usual.
                return node == _source ? _target : base.VisitParameter(node);
            }
        }
    }

}
