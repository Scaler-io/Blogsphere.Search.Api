using System.Linq.Expressions;

namespace Blogsphere.Search.Api.Data.Interfaces;

public interface IDataTableRepository<T> where T : BaseTabeEntity
{
    Task<T?> GetAsync(string partitionKey, string rowKey);
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> QueryAsync(string filter);
    Task<IEnumerable<T>> QueryAsync(Expression<Func<T, bool>> filter);
    Task<bool> ExistsAsync(string partitionKey, string rowKey);
    Task UpsertAsync(T entity);
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(string partitionKey, string rowKey);
    Task DeleteAsync(T entity);
    Task AddBatchAsync(IEnumerable<T> entities);
    Task<int> CountAsync(string filter = null);
}
