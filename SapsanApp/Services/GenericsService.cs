using Microsoft.EntityFrameworkCore;
using SapsanApp.Models;
using SapsanApp.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SapsanApp.Services
{
    // Repository service
    public class GenericsService<T, U> : IGenericRepository<T, U> where T : class where U : struct
    {
        private readonly SapsanContext _context;
        private DbSet<T> _dbSet;
        public GenericsService(SapsanContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }
        public async Task Post(T entity)
        {
            await _dbSet.AddAsync(entity);

            await _context.SaveChangesAsync();
        }
        public async Task Delete(U id)
        {
            _dbSet.Remove(_dbSet.Find(id));

            await _context.SaveChangesAsync();
        }
        public async Task<object> Get()
        {
            return await _dbSet.AsNoTracking().ToListAsync();
        }
        public async Task<T> GetById(U id)
        {
            return await _dbSet.FindAsync(id);
        }
        public async Task Put(T entity)
        {
            _context.Update(entity);

            await _context.SaveChangesAsync();
        }
    }
}
