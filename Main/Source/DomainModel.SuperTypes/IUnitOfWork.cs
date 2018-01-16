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

    /// <summary>
    /// Represent asynchronous unit of work
    /// </summary>
    public interface IUnitOfWorkAsync : IDisposable
    {
        /// <summary>
        /// Commit all changes
        /// </summary>
        void CommitAsync();
    }
}