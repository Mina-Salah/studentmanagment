using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace StudentManagement.Core.Interfaces
{
    public interface IGenericRepository<T> where T : class
        {
            Task<IEnumerable<T>> GetAllAsync(Func<IQueryable<T>, IQueryable<T>>? include = null);
            Task<T?> GetByIdAsync(int id, Func<IQueryable<T>, IQueryable<T>>? include = null);
            Task AddAsync(T entity);
            void Update(T entity);
            void Delete(T entity);
        }
    }
