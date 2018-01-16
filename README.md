# dcp-DDD-Foundation
.NET implementation of Domain-Driven Design principles.

Base super types for repository, specification, unit of work, query commands and other of Domain and Infrastructure Data layers of Domain Driven Design approach.

The project includes some useful base classes to build a "Domain" layer and an "Infrasructure Data" layer when you use Domain Driven Design approach. In terms of DDD these classes named as Super Types.

Infrastructure Data layer supports Entity Framework.

# Overview
DDD approach assumes that you separate your types into several layers: UI, Application, Domain, Infrastructure for data persistence and other. You can learn more details using the following link: [http://blogs.msdn.com/b/marblogging/archive/2011/05/23/domain-drive-design-n-layered-net-4-0-architecture-guide.aspx](http://blogs.msdn.com/b/marblogging/archive/2011/05/23/domain-drive-design-n-layered-net-4-0-architecture-guide.aspx).
 
![](/docs/images/Home_scheme1.PNG)

The project consists of two libraries (_dcp.DDD.DomainModel.SuperTypes.dll, dcp.DDD.Infrastructure.Data.EF.SuperTypes.dll_) and, as you can see in **figure 1**, covers two main blocks: 
* Bases (SuperTypes) of Domain Model Layer;
* Bases (Layer SuperTypes) of Infrastructure Layer for Data Persistence. This block relies on Entity Framework.

It provides base types (classes) and contracts (interfaces) as a start points to create your own repositories, specification, units of work.
Domain Model Layer block contains:
* _IRepository_ contract that covers necessary general methods of a repository pattern, such as Find, Add, Delete and other useful query methods (see details in Documentation SOON). Sometimesa  consumer of the repository (developer) should know that he works with the database on a lower layer, and he should do more optimized queries: select only necessary information and pre-load related entites.  To do optimized queries you can use methods that specify a projection that will contain only necessary data, and specify relations that should be eager loaded for the best performace;
* _IUnitOfWork_ contract that covers "unit of work" pattern;
* _Specification_ base types. Learn more on [http://en.wikipedia.org/wiki/Specification_pattern](http://en.wikipedia.org/wiki/Specification_pattern). Repository can query entities satisfied by specification.
Infrastructure Layer for Data Persistence block contains:
* Abstract _RepositoryBase_ base type that implements all necessary general things, and relied on **EntityFramework** ([https://github.com/aspnet/EntityFramework6](https://github.com/aspnet/EntityFramework6));
* _AdaptedRepositoryBase_ **(not completed)** base repository type that may be useful when your data model and domain model are different. It uses **AutoMapper** ([http://automapper.org](http://automapper.org)) to transparently map a domain entity to a data entity.

# Installation

There are two ways to install.

Download a zip-archive with dll-files manually from [https://github.com/ziic/dcp-DDD-Foundation/releases](https://github.com/ziic/dcp-DDD-Foundation/releases)

As NuGet packages:
* [https://www.nuget.org/packages/dcp.DDD.DomainModel](https://www.nuget.org/packages/dcp.DDD.DomainModel)
* [https://www.nuget.org/packages/dcp.DDD.DataInfrastructure](https://www.nuget.org/packages/dcp.DDD.DataInfrastructure)
* [https://www.nuget.org/packages/dcp.DDD.EFAdapted](https://www.nuget.org/packages/dcp.DDD.EFAdapted)

# How To use

See wiki [https://github.com/ziic/dcp-DDD-Foundation/wiki](https://github.com/ziic/dcp-DDD-Foundation/wiki)

# Documentation

You can download documentation from releases page: [https://github.com/ziic/dcp-DDD-Foundation/releases](https://github.com/ziic/dcp-DDD-Foundation/releases).

As the library are fully covered with unit tests, you can also use them to learn how it works.





