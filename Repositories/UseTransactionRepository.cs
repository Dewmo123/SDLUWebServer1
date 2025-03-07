

using MySqlConnector;

namespace Repositories
{
    public abstract class UseTransactionRepository<T> : IRepository<T>
    {
        protected MySqlTransaction? _transaction;
        protected MySqlConnection? _connection;
        protected string _dbAddress;
        public UseTransactionRepository(string address)
        {
            _dbAddress = address;
        }
        public void SetConnection(MySqlConnection conn, MySqlTransaction tran)
        {
            _transaction = tran;
            _connection = conn;
        }
        public void RemoveConnection()
        {
            _transaction = null;
            _connection = null;
        }
        public abstract Task<bool> AddAsync(T entity);
        public abstract Task<bool> DeleteAsync(T entity);
        public abstract Task<List<T>> GetAllItemsAsync();
        public abstract Task<T> GetByIdAsync(string id);
        public abstract Task<bool> UpdateAsync(T entity);

    }
}
