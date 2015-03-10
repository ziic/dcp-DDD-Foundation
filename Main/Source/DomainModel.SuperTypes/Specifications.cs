using System;
using System.Linq.Expressions;

namespace dcp.DDD.DomainModel.SuperTypes
{
    #region Queryable specifications

    /// <summary>
    /// Contract for Specificaation pattern
    /// </summary>
    /// <typeparam name="T">Entity</typeparam>
    public interface ISpecification<T> : IQueryCommand<T> where T : class
    {
        /// <summary>
        /// Check if entity is satisfied by specification
        /// </summary>
        /// <param name="obj">Entity</param>
        /// <returns>Is satisfied</returns>
        bool IsSatisfiedBy(T obj);

        /// <summary>
        /// Predicate that should be satisfied by the entity 
        /// </summary>
        Expression<Func<T, bool>> SpecExpression { get; }
    }

    /// <summary>
    /// Base type for implementing custom specification
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class SpecificationBase<T> : ISpecification<T> where T : class
    {
        private Func<T, bool> _compiledExpression;

        private Func<T, bool> CompiledExpression
        {
            get { return _compiledExpression ?? (_compiledExpression = SpecExpression.Compile()); }
        }

        public abstract Expression<Func<T, bool>> SpecExpression { get; }

        public bool IsSatisfiedBy(T obj)
        {
            return CompiledExpression(obj);
        }

        public Expression<Func<T, bool>> Predicate
        {
            get { return SpecExpression; }
        }
    }

    public abstract class CompositeSpecificationBase<T> : SpecificationBase<T> where T : class
    {
        private readonly ISpecification<T> _leftExpr;
        private readonly ISpecification<T> _rightExpr;

        protected CompositeSpecificationBase(
            ISpecification<T> left,
            ISpecification<T> right)
        {
            _leftExpr = left;
            _rightExpr = right;
        }

        public ISpecification<T> Left { get { return _leftExpr; } }
        public ISpecification<T> Right { get { return _rightExpr; } }

        public new abstract bool IsSatisfiedBy(T obj);
    }

    public class AndSpecification<T> : CompositeSpecificationBase<T> where T : class
    {
        public AndSpecification(
            ISpecification<T> left,
            ISpecification<T> right)
            : base(left, right)
        {
        }

        public override Expression<Func<T, bool>> SpecExpression
        {
            get
            {
                var objParam = Expression.Parameter(typeof(T), "obj");

                var newExpr = Expression.Lambda<Func<T, bool>>(
                    Expression.AndAlso(
                        Expression.Invoke(Left.SpecExpression, objParam),
                        Expression.Invoke(Right.SpecExpression, objParam)
                    ),
                    objParam
                );

                return newExpr;
            }
        }

        public override bool IsSatisfiedBy(T obj)
        {
            return Left.IsSatisfiedBy(obj) && Right.IsSatisfiedBy(obj);
        }
    }

    public class OrSpecification<T> : CompositeSpecificationBase<T> where T : class
    {
        public OrSpecification(
            ISpecification<T> left,
            ISpecification<T> right)
            : base(left, right)
        {
        }

        public override Expression<Func<T, bool>> SpecExpression
        {
            get
            {
                var objParam = Expression.Parameter(typeof(T), "obj");

                var newExpr = Expression.Lambda<Func<T, bool>>(
                    Expression.OrElse(
                        Expression.Invoke(Left.SpecExpression, objParam),
                        Expression.Invoke(Right.SpecExpression, objParam)
                    ),
                    objParam
                );

                return newExpr;
            }
        }

        public override bool IsSatisfiedBy(T obj)
        {
            return Left.IsSatisfiedBy(obj) || Right.IsSatisfiedBy(obj);
        }
    }

    public class NegatedSpecification<T> : SpecificationBase<T> where T : class
    {
        private readonly ISpecification<T> _inner;

        public NegatedSpecification(ISpecification<T> inner)
        {
            _inner = inner;
        }

        public ISpecification<T> Inner
        {
            get { return _inner; }
        }

        public override Expression<Func<T, bool>> SpecExpression
        {
            get
            {
                var objParam = Expression.Parameter(typeof(T), "obj");

                var newExpr = Expression.Lambda<Func<T, bool>>(
                    Expression.Not(
                        Expression.Invoke(Inner.SpecExpression, objParam)
                    ),
                    objParam
                );

                return newExpr;
            }
        }

        public new bool IsSatisfiedBy(T obj)
        {
            return !_inner.IsSatisfiedBy(obj);
        }
    }

    public static class SpecificationExtensions
    {
        public static ISpecification<T> And<T>(
            this ISpecification<T> left,
            ISpecification<T> right) where T : class
        {
            return new AndSpecification<T>(left, right);
        }

        public static ISpecification<T> Or<T>(
            this ISpecification<T> left,
            ISpecification<T> right) where T : class
        {
            return new OrSpecification<T>(left, right);
        }

        public static ISpecification<T> Negate<T>(this ISpecification<T> inner) where T : class
        {
            return new NegatedSpecification<T>(inner);
        }
    }

    #endregion

    public abstract class AssociationSpecification<TRelated, T> : SpecificationBase<T>
        where TRelated : class
        where T : class
    {
        private readonly IRepository<TRelated> _repository;

        protected AssociationSpecification(IRepository<TRelated> repository)
        {
            _repository = repository;
        }

        public abstract Expression<Func<TRelated, bool>> GetConcretePredicate(T entity);

        public new bool IsSatisfiedBy(T obj)
        {
            return _repository.AnyBy(GetConcretePredicate(obj));
        }
    }
}