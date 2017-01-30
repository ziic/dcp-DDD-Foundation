using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper;
using AutoMapper.Configuration;
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
        private readonly IMapper _mapper;

        protected AdaptedRepositoryBase(IUnitOfWork unitOfWork, Dictionary<Type, Type> typesMappings, IEnumerable<Expression<Func<TData, object>>> keys) 
            : base(unitOfWork, keys.ToArray())
        {
            
            _replaceTypeVisitor = new ReplaceTypeVisitor(typesMappings);
            Context = (DbContext)unitOfWork;
            
            Set = Context.Set<TData>();

            var configurationExpression = new MapperConfigurationExpression();
            
            foreach (var typesMapping in typesMappings)
            {
                configurationExpression.CreateMap(typesMapping.Key, typesMapping.Value);
            }
            var config = new MapperConfiguration(configurationExpression);
            _mapper = config.CreateMapper();

        }

        public new IEnumerable<TDomain> GetAll()
        {
            return _mapper.Map<IEnumerable<TData>, IEnumerable<TDomain>>(base.GetAll());
        }

        public new virtual TDomain Find(params object[] keyValues)
        {
            return _mapper.Map<TData, TDomain>(base.Find(keyValues));
        }

        public IEnumerable<TDomain> FindBy(IQueryCommand<TDomain> queryObject)
        {
            var predicateTData = _replaceTypeVisitor.Convert<TDomain, TData, bool>(queryObject.Predicate);

            return _mapper.Map<IEnumerable<TData>, IEnumerable<TDomain>>(FindBy(predicateTData));
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

            return _mapper.Map<TData, TDomain>(entityTd);
        }

        public IEnumerable<TDomain> AddRange(IEnumerable<TDomain> entities)
        {
            var entitiesDM = _mapper.Map<IEnumerable<TDomain>, IEnumerable<TData>>(entities);
            entitiesDM = AddRange(entitiesDM);

            _mapper.Map(entitiesDM, entities);

            return entities;
        }

        public IEnumerable<TDomain> FindBy(Expression<Func<TDomain, bool>> predicate)
        {
            var predicateTData = _replaceTypeVisitor.Convert<TDomain, TData, bool>(predicate);

            return _mapper.Map<IEnumerable<TData>, IEnumerable<TDomain>>(FindBy(predicateTData));
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
            var predicateTData = _replaceTypeVisitor.Convert<TDomain, TData, bool>(predicate);

            return CountBy(predicateTData);
        }

        public bool AnyBy(Expression<Func<TDomain, bool>> predicate)
        {
            var predicateTData = _replaceTypeVisitor.Convert<TDomain, TData, bool>(predicate);

            return AnyBy(predicateTData);
        }

        public TDomain Remove(TDomain entity)
        {
            var entityTd = Mapper.Map<TDomain, TData>(entity);
            Set.Attach(entityTd);

            entityTd = Set.Remove(entityTd);

            _mapper.Map(entityTd, entity);

            return entity;
        }

        public IEnumerable<TDomain> RemoveRange(IEnumerable<TDomain> entities)
        {
            foreach (var entity in entities)
            {
                Remove(entity);
            }
            return entities;
        }

        public TDomain Find(object[] keyValues, IEnumerable<Expression<Func<TDomain, object>>> includePaths)
        {
            
            throw new NotImplementedException();
        }

        public TDomain Find(object keyValue, IEnumerable<Expression<Func<TDomain, object>>> includePaths)
        {
            throw new NotImplementedException();
        }

        public TR Find<TR>(object[] keyValues, Expression<Func<TDomain, TR>> projection)
        {
            throw new NotImplementedException();
        }

        public TR Find<TR>(object keyValue, Expression<Func<TDomain, TR>> projection)
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