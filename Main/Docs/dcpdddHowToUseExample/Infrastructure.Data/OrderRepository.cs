using System;
using System.Linq.Expressions;
using dcp.DDD.DomainModel.SuperTypes;
using dcp.DDD.Infrastructure.Data.EF.SuperTypes;
using DomainModel;

namespace Infrastructure.Data
{
    public class OrderRepository : RepositoryBase<Order>, IOrderRepository
    {
        public OrderRepository(IUnitOfWork unitOfWork) : base(unitOfWork, x => x.OrderID)
        {
        }
    }
}