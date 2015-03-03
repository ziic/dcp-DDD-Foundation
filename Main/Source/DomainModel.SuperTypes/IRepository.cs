using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace dcp.DDD.DomainModel.SuperTypes
{
    public interface IRepository<T>
        where T : class
    {
        IEnumerable<T> GetAll();
        
        T Find(params object[] keyValues);
        T Find(object keyValue, params Expression<Func<T, object>>[] includePaths);
        T Find(object[] keyValues, IEnumerable<Expression<Func<T, object>>> includePaths);
        TR Find<TR>(object[] keyValues, Expression<Func<T, TR>> projection, IEnumerable<Expression<Func<T, object>>> includePaths);
        TR Find<TR>(object keyValue, Expression<Func<T, TR>> projection, params Expression<Func<T, object>>[] includePaths);

        IEnumerable<T> FindBy(IQueryCommand<T> queryObject);
        int CountBy(IQueryCommand<T> queryObject);
        bool AnyBy(IQueryCommand<T> queryObject);

        T Add(T entity);
        IEnumerable<T> FindBy(Expression<Func<T, bool>> predicate);
        IEnumerable<TR> FindBy<TR>(Expression<Func<T, bool>> predicate, Expression<Func<T, TR>> projection);
        IEnumerable<TR> FindBy<TR>(IQueryCommand<T> queryObject, Expression<Func<T, TR>> projection);

        int CountBy(Expression<Func<T, bool>> predicate);
        bool AnyBy(Expression<Func<T, bool>> predicate);
        
        IEnumerable<TR> FindBy<TR>(IQueryCommand<T> queryObject, Expression<Func<T, TR>> projection, IEnumerable<Expression<Func<T, object>>> includePaths);
        IEnumerable<TR> FindBy<TR>(IQueryCommand<T> queryObject, Expression<Func<T, TR>> projection, params Expression<Func<T, object>>[] includePaths);
        IEnumerable<TR> FindBy<TR>(Expression<Func<T, bool>> predicate, Expression<Func<T, TR>> projection, IEnumerable<Expression<Func<T, object>>> includePaths);

        IEnumerable<TR> FindBy<TR>(Expression<Func<T, bool>> predicate, Expression<Func<T, TR>> projection,
            params Expression<Func<T, object>>[] includePaths);
    }
}