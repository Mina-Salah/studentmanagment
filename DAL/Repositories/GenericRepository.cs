using Microsoft.EntityFrameworkCore;
using StudentManagement.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace StudentManagement.Data.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly ManagDbContext _context;
        private readonly DbSet<T> _dbSet;

        public GenericRepository(ManagDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAllAsync(
            Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            Func<IQueryable<T>, IQueryable<T>>? include = null,
            bool includeSoftDeleted = false
        )
        {
            IQueryable<T> query = _dbSet;

            if (!includeSoftDeleted && typeof(T).GetProperty("IsDeleted") != null)
            {
                var param = Expression.Parameter(typeof(T), "x");
                var prop = Expression.Property(param, "IsDeleted");
                var condition = Expression.Equal(prop, Expression.Constant(false));
                var lambda = Expression.Lambda<Func<T, bool>>(condition, param);
                query = query.Where(lambda);
            }

            if (filter != null)
                query = query.Where(filter);

            if (include != null)
                query = include(query);

            if (orderBy != null)
                return await orderBy(query).ToListAsync();

            return await query.ToListAsync();
        }

        public async Task<T?> GetByIdAsync(int id, Func<IQueryable<T>, IQueryable<T>>? include = null)
        {
            IQueryable<T> query = _dbSet;

            if (include != null)
                query = include(query);

            if (typeof(T).GetProperty("IsDeleted") != null)
            {
                var param = Expression.Parameter(typeof(T), "x");
                var prop = Expression.Property(param, "IsDeleted");
                var condition = Expression.Equal(prop, Expression.Constant(false));
                var lambda = Expression.Lambda<Func<T, bool>>(condition, param);
                query = query.Where(lambda);
            }

            return await query.FirstOrDefaultAsync(e => EF.Property<int>(e, "Id") == id);
        }

        public async Task<T?> GetFirstOrDefaultAsync(
            Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IQueryable<T>>? include = null,
            bool tracking = true,
            bool includeSoftDeleted = false
        )
        {
            IQueryable<T> query = _dbSet;

            if (!tracking)
                query = query.AsNoTracking();

            if (!includeSoftDeleted && typeof(T).GetProperty("IsDeleted") != null)
            {
                var param = Expression.Parameter(typeof(T), "x");
                var prop = Expression.Property(param, "IsDeleted");
                var condition = Expression.Equal(prop, Expression.Constant(false));
                var lambda = Expression.Lambda<Func<T, bool>>(condition, param);
                query = query.Where(lambda);
            }

            if (include != null)
                query = include(query);

            if (filter != null)
                query = query.Where(filter);

            return await query.FirstOrDefaultAsync();
        }

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        public async Task SoftDeleteAsync(int id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity != null)
            {
                var prop = typeof(T).GetProperty("IsDeleted");
                if (prop != null)
                {
                    prop.SetValue(entity, true);
                    _dbSet.Update(entity);
                }
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            var entity = await _dbSet.FindAsync(id);
            return entity != null;
        }

        public async Task<IEnumerable<T>> GetAllIncludingDeletedAsync()
        {
            return await _dbSet.IgnoreQueryFilters().ToListAsync();
        }

        public async Task<T?> GetByIdIncludingDeletedAsync(int id, Func<IQueryable<T>, IQueryable<T>>? include = null)
        {
            IQueryable<T> query = _dbSet;
            query = query.IgnoreQueryFilters();

            if (include != null)
                query = include(query);

            return await query.FirstOrDefaultAsync(e => EF.Property<int>(e, "Id") == id);
        }

        // ✅ CountAsync - دعم شامل للـ Soft Delete
        public async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null)
        {
            IQueryable<T> query = _dbSet;

            if (typeof(T).GetProperty("IsDeleted") != null)
            {
                var param = Expression.Parameter(typeof(T), "x");
                var prop = Expression.Property(param, "IsDeleted");
                var condition = Expression.Equal(prop, Expression.Constant(false));
                var lambda = Expression.Lambda<Func<T, bool>>(condition, param);
                query = query.Where(lambda);
            }

            if (predicate != null)
                query = query.Where(predicate);

            return await query.CountAsync();
        }
    }
}
