using Payeh.SharedKernel.Domain;
using Payeh.SharedKernel.Domain.Repositories;

namespace Payeh.SharedKernel.UnitOfWork;

public interface IUnitOfWork : IDisposable
{
    IRepository<TEntity> Repository<TEntity>() where TEntity : Entity;
    Task<int> CommitAsync();
    int Commit();
}