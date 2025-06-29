using System.Linq.Expressions;

public interface IGenericRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAllAsync(
        Expression<Func<T, bool>>? filter = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        Func<IQueryable<T>, IQueryable<T>>? include = null,
        bool includeSoftDeleted = false // ✅ أضف هذا
    );

    Task<T?> GetByIdAsync(
        int id,
        Func<IQueryable<T>, IQueryable<T>>? include = null
    );


    Task<T?> GetFirstOrDefaultAsync(
        Expression<Func<T, bool>>? filter = null,
        Func<IQueryable<T>, IQueryable<T>>? include = null,
        bool tracking = true,
        bool includeSoftDeleted = false
    );

    Task AddAsync(T entity);
    void Update(T entity);
    void Delete(T entity);
    Task SoftDeleteAsync(int id);
    Task<bool> ExistsAsync(int id);

    Task<IEnumerable<T>> GetAllIncludingDeletedAsync();
    Task<T?> GetByIdIncludingDeletedAsync( int id, Func<IQueryable<T>, IQueryable<T>>? include = null
);

}
