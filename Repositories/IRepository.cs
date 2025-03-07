namespace Repositories
{
    public interface IRepository<T>
    {
        public Task<T> GetByIdAsync(string id);
        public Task<List<T>> GetAllItemsAsync();
        public Task<bool> AddAsync(T entity);
        public Task<bool> UpdateAsync(T entity);
        public Task<bool> DeleteAsync(T entity);
    }
}
