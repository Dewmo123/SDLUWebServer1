using MySqlConnector;

namespace Repositories
{
    public abstract class RepositoryBase<T> : IRepository<T>
    {
        public abstract Task<bool> AddAsync(T entity, MySqlConnection connection, MySqlTransaction transaction);
        public abstract Task<bool> DeleteAsync(T entity, MySqlConnection connection, MySqlTransaction transaction);
        public abstract Task<List<T>> GetAllItemsAsync(MySqlConnection connection, MySqlTransaction transaction);
        public abstract Task<T> GetByIdAsync(string id, MySqlConnection connection, MySqlTransaction transaction);
        public abstract Task<bool> UpdateAsync(T entity, MySqlConnection connection, MySqlTransaction transaction);
    }
}
