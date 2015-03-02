using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper;
using dcp.DDD.DomainModel.SuperTypes;

namespace dcp.DDD.Infrastructure.Data.EF.SuperTypes
{
    public abstract class AdaptedRepositoryBase<TData, TDomain> : RepositoryBase<TData>, IRepository<TDomain>
        where TDomain : class
        where TData : class
    {
        protected new readonly DbContext Context;
        protected new readonly DbSet<TData> Set;
        private readonly ReplaceTypeVisitor _replaceTypeVisitor;

        protected AdaptedRepositoryBase(IUnitOfWork unitOfWork, Dictionary<Type, Type> typesMappings, IEnumerable<Expression<Func<TData, object>>> keys) : base(unitOfWork, keys)
        {
            
            _replaceTypeVisitor = new ReplaceTypeVisitor(typesMappings);
            Context = (DbContext)unitOfWork;
            
            Set = Context.Set<TData>();

            foreach (var typesMapping in typesMappings)
            {
                Mapper.CreateMap(typesMapping.Key, typesMapping.Value);
            }
            
        }

        public new IEnumerable<TDomain> GetAll()
        {
            return Mapper.Map<IEnumerable<TData>, IEnumerable<TDomain>>(base.GetAll());
        }

        public new virtual TDomain Find(params object[] keyValues)
        {
            return Mapper.Map<TData, TDomain>(base.Find(keyValues));
        }

        public IEnumerable<TDomain> FindBy(IQueryCommand<TDomain> queryObject)
        {
            var predicateTData = _replaceTypeVisitor.Convert<TDomain, TData, bool>(queryObject.Predicate);

            return Mapper.Map<IEnumerable<TData>, IEnumerable<TDomain>>(FindBy(predicateTData));
        }

        public int CountBy(IQueryCommand<TDomain> queryObject)
        {
            var predicateTData = _replaceTypeVisitor.Convert<TDomain, TData, bool>(queryObject.Predicate);
            return Set.Count(predicateTData);
        }

        public bool AnyBy(IQueryCommand<TDomain> queryObject)
        {
            var predicateTData = _replaceTypeVisitor.Convert<TDomain, TData, bool>(queryObject.Predicate);
            return Set.Any(predicateTData);
        }

        public TDomain Add(TDomain entity)
        {
            var entityTd = Add(Mapper.Map<TDomain, TData>(entity));

            return Mapper.Map<TData, TDomain>(entityTd);
        }

        public IEnumerable<TDomain> FindBy(Expression<Func<TDomain, bool>> predicate)
        {
            var predicateTData = _replaceTypeVisitor.Convert<TDomain, TData, bool>(predicate);

            return Mapper.Map<IEnumerable<TData>, IEnumerable<TDomain>>(FindBy(predicateTData));
        }

        public IEnumerable<TR> FindBy<TR>(Expression<Func<TDomain, bool>> predicate, Expression<Func<TDomain, TR>> projection)
        {
            var predicateTData = _replaceTypeVisitor.Convert<TDomain, TData, bool>(predicate);

            var projectionTData = _replaceTypeVisitor.Convert<TDomain, TData, TR>(projection);
            
            return Set.Where(predicateTData).Select(projectionTData).ToList();
        }

        public IEnumerable<TR> FindBy<TR>(IQueryCommand<TDomain> queryObject, Expression<Func<TDomain, TR>> projection)
        {
            var predicateTData = _replaceTypeVisitor.Convert<TDomain, TData, bool>(queryObject.Predicate);

            var projectionTData = _replaceTypeVisitor.Convert<TDomain, TData, TR>(projection);

            return Set.Where(predicateTData).Select(projectionTData).ToList();
        }

        public int CountBy(Expression<Func<TDomain, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public bool AnyBy(Expression<Func<TDomain, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public TDomain Find(object[] keyValues, IEnumerable<Expression<Func<TDomain, object>>> includePaths)
        {
            throw new NotImplementedException();
        }

        public TDomain Find(object keyValue, params Expression<Func<TDomain, object>>[] includePaths)
        {
            throw new NotImplementedException();
        }

        public TR Find<TR>(object[] keyValues, Expression<Func<TDomain, TR>> projection, IEnumerable<Expression<Func<TDomain, object>>> includePaths)
        {
            throw new NotImplementedException();
        }

        public TR Find<TR>(object keyValue, Expression<Func<TDomain, TR>> projection, params Expression<Func<TDomain, object>>[] includePaths)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TR> FindBy<TR>(IQueryCommand<TDomain> queryObject, Expression<Func<TDomain, TR>> projection, IEnumerable<Expression<Func<TDomain, object>>> includePaths)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TR> FindBy<TR>(IQueryCommand<TDomain> queryObject, Expression<Func<TDomain, TR>> projection, params Expression<Func<TDomain, object>>[] includePaths)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TR> FindBy<TR>(Expression<Func<TDomain, bool>> predicate, Expression<Func<TDomain, TR>> projection, IEnumerable<Expression<Func<TDomain, object>>> includePaths)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TR> FindBy<TR>(Expression<Func<TDomain, bool>> predicate, Expression<Func<TDomain, TR>> projection, params Expression<Func<TDomain, object>>[] includePaths)
        {
            throw new NotImplementedException();
        }
    }
}