using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace StudentManagement.Core.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync(
            Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            Func<IQueryable<T>, IQueryable<T>>? include = null
        );

        Task<T?> GetByIdAsync(
            int id,
            Func<IQueryable<T>, IQueryable<T>>? include = null
        );

        Task AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
        Task SoftDeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}
