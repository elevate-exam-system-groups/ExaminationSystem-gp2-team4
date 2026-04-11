using System;
using System.Threading.Tasks;
using Examination_System.Common.Models;

namespace Examination_System.Common.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<T> Repository<T>() where T : BaseEntity;
        Task<int> SaveChangesAsync();
    }
}
