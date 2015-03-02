using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using dcp.DDD.DomainModel.SuperTypes;

namespace dcp.DDD.Infrastructure.Data.EF.SuperTypes
{
    public abstract class RepositoryBase<T> : IRepository<T>
        where T : class
    {
        private readonly IEnumerable<Expression<Func<T, object>>> _keys;
        protected readonly DbContext Context;
        protected readonly DbSet<T> Set;
        protected readonly ObjectContext ObjectContext;
        protected readonly ObjectSet<T> ObjectSet;

        protected RepositoryBase(IUnitOfWork unitOfWork, IEnumerable<Expression<Func<T, object>>> keys)
        {
            _keys = keys;
            Context = (DbContext)unitOfWork;

            ObjectContext = ((IObjectContextAdapter) unitOfWork).ObjectContext;

            Set = Context.Set<T>();

            ObjectSet = ObjectContext.CreateObjectSet<T>();
        }

        public IEnumerable<T> GetAll()
        {
            return Set.ToList();
        }

        public virtual T Find(params object[] keyValues)
        {
            return Set.Find(keyValues);
        }

        public T Find(object[] keyValues, IEnumerable<Expression<Func<T, object>>> includePaths)
        {
            ObjectQuery<T> objectQuery = ObjectSet;

            return objectQuery
                .IncludeToQuery(includePaths)
                .FilterQueryByKeys(_keys, keyValues)
                .SingleOrDefault();
        }

        public T Find(object keyValue, params Expression<Func<T, object>>[] includePaths)
        {
            return Find(new[] {keyValue}, includePaths.AsEnumerable());
        }

        public TR Find<TR>(object[] keyValues, Expression<Func<T, TR>> projection, IEnumerable<Expression<Func<T, object>>> includePaths)
        {
            ObjectQuery<T> objectQuery = ObjectSet;

            return objectQuery
                .IncludeToQuery(includePaths)
                .FilterQueryByKeys(_keys, keyValues)
                .Select(projection)
                .SingleOrDefault();
        }

        public TR Find<TR>(object keyValue, Expression<Func<T, TR>> projection, params Expression<Func<T, object>>[] includePaths)
        {
            return Find(new[] {keyValue}, projection, includePaths.AsEnumerable());
        }

        public IEnumerable<T> FindBy(IQueryCommand<T> queryObject)
        {
            return Set
                .Where(queryObject.Predicate)
                .ToList();
        }

        public IEnumerable<TR> FindBy<TR>(IQueryCommand<T> queryObject, Expression<Func<T, TR>> projection)
        {
            return FindBy(queryObject.Predicate, projection);
        }

        public IEnumerable<TR> FindBy<TR>(IQueryCommand<T> queryObject, Expression<Func<T, TR>> projection, IEnumerable<Expression<Func<T, object>>> includePaths)
        {
            return FindBy(queryObject.Predicate, projection, includePaths);
        }

        public IEnumerable<TR> FindBy<TR>(IQueryCommand<T> queryObject, Expression<Func<T, TR>> projection, params Expression<Func<T, object>>[] includePaths)
        {
            return FindBy(queryObject, projection, includePaths.AsEnumerable());
        }

        public int CountBy(IQueryCommand<T> queryObject)
        {
            return Set.Count(queryObject.Predicate);
        }

        public bool AnyBy(IQueryCommand<T> queryObject)
        {
            return Set.Any(queryObject.Predicate);
        }

        public int CountBy(Expression<Func<T, bool>> predicate)
        {
            return Set.Count(predicate);
        }

        public bool AnyBy(Expression<Func<T, bool>> predicate)
        {
            return Set.Any(predicate);
        }

        public IEnumerable<T> FindBy(Expression<Func<T, bool>> predicate)
        {
            return Set.Where(predicate).ToList();
        }

        public IEnumerable<TR> FindBy<TR>(Expression<Func<T, bool>> predicate, Expression<Func<T, TR>> projection)
        {
            return Set.Where(predicate).Select(projection).ToList();
        }

        public IEnumerable<TR> FindBy<TR>(Expression<Func<T, bool>> predicate, Expression<Func<T, TR>> projection, IEnumerable<Expression<Func<T, object>>> includePaths)
        {
            return Set
                .IncludeToQuery(includePaths)
                .Where(predicate)
                .Select(projection)
                .ToList();
        }

        public IEnumerable<TR> FindBy<TR>(Expression<Func<T, bool>> predicate, Expression<Func<T, TR>> projection,
            params Expression<Func<T, object>>[] includePaths)
        {
            return FindBy(predicate, projection, includePaths.AsEnumerable());
        }

        public T Add(T entity)
        {
            return Set.Add(entity);
        }
    }

    public static class QueryObjectsExtenssionsHelper
    {
        public static ObjectQuery<T> IncludeToQuery<T>(this ObjectQuery<T> query, IEnumerable<Expression<Func<T, object>>> includePaths)
        {
            return includePaths.Aggregate(query, (current, path) => (ObjectQuery<T>) current.Include(path));
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
                objectQuery = objectQuery.Where("It." + key.Name + "=" + keyValues[i]);
                i++;
            }

            return objectQuery;
        }
    }
}
