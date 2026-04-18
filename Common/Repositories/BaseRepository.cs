using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Examination_System.Common.Models;
using ExaminationSystem.API.Common.Data;

namespace Examination_System.Common.Repositories
{
    public class BaseRepository<T> : IRepository<T> where T : BaseEntity 
    {
        protected readonly AppDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public BaseRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task<T?> GetByIdAsync(Guid id)
        {
            return await _dbSet.FindAsync(id);
        }

        public Task<IQueryable<T>> GetAllAsync()
        {
            return Task.FromResult(_dbSet.AsQueryable());
        }

        public Task<IQueryable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return Task.FromResult(_dbSet.Where(predicate));
        }

        public void Add(T entity)
        {
            _dbSet.Add(entity);
        }

        public void Update(T entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
        }

        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }
    }
}
