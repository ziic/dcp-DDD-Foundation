using System;

namespace dcp.DDD.DomainModel.SuperTypes
{
    /// <summary>
    /// Represent unit of work
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// Commit all changes
        /// </summary>
        void Commit();
    }
}