using System;

namespace dcp.DDD.DomainModel.SuperTypes
{
    public interface IUnitOfWork : IDisposable
    {
        void Commit();
    }
}