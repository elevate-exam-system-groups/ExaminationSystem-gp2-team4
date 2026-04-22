using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Examination_System.Common.Models;

namespace Examination_System.Common.Repositories
{
    public interface IRepository<T> where T : BaseEntity
    {
        Task<T?> GetByIdAsync(Guid id);
        IQueryable<T> GetAll();
        IQueryable<T> Find(Expression<Func<T, bool>> predicate);
        Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null);
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);
    }
}
