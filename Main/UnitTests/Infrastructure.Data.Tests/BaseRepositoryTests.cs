using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using dcp.DDD.DomainModel.SuperTypes;
using dcp.DDD.Infrastructure.Data.EF.SuperTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data.Tests
{
    [TestClass]
    public class RepositoryBaseTests
    {
        [TestClass]
        public class Sync
        {
            [TestMethod]
            public void FindSingleResult()
            {
                using (var context = new NorthwindEntities())
                {
                    context.Configuration.LazyLoadingEnabled = false;

                    var orderRepository = new OrderRepository(context);

                    var order = orderRepository.Find(10248);

                    Assert.IsNotNull(order);
                }
            }

            [TestMethod]
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

                    Assert.IsInstanceOfType(res, typeof(Order));
                    Assert.IsNotNull(res.Customer);
                    Assert.IsNotNull(res.Employee);
                    Assert.IsNotNull(res.Customer.CustomerDemographics);
                }

                using (var context = new NorthwindEntities())
                {
                    context.Configuration.LazyLoadingEnabled = false;

                    var orderRepository = new OrderRepository(context);

                    //invoke shorter T Find(object keyValue, params Expression<Func<T, object>>[] includePaths) with single include path
                    var res = orderRepository.Find(10248, includePaths: x => x.Customer);

                    Assert.IsInstanceOfType(res, typeof(Order));
                    Assert.IsNotNull(res.Customer);
                }

                using (var context = new NorthwindEntities())
                {
                    context.Configuration.LazyLoadingEnabled = false;

                    var orderRepository = new OrderRepository(context);

                    //invoke more usuful extenssion method with several include path
                    var res = orderRepository.FindOnlyWithIncludes(10248, x => x.Customer, x => x.Employee);

                    Assert.IsInstanceOfType(res, typeof(Order));
                    Assert.IsNotNull(res.Customer);
                    Assert.IsNotNull(res.Employee);
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

                    Assert.IsInstanceOfType(res, typeof(Order));
                    Assert.IsNotNull(res.Customer);
                    Assert.IsNotNull(res.Employee);
                }
            }

            [TestMethod]
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

                    Assert.IsNotNull(orderProjection);
                }
            }

            [TestMethod]
            public void FindSingleResultProjectionWithManyIncludes()
            {
                //TR Find<TR>(object keyValue, Expression<Func<T, TR>> projection, params Expression<Func<T, object>>[] includePaths)
                using (var context = new NorthwindEntities())
                {
                    context.Configuration.LazyLoadingEnabled = false;
                    var logger = new StringBuilder();
                    Action<string> log = x => logger.AppendLine(x);
                    context.Database.Log = log;
                    var orderRepository = new OrderRepository(context);
                    var orderProjection = orderRepository.Find(10248, x => new {
                        OrderId = x.OrderID,
                        Order = x
                    }, x => x.Customer, x => x.Employee);

                    System.Diagnostics.Debug.Write(logger.ToString());

                    Assert.IsNotNull(orderProjection.Order.Customer);
                    Assert.IsNotNull(orderProjection.Order.Employee);
                }
            }

            [TestMethod]
            public void FindSingleResultProjectionWithSingleInclude()
            {
                using (var context = new NorthwindEntities())
                {
                    context.Configuration.LazyLoadingEnabled = false;

                    var orderRepository = new OrderRepository(context);
                    var orderProjection = orderRepository.Find(10248, x => new
                    {
                        Order = x
                    }, new Expression<Func<Order, object>>[] { x => x.Customer });
                    Assert.IsNotNull(orderProjection.Order.Customer);
                }
            }

            [TestMethod]
            public void FindSingleResultProjectionWithFactoryManyIncludes()
            {
                using (var context = new NorthwindEntities())
                {
                    context.Configuration.LazyLoadingEnabled = false;

                    var orderRepository = new OrderRepository(context);
                    var orderProjection = orderRepository.Find(10248, x => new
                    {
                        Order = x
                    }, x => new { x.Customer, x.Employee });

                    Assert.IsNotNull(orderProjection.Order.Customer);
                    Assert.IsNotNull(orderProjection.Order.Employee);
                }
            }

            [TestMethod]
            public void FindSingleResultProjectionWithFactorySingleInclude()
            {
                using (var context = new NorthwindEntities())
                {
                    context.Configuration.LazyLoadingEnabled = false;

                    var orderRepository = new OrderRepository(context);
                    var orderProjection = orderRepository.Find(10248, x => new
                    {
                        Order = x
                    }, x => x.Customer);

                    Assert.IsNotNull(orderProjection.Order.Customer);
                }
            }

            [TestMethod]
            public void FindResultsBySpecification()
            {
                using (var context = new NorthwindEntities())
                {
                    context.Configuration.LazyLoadingEnabled = false;

                    var orderRepository = new OrderRepository(context);
                    var results = orderRepository.FindBy(new UsaOrderSpecification());

                    Assert.AreEqual(122, results.Count());
                }
            }

            [TestMethod]
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

                    Assert.AreEqual(122, results.Count());
                }
            }

            [TestMethod]
            public void FindProjectionResultsBySpecificationWithManyIncludes()
            {
                using (var context = new NorthwindEntities())
                {
                    context.Configuration.LazyLoadingEnabled = false;

                    var orderRepository = new OrderRepository(context);
                    var results = orderRepository.FindBy(new UsaOrderSpecification(), x => new
                    {
                        Order = x
                    }, x => x.Customer, x => x.Employee);

                    Assert.AreEqual(122, results.Count());
                    foreach (var result in results)
                    {
                        if (string.IsNullOrEmpty(result.Order.CustomerID))
                            Assert.IsNotNull(result.Order.Customer);

                        if (result.Order.EmployeeID.HasValue)
                            Assert.IsNotNull(result.Order.Employee);
                    }
                }
            }

            [TestMethod]
            public void FindProjectionResultsBySpecificationWithOneInclude()
            {
                using (var context = new NorthwindEntities())
                {
                    context.Configuration.LazyLoadingEnabled = false;

                    var orderRepository = new OrderRepository(context);
                    var results = orderRepository.FindBy(new UsaOrderSpecification(), x => new
                    {
                        Order = x
                    }, new Expression<Func<Order, object>>[] { x => x.Customer });

                    Assert.AreEqual(122, results.Count());
                    foreach (var result in results)
                    {
                        if (string.IsNullOrEmpty(result.Order.CustomerID))
                            Assert.IsNotNull(result.Order.Customer);
                    }
                }
            }

            [TestMethod]
            public void FindProjectionResultsBySpecificationWithFactoryManyIncludes()
            {
                using (var context = new NorthwindEntities())
                {
                    context.Configuration.LazyLoadingEnabled = false;

                    var orderRepository = new OrderRepository(context);
                    var results = orderRepository.FindBy(new UsaOrderSpecification(), x => new
                    {
                        Order = x
                    }, x => new { x.Customer, x.Employee });

                    Assert.AreEqual(122, results.Count());
                    foreach (var result in results)
                    {
                        if (string.IsNullOrEmpty(result.Order.CustomerID))
                            Assert.IsNotNull(result.Order.Customer);

                        if (result.Order.EmployeeID.HasValue)
                            Assert.IsNotNull(result.Order.Employee);
                    }
                }
            }

            [TestMethod]
            public void FindProjectionResultsBySpecificationWithFactoryOneInclude()
            {
                using (var context = new NorthwindEntities())
                {
                    context.Configuration.LazyLoadingEnabled = false;

                    var orderRepository = new OrderRepository(context);
                    var results = orderRepository.FindBy(new UsaOrderSpecification(), x => new
                    {
                        Order = x
                    }, x => x.Customer);

                    Assert.AreEqual(122, results.Count());
                    foreach (var result in results)
                    {
                        if (string.IsNullOrEmpty(result.Order.CustomerID))
                            Assert.IsNotNull(result.Order.Customer);
                    }
                }
            }

            [TestMethod]
            public void CountBySpecification()
            {
                using (var context = new NorthwindEntities())
                {
                    context.Configuration.LazyLoadingEnabled = false;

                    var orderRepository = new OrderRepository(context);
                    var res = orderRepository.CountBy(new UsaOrderSpecification());

                    Assert.AreEqual(122, res);
                }
            }

            [TestMethod]
            public void AnyBySpecification()
            {
                using (var context = new NorthwindEntities())
                {
                    context.Configuration.LazyLoadingEnabled = false;

                    var orderRepository = new OrderRepository(context);
                    var res = orderRepository.AnyBy(new UsaOrderSpecification());

                    Assert.IsTrue(res);
                }
            }

            [TestMethod]
            public void FindResultsByPredicate()
            {
                using (var context = new NorthwindEntities())
                {
                    context.Configuration.LazyLoadingEnabled = false;

                    var orderRepository = new OrderRepository(context);
                    var orders = orderRepository.FindBy(x => x.ShipCountry == "USA");

                    Assert.AreEqual(122, orders.Count());
                }
            }

            [TestMethod]
            public void FindResultProjectionsByPredicate()
            {
                using (var context = new NorthwindEntities())
                {
                    context.Configuration.LazyLoadingEnabled = false;

                    var orderRepository = new OrderRepository(context);
                    var orderProjections = orderRepository.FindBy(x => x.ShipCountry == "USA", x => new {
                        Order = x
                    });

                    Assert.AreEqual(122, orderProjections.Count());
                }
            }

            [TestMethod]
            public void FindResultProjectionsByPredicateWithManyIncludes()
            {
                using (var context = new NorthwindEntities())
                {
                    context.Configuration.LazyLoadingEnabled = false;

                    var orderRepository = new OrderRepository(context);
                    var orderProjections = orderRepository.FindBy(x => x.ShipCountry == "USA", x => new
                    {
                        Order = x
                    }, x => x.Customer, x => x.Employee);

                    Assert.AreEqual(122, orderProjections.Count());
                    foreach (var result in orderProjections)
                    {
                        if (!string.IsNullOrEmpty(result.Order.CustomerID))
                            Assert.IsNotNull(result.Order.Customer);

                        if (result.Order.EmployeeID.HasValue)
                            Assert.IsNotNull(result.Order.Employee);
                    }
                }
            }

            [TestMethod]
            public void FindResultProjectionsByPredicateWithOneInclude()
            {
                using (var context = new NorthwindEntities())
                {
                    context.Configuration.LazyLoadingEnabled = false;

                    var orderRepository = new OrderRepository(context);
                    var orderProjections = orderRepository.FindBy(x => x.ShipCountry == "USA", x => new
                    {
                        Order = x
                    }, new Expression<Func<Order, object>>[] { x => x.Customer });

                    Assert.AreEqual(122, orderProjections.Count());
                    foreach (var result in orderProjections)
                    {
                        if (!string.IsNullOrEmpty(result.Order.CustomerID))
                            Assert.IsNotNull(result.Order.Customer);
                    }
                }
            }

            [TestMethod]
            public void FindResultProjectionsByPredicateWithFactoryManyIncludes()
            {
                using (var context = new NorthwindEntities())
                {
                    context.Configuration.LazyLoadingEnabled = false;

                    var orderRepository = new OrderRepository(context);
                    var orderProjections = orderRepository.FindBy(x => x.ShipCountry == "USA", x => new
                    {
                        Order = x
                    }, x => new { x.Customer, x.Employee });

                    Assert.AreEqual(122, orderProjections.Count());
                    foreach (var result in orderProjections)
                    {
                        if (!string.IsNullOrEmpty(result.Order.CustomerID))
                            Assert.IsNotNull(result.Order.Customer);

                        if (result.Order.EmployeeID.HasValue)
                            Assert.IsNotNull(result.Order.Employee);
                    }
                }
            }

            [TestMethod]
            public void FindResultProjectionsByPredicateWithFactoryOneInclude()
            {
                using (var context = new NorthwindEntities())
                {
                    context.Configuration.LazyLoadingEnabled = false;

                    var orderRepository = new OrderRepository(context);
                    var orderProjections = orderRepository.FindBy(x => x.ShipCountry == "USA", x => new
                    {
                        Order = x
                    }, x => x.Customer);

                    Assert.AreEqual(122, orderProjections.Count());
                    foreach (var result in orderProjections)
                    {
                        if (!string.IsNullOrEmpty(result.Order.CustomerID))
                            Assert.IsNotNull(result.Order.Customer);
                    }
                }
            }


            [TestMethod]
            public void CountByPredicate()
            {
                using (var context = new NorthwindEntities())
                {
                    context.Configuration.LazyLoadingEnabled = false;

                    var orderRepository = new OrderRepository(context);
                    var res = orderRepository.CountBy(x => x.ShipCountry == "USA");

                    Assert.AreEqual(122, res);
                }
            }

            [TestMethod]
            public void AnyByPredicate()
            {
                using (var context = new NorthwindEntities())
                {
                    context.Configuration.LazyLoadingEnabled = false;

                    var orderRepository = new OrderRepository(context);
                    var res = orderRepository.AnyBy(x => x.ShipCountry == "USA");

                    Assert.IsTrue(res);
                }
            }

            [TestMethod]
            [Ignore]
            public void RemoveByKey()
            {
                using (var context = new NorthwindEntities())
                {
                    context.Configuration.LazyLoadingEnabled = false;

                    var orderRepository = new OrderRepository(context);

                    orderRepository.Remove(10248);
                }
            }
        }

        [TestClass]
        public class Async
        {
            [TestMethod]
            public async Task FindSingleResult()
            {
                using (var context = new NorthwindEntities())
                {
                    context.Configuration.LazyLoadingEnabled = false;

                    var orderRepository = new OrderRepository(context);

                    var order = await orderRepository.FindAsync(10248);

                    Assert.IsNotNull(order);
                }
            }

            [TestMethod]
            public async Task FindSingleResultWithInclude()
            {
                using (var context = new NorthwindEntities())
                {
                    context.Configuration.LazyLoadingEnabled = false;

                    var orderRepository = new OrderRepository(context);

                    //invoke T Find(object keyValue, params Expression<Func<T, object>>[] includePaths) with several include paths
                    var res = await orderRepository.FindAsync(10248, new Expression<Func<Order, object>>[]
                    {
                    x => x.Customer,
                    x => x.Employee,
                    x => x.Customer.CustomerDemographics
                    });

                    Assert.IsInstanceOfType(res, typeof(Order));
                    Assert.IsNotNull(res.Customer);
                    Assert.IsNotNull(res.Employee);
                    Assert.IsNotNull(res.Customer.CustomerDemographics);
                }

                using (var context = new NorthwindEntities())
                {
                    context.Configuration.LazyLoadingEnabled = false;

                    var orderRepository = new OrderRepository(context);

                    //invoke shorter T Find(object keyValue, params Expression<Func<T, object>>[] includePaths) with single include path
                    var res = await orderRepository.FindAsync(10248, includePaths: x => x.Customer);

                    Assert.IsInstanceOfType(res, typeof(Order));
                    Assert.IsNotNull(res.Customer);
                }

                using (var context = new NorthwindEntities())
                {
                    context.Configuration.LazyLoadingEnabled = false;

                    var orderRepository = new OrderRepository(context);

                    //invoke more usuful extenssion method with several include path
                    var res = await orderRepository.FindOnlyWithIncludesAsync(10248, x => x.Customer, x => x.Employee);

                    Assert.IsInstanceOfType(res, typeof(Order));
                    Assert.IsNotNull(res.Customer);
                    Assert.IsNotNull(res.Employee);
                }

                using (var context = new NorthwindEntities())
                {
                    context.Configuration.LazyLoadingEnabled = false;

                    var orderRepository = new OrderRepository(context);

                    //invoke shorter T Find(object keyValue, IEnumerable<Expression<Func<T, object>>> includePaths) with include paths
                    var res = await orderRepository.FindAsync(10248, new List<Expression<Func<Order, object>>>
                {
                    x => x.Customer,
                    x => x.Employee
                });

                    Assert.IsInstanceOfType(res, typeof(Order));
                    Assert.IsNotNull(res.Customer);
                    Assert.IsNotNull(res.Employee);
                }
            }

            [TestMethod]
            public async Task FindSingleResultProjection()
            {
                using (var context = new NorthwindEntities())
                {
                    context.Configuration.LazyLoadingEnabled = false;

                    var orderRepository = new OrderRepository(context);

                    //invoke TR Find<TR>(object keyValue, Expression<Func<T, TR>> projection)
                    var orderProjection = await orderRepository.FindAsync(10248, x => new
                    {
                        OrderId = x.OrderID,
                        CustomerName = x.Customer.ContactName
                    });

                    Assert.IsNotNull(orderProjection);
                }
            }

            [TestMethod]
            public async Task FindSingleResultProjectionWithManyIncludes()
            {
                //TR Find<TR>(object keyValue, Expression<Func<T, TR>> projection, params Expression<Func<T, object>>[] includePaths)
                using (var context = new NorthwindEntities())
                {
                    context.Configuration.LazyLoadingEnabled = false;
                    var logger = new StringBuilder();
                    Action<string> log = x => logger.AppendLine(x);
                    context.Database.Log = log;
                    var orderRepository = new OrderRepository(context);
                    var orderProjection = await orderRepository.FindAsync(10248, x => new {
                        OrderId = x.OrderID,
                        Order = x
                    }, x => x.Customer, x => x.Employee);

                    System.Diagnostics.Debug.Write(logger.ToString());

                    Assert.IsNotNull(orderProjection.Order.Customer);
                    Assert.IsNotNull(orderProjection.Order.Employee);
                }
            }

            [TestMethod]
            public async Task FindSingleResultProjectionWithSingleInclude()
            {
                using (var context = new NorthwindEntities())
                {
                    context.Configuration.LazyLoadingEnabled = false;

                    var orderRepository = new OrderRepository(context);
                    var orderProjection = await orderRepository.FindAsync(10248, x => new
                    {
                        Order = x
                    }, new Expression<Func<Order, object>>[] { x => x.Customer });
                    Assert.IsNotNull(orderProjection.Order.Customer);
                }
            }

            [TestMethod]
            public async Task FindSingleResultProjectionWithFactoryManyIncludes()
            {
                using (var context = new NorthwindEntities())
                {
                    context.Configuration.LazyLoadingEnabled = false;

                    var orderRepository = new OrderRepository(context);
                    var orderProjection = await orderRepository.FindAsync(10248, x => new
                    {
                        Order = x
                    }, x => new { x.Customer, x.Employee });

                    Assert.IsNotNull(orderProjection.Order.Customer);
                    Assert.IsNotNull(orderProjection.Order.Employee);
                }
            }

            [TestMethod]
            public async Task FindSingleResultProjectionWithFactorySingleInclude()
            {
                using (var context = new NorthwindEntities())
                {
                    context.Configuration.LazyLoadingEnabled = false;

                    var orderRepository = new OrderRepository(context);
                    var orderProjection = await orderRepository.FindAsync(10248, x => new
                    {
                        Order = x
                    }, x => x.Customer);

                    Assert.IsNotNull(orderProjection.Order.Customer);
                }
            }

            [TestMethod]
            public async Task FindResultsBySpecification()
            {
                using (var context = new NorthwindEntities())
                {
                    context.Configuration.LazyLoadingEnabled = false;

                    var orderRepository = new OrderRepository(context);
                    var results = await orderRepository.FindByAsync(new UsaOrderSpecification());

                    Assert.AreEqual(122, results.Count());
                }
            }

            [TestMethod]
            public async Task FindProjectionResultsBySpecification()
            {
                using (var context = new NorthwindEntities())
                {
                    context.Configuration.LazyLoadingEnabled = false;

                    var orderRepository = new OrderRepository(context);
                    var results = await orderRepository.FindByAsync(new UsaOrderSpecification(), x => new
                    {
                        Order = x
                    });

                    Assert.AreEqual(122, results.Count());
                }
            }

            [TestMethod]
            public async Task FindProjectionResultsBySpecificationWithManyIncludes()
            {
                using (var context = new NorthwindEntities())
                {
                    context.Configuration.LazyLoadingEnabled = false;

                    var orderRepository = new OrderRepository(context);
                    var results = await orderRepository.FindByAsync(new UsaOrderSpecification(), x => new
                    {
                        Order = x
                    }, x => x.Customer, x => x.Employee);

                    Assert.AreEqual(122, results.Count());
                    foreach (var result in results)
                    {
                        if (string.IsNullOrEmpty(result.Order.CustomerID))
                            Assert.IsNotNull(result.Order.Customer);

                        if (result.Order.EmployeeID.HasValue)
                            Assert.IsNotNull(result.Order.Employee);
                    }
                }
            }

            [TestMethod]
            public async Task FindProjectionResultsBySpecificationWithOneInclude()
            {
                using (var context = new NorthwindEntities())
                {
                    context.Configuration.LazyLoadingEnabled = false;

                    var orderRepository = new OrderRepository(context);
                    var results = await orderRepository.FindByAsync(new UsaOrderSpecification(), x => new
                    {
                        Order = x
                    }, new Expression<Func<Order, object>>[] { x => x.Customer });

                    Assert.AreEqual(122, results.Count());
                    foreach (var result in results)
                    {
                        if (string.IsNullOrEmpty(result.Order.CustomerID))
                            Assert.IsNotNull(result.Order.Customer);
                    }
                }
            }

            [TestMethod]
            public async Task FindProjectionResultsBySpecificationWithFactoryManyIncludes()
            {
                using (var context = new NorthwindEntities())
                {
                    context.Configuration.LazyLoadingEnabled = false;

                    var orderRepository = new OrderRepository(context);
                    var results = await orderRepository.FindByAsync(new UsaOrderSpecification(), x => new
                    {
                        Order = x
                    }, x => new { x.Customer, x.Employee });

                    Assert.AreEqual(122, results.Count());
                    foreach (var result in results)
                    {
                        if (string.IsNullOrEmpty(result.Order.CustomerID))
                            Assert.IsNotNull(result.Order.Customer);

                        if (result.Order.EmployeeID.HasValue)
                            Assert.IsNotNull(result.Order.Employee);
                    }
                }
            }

            [TestMethod]
            public async Task FindProjectionResultsBySpecificationWithFactoryOneInclude()
            {
                using (var context = new NorthwindEntities())
                {
                    context.Configuration.LazyLoadingEnabled = false;

                    var orderRepository = new OrderRepository(context);
                    var results = await orderRepository.FindByAsync(new UsaOrderSpecification(), x => new
                    {
                        Order = x
                    }, x => x.Customer);

                    Assert.AreEqual(122, results.Count());
                    foreach (var result in results)
                    {
                        if (string.IsNullOrEmpty(result.Order.CustomerID))
                            Assert.IsNotNull(result.Order.Customer);
                    }
                }
            }

            [TestMethod]
            public async Task CountBySpecification()
            {
                using (var context = new NorthwindEntities())
                {
                    context.Configuration.LazyLoadingEnabled = false;

                    var orderRepository = new OrderRepository(context);
                    var res = await orderRepository.CountByAsync(new UsaOrderSpecification());

                    Assert.AreEqual(122, res);
                }
            }

            [TestMethod]
            public async Task AnyBySpecification()
            {
                using (var context = new NorthwindEntities())
                {
                    context.Configuration.LazyLoadingEnabled = false;

                    var orderRepository = new OrderRepository(context);
                    var res = await orderRepository.AnyByAsync(new UsaOrderSpecification());

                    Assert.IsTrue(res);
                }
            }

            [TestMethod]
            public async Task FindResultsByPredicate()
            {
                using (var context = new NorthwindEntities())
                {
                    context.Configuration.LazyLoadingEnabled = false;

                    var orderRepository = new OrderRepository(context);
                    var orders = await orderRepository.FindByAsync(x => x.ShipCountry == "USA");

                    Assert.AreEqual(122, orders.Count());
                }
            }

            [TestMethod]
            public async Task FindResultProjectionsByPredicate()
            {
                using (var context = new NorthwindEntities())
                {
                    context.Configuration.LazyLoadingEnabled = false;

                    var orderRepository = new OrderRepository(context);
                    var orderProjections = await orderRepository.FindByAsync(x => x.ShipCountry == "USA", x => new {
                        Order = x
                    });

                    Assert.AreEqual(122, orderProjections.Count());
                }
            }

            [TestMethod]
            public async Task FindResultProjectionsByPredicateWithManyIncludes()
            {
                using (var context = new NorthwindEntities())
                {
                    context.Configuration.LazyLoadingEnabled = false;

                    var orderRepository = new OrderRepository(context);
                    var orderProjections = await orderRepository.FindByAsync(x => x.ShipCountry == "USA", x => new
                    {
                        Order = x
                    }, x => x.Customer, x => x.Employee);

                    Assert.AreEqual(122, orderProjections.Count());
                    foreach (var result in orderProjections)
                    {
                        if (!string.IsNullOrEmpty(result.Order.CustomerID))
                            Assert.IsNotNull(result.Order.Customer);

                        if (result.Order.EmployeeID.HasValue)
                            Assert.IsNotNull(result.Order.Employee);
                    }
                }
            }

            [TestMethod]
            public async Task FindResultProjectionsByPredicateWithOneInclude()
            {
                using (var context = new NorthwindEntities())
                {
                    context.Configuration.LazyLoadingEnabled = false;

                    var orderRepository = new OrderRepository(context);
                    var orderProjections = await orderRepository.FindByAsync(x => x.ShipCountry == "USA", x => new
                    {
                        Order = x
                    }, new Expression<Func<Order, object>>[] { x => x.Customer });

                    Assert.AreEqual(122, orderProjections.Count());
                    foreach (var result in orderProjections)
                    {
                        if (!string.IsNullOrEmpty(result.Order.CustomerID))
                            Assert.IsNotNull(result.Order.Customer);
                       
                    }
                }
            }

            [TestMethod]
            public async Task FindResultProjectionsByPredicateWithFactoryManyIncludes()
            {
                using (var context = new NorthwindEntities())
                {
                    context.Configuration.LazyLoadingEnabled = false;

                    var orderRepository = new OrderRepository(context);
                    var orderProjections = await orderRepository.FindByAsync(x => x.ShipCountry == "USA", x => new
                    {
                        Order = x
                    }, x => new { x.Customer, x.Employee });

                    Assert.AreEqual(122, orderProjections.Count());
                    foreach (var result in orderProjections)
                    {
                        if (!string.IsNullOrEmpty(result.Order.CustomerID))
                            Assert.IsNotNull(result.Order.Customer);

                        if (result.Order.EmployeeID.HasValue)
                            Assert.IsNotNull(result.Order.Employee);
                    }
                }
            }

            [TestMethod]
            public async Task FindResultProjectionsByPredicateWithFactoryOneInclude()
            {
                using (var context = new NorthwindEntities())
                {
                    context.Configuration.LazyLoadingEnabled = false;

                    var orderRepository = new OrderRepository(context);
                    var orderProjections = await orderRepository.FindByAsync(x => x.ShipCountry == "USA", x => new
                    {
                        Order = x
                    }, x => x.Customer);

                    Assert.AreEqual(122, orderProjections.Count());
                    foreach (var result in orderProjections)
                    {
                        if (!string.IsNullOrEmpty(result.Order.CustomerID))
                            Assert.IsNotNull(result.Order.Customer);
                    }
                }
            }


            [TestMethod]
            public async Task CountByPredicate()
            {
                using (var context = new NorthwindEntities())
                {
                    context.Configuration.LazyLoadingEnabled = false;

                    var orderRepository = new OrderRepository(context);
                    var res = await orderRepository.CountByAsync(x => x.ShipCountry == "USA");

                    Assert.AreEqual(122, res);
                }
            }

            [TestMethod]
            public async Task AnyByPredicate()
            {
                using (var context = new NorthwindEntities())
                {
                    context.Configuration.LazyLoadingEnabled = false;

                    var orderRepository = new OrderRepository(context);
                    var res = await orderRepository.AnyByAsync(x => x.ShipCountry == "USA");

                    Assert.IsTrue(res);
                }
            }
        }
        
    }

    

    public class OrderRepository : RepositoryBase<Order>
    {
        public OrderRepository(IUnitOfWork unitOfWork) : base(unitOfWork, x => x.OrderID)
        {
        }
    }

    public partial class NorthwindEntities : IUnitOfWork, IUnitOfWorkAsync
    {
        public void Commit()
        {
            SaveChanges();
        }

        public async void CommitAsync()
        {
            await SaveChangesAsync();
        }
    }

    public class UsaOrderSpecification : SpecificationBase<Order>
    {
        public override Expression<Func<Order, bool>> SpecExpression
        {
            get { return x => x.ShipCountry == "USA"; }
        }
    }
}
