using System;
using System.Linq.Expressions;
using dcp.DDD.DomainModel.SuperTypes;
using dcp.DDD.Infrastructure.Data.EF.SuperTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Infrastructure.Data.Tests
{
    [TestClass]
    public class BaseRepositoryTests
    {
        [TestMethod]
        public void Include()
        {
            using (var context = new NorthwindEntities())
            {
                context.Configuration.LazyLoadingEnabled = false;

                var orderRepository = new OrderRepository(context);
                var order = orderRepository.Find(10248, includePaths: x => x.Customer);

                Assert.IsNotNull(order.Customer);
            }
            
        }
    }

    public class OrderRepository : RepositoryBase<Order>
    {
        public OrderRepository(IUnitOfWork unitOfWork) : base(unitOfWork, x => x.OrderID)
        {
        }
    }

    public partial class NorthwindEntities : IUnitOfWork
    {
        public void Commit()
        {
            SaveChanges();
        }
    }
}
