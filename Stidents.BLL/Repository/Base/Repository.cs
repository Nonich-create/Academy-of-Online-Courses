using Microsoft.EntityFrameworkCore;
using Students.DAL.Models;
using Students.DAL.Repositories.Base;
using Students.DAL.Specifications.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Students.BLL.Repository.Base
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly Context _db;

        public Repository(Context db)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }

        public async Task AddRangeAsync(IEnumerable<T> entity) => await _db.Set<T>().AddRangeAsync(entity);

        public async Task<IEnumerable<T>> GetAllAsync() => await _db.Set<T>().ToListAsync();

        private IQueryable<T> ApplySpecification(ISpecification<T> spec) => SpecificationEvaluator<T>.GetQuery(_db.Set<T>().AsQueryable(), spec);
        
        public async Task<IEnumerable<T>> GetAsync(ISpecification<T> spec) => await ApplySpecification(spec).ToListAsync();

        public async Task<int> CountAsync(ISpecification<T> spec) => await ApplySpecification(spec).CountAsync();

        public async Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> predicate) =>  await _db.Set<T>().Where(predicate).ToListAsync();

        public async Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> predicate = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeString = null, bool disableTracking = true)
        {
            IQueryable<T> query = _db.Set<T>();
            if (disableTracking) query = query.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(includeString)) query = query.Include(includeString);

            if (predicate != null) query = query.Where(predicate);

            if (orderBy != null)
                return await orderBy(query).ToListAsync();
            return await query.ToListAsync();
        }

        public virtual async Task<T> GetAsync(ISpecification<T> spec, bool disableTracking = true)
        {
            IQueryable<T> query = ApplySpecification(spec);
            if (disableTracking) query = query.AsNoTracking();
            return await query.FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> predicate = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, List<Expression<Func<T, object>>> includes = null, bool disableTracking = true)
        {
            IQueryable<T> query = _db.Set<T>();
            if (disableTracking) query = query.AsNoTracking();

            if (includes != null) query = includes.Aggregate(query, (current, include) => current.Include(include));

            if (predicate != null) query = query.Where(predicate);

            if (orderBy != null)
                return await orderBy(query).ToListAsync();
            return await query.ToListAsync();
        }

        public virtual async Task<T> GetByIdAsync(int id) => await _db.Set<T>().FindAsync(id);

      

        public async Task<T> AddAsync(T entity)
        {
            await _db.Set<T>().AddAsync(entity);
            return entity;
        }

        public async Task UpdateAsync(T entity) => _db.Entry(entity).State = EntityState.Modified;

        public async Task DeleteAsync(T entity) => _db.Set<T>().Remove(entity);

        public async Task DeleteRangeAsync(IEnumerable<T> entity) => _db.Set<T>().RemoveRange(entity);

        public async Task<T> SearchAsync(string query) => await _db.Set<T>().Where(query).FirstAsync();

 
    }
}
