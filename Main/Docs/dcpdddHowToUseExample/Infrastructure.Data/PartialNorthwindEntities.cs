using dcp.DDD.DomainModel.SuperTypes;

namespace Infrastructure.Data
{
    public partial class NorthwindEntities : IUnitOfWork
    {
        public void Commit()
        {
            SaveChanges();
        }
    }
}
