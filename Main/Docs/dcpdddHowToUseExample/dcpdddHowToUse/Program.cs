using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainModel;
using Infrastructure.Data;

namespace dcpdddHowToUse
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var context = new NorthwindEntities())
            {
                context.Configuration.LazyLoadingEnabled = false;

                IOrderRepository orderRepository = new OrderRepository(context);

                //find single result by key
                var order = orderRepository.Find(10248);

                //find single result projection by key
                var orderProjection = orderRepository.Find(10248, projection: x => new
                {
                    OrderId = x.OrderID,
                    CustomerName = x.Customer.ContactName
                });

                //find single result by key with eager loaded Customer association
                order = orderRepository.Find(10248, includePaths: x => x.Customer);

                //find single result projection by key with eager loaded Customer association
                var orderProjection2 = orderRepository.Find(10248, projection: x => new
                {
                    OrderId = x.OrderID,
                    Order = x,
                    CustomerName = x.Customer.ContactName
                }, includePaths: x => x.Customer);

                //find results by predicate
                var orders = orderRepository.FindBy(x => x.ShipCountry == "Russia");

                //find result projections by predicate
                var orderProjections = orderRepository.FindBy(x => x.ShipCountry == "Russia", projection: x => new
                {
                    OrderId = x.OrderID,
                    CustomerName = x.Customer.ContactName
                });

                //find result projections by predicate
                var orderProjections2 = orderRepository.FindBy(x => x.ShipCountry == "Russia", projection: x => new
                {
                    OrderId = x.OrderID,
                    CustomerName = x.Customer.ContactName
                }, includePaths: x => x.Customer);
            }
        }
    }
}
