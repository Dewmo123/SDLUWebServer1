using MySqlConnector;
namespace Repositories
{
    public interface IRepository<T>
    {
        public Task<T> GetByIdAsync(T entity, MySqlConnection connection, MySqlTransaction transaction);
        public Task<List<T>> GetAllItemsAsync(MySqlConnection connection, MySqlTransaction transaction);
        public Task<bool> AddAsync(T entity, MySqlConnection connection, MySqlTransaction transaction);
        public Task<bool> UpdateAsync(T entity, MySqlConnection connection, MySqlTransaction transaction);
        public Task<bool> DeleteAsync(T entity, MySqlConnection connection, MySqlTransaction transaction);

    }
}
