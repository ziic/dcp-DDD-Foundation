using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using DomainModel;
using Infrastructure.Data;
using dcp.DDD.Infrastructure.Data.EF.SuperTypes;

namespace dcpdddHowToUse
{
    class Program
    {
        static void Main(string[] args)
        {
            // use methods below ...
        }
        
        public void FindSingleResult()
        {
            using (var context = new NorthwindEntities())
            {
                context.Configuration.LazyLoadingEnabled = false;

                var orderRepository = new OrderRepository(context);
                var order = orderRepository.Find(10248);

         
            }
        }

        public void FindSingleResultWithInclude()
        {
            using (var context = new NorthwindEntities())
            {
                context.Configuration.LazyLoadingEnabled = false;

                var orderRepository = new OrderRepository(context);

                //invoke T Find(object keyValue, params Expression<Func<T, object>>[] includePaths) with several include paths
                var res = orderRepository.Find(10248, new Expression<Func<Order, object>>[]
                {
                    x => x.Customer,
                    x => x.Employee,
                    x => x.Customer.CustomerDemographics
                });
                
            }

            using (var context = new NorthwindEntities())
            {
                context.Configuration.LazyLoadingEnabled = false;

                var orderRepository = new OrderRepository(context);

                //invoke shorter T Find(object keyValue, params Expression<Func<T, object>>[] includePaths) with single include path
                var res = orderRepository.Find(10248, includePaths: x => x.Customer);

            }

            using (var context = new NorthwindEntities())
            {
                context.Configuration.LazyLoadingEnabled = false;

                var orderRepository = new OrderRepository(context);
                
                //invoke more usuful extenssion method with several include path
                var res = orderRepository.FindOnlyWithIncludes(10248, x => x.Customer, x => x.Employee);
            }

            using (var context = new NorthwindEntities())
            {
                context.Configuration.LazyLoadingEnabled = false;

                var orderRepository = new OrderRepository(context);

                //invoke shorter T Find(object keyValue, IEnumerable<Expression<Func<T, object>>> includePaths) with include paths
                var res = orderRepository.Find(10248, new List<Expression<Func<Order, object>>>
                {
                    x => x.Customer,
                    x => x.Employee
                });
               
            }
        }
        
        public void FindSingleResultProjection()
        {
            using (var context = new NorthwindEntities())
            {
                context.Configuration.LazyLoadingEnabled = false;

                var orderRepository = new OrderRepository(context);

                //invoke TR Find<TR>(object keyValue, Expression<Func<T, TR>> projection)
                var orderProjection = orderRepository.Find(10248, x => new
                {
                    OrderId = x.OrderID,
                    CustomerName = x.Customer.ContactName
                });
            }
        }
        
        public void FindSingleResultProjectionWithInclude()
        {
            //TR Find<TR>(object keyValue, Expression<Func<T, TR>> projection, params Expression<Func<T, object>>[] includePaths)
            using (var context = new NorthwindEntities())
            {
                context.Configuration.LazyLoadingEnabled = false;
                
                var orderRepository = new OrderRepository(context);
                var orderProjection = orderRepository.Find(10248, x => new
                {
                    OrderId = x.OrderID,
                    Order = x
                }, x => x.Customer, x => x.Employee);
         
            }

        }
        
        public void FindResultsBySpecification()
        {
            using (var context = new NorthwindEntities())
            {
                context.Configuration.LazyLoadingEnabled = false;

                var orderRepository = new OrderRepository(context);
                var results = orderRepository.FindBy(new UsaOrderSpecification());
            }
        }
        
        public void FindProjectionResultsBySpecification()
        {
            using (var context = new NorthwindEntities())
            {
                context.Configuration.LazyLoadingEnabled = false;

                var orderRepository = new OrderRepository(context);
                var results = orderRepository.FindBy(new UsaOrderSpecification(), x => new
                {
                    Order = x
                });
        
            }
        }
        
        public void FindProjectionResultsBySpecificationWithIncludes()
        {
            using (var context = new NorthwindEntities())
            {
                context.Configuration.LazyLoadingEnabled = false;

                var orderRepository = new OrderRepository(context);
                var results = orderRepository.FindBy(new UsaOrderSpecification(), x => new
                {
                    Order = x
                }, x => x.Customer, x => x.Employee);
                
            }
        }
        
        public void CountBySpecification()
        {
            using (var context = new NorthwindEntities())
            {
                context.Configuration.LazyLoadingEnabled = false;

                var orderRepository = new OrderRepository(context);
                var res = orderRepository.CountBy(new UsaOrderSpecification());
                
            }
        }
        
        public void AnyBySpecification()
        {
            using (var context = new NorthwindEntities())
            {
                context.Configuration.LazyLoadingEnabled = false;

                var orderRepository = new OrderRepository(context);
                var res = orderRepository.AnyBy(new UsaOrderSpecification());

         
            }
        }
        
        public void FindResultsByPredicate()
        {
            using (var context = new NorthwindEntities())
            {
                context.Configuration.LazyLoadingEnabled = false;

                var orderRepository = new OrderRepository(context);
                var orders = orderRepository.FindBy(x => x.ShipCountry == "USA");
                
            }
        }

       
        public void FindResultProjectionsByPredicate()
        {
            using (var context = new NorthwindEntities())
            {
                context.Configuration.LazyLoadingEnabled = false;

                var orderRepository = new OrderRepository(context);
                var orderProjections = orderRepository.FindBy(x => x.ShipCountry == "USA", x => new
                {
                    Order = x
                });
            }
        }
        
        public void FindResultProjectionsByPredicateWithIncludes()
        {
            using (var context = new NorthwindEntities())
            {
                context.Configuration.LazyLoadingEnabled = false;

                var orderRepository = new OrderRepository(context);
                var orderProjections = orderRepository.FindBy(x => x.ShipCountry == "USA", x => new
                {
                    Order = x
                }, x => x.Customer, x => x.Employee);
            }
        }
        
        public void CountByPredicate()
        {
            using (var context = new NorthwindEntities())
            {
                context.Configuration.LazyLoadingEnabled = false;

                var orderRepository = new OrderRepository(context);
                var res = orderRepository.CountBy(x => x.ShipCountry == "USA");

                
            }
        }
        
        public void AnyByPredicate()
        {
            using (var context = new NorthwindEntities())
            {
                context.Configuration.LazyLoadingEnabled = false;

                var orderRepository = new OrderRepository(context);
                var res = orderRepository.AnyBy(x => x.ShipCountry == "USA");
        
            }
        }
    }
}
