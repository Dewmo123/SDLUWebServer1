using MySqlConnector;
namespace Repositories
{
    public interface IRepository<T>
    {
        public Task<T?> GetItemByPrimaryKeysAsync(T entity, MySqlConnection connection);
        public Task<List<T>> GetAllItemsAsync(MySqlConnection connection);
        public Task<bool> AddAsync(T entity, MySqlConnection connection, MySqlTransaction transaction);
        public Task<bool> UpdateAsync(T entity, MySqlConnection connection, MySqlTransaction transaction);
        public Task<bool> DeleteWithPrimaryKeysAsync(T entity, MySqlConnection connection, MySqlTransaction transaction);

    }
}
