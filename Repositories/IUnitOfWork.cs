using MySqlConnector;

namespace Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        PlayerInfoRepository PlayerInfos { get; }
        ItemInfoRepository ItemInfos { get; }
        PlayerItemRepostory PlayerItems{ get; }
        AuctionItemRepository AuctionItems{ get; }
    }
    public class UnitOfWork : IUnitOfWork
    {
        private readonly string _dbAddress;

        private MySqlConnection _connection;
        private MySqlTransaction _transaction;

        private PlayerInfoRepository _playerInfoRepo;
        private PlayerItemRepostory _playerItemRepo;
        private AuctionItemRepository _auctionInfoRepo;
        private ItemInfoRepository _itemInfoRepo;
        public UnitOfWork(string address)
        {
            _dbAddress = address;
        }
        private async Task<MySqlConnection> GetConnectionAsync()
        {
            if (_connection == null)
            {
                _connection = new MySqlConnection(_dbAddress);
                await _connection.OpenAsync();
            }
            return _connection;
        }
        public async Task BeginTransactionAsync()
        {
            var connection = await GetConnectionAsync();
            _transaction = await connection.BeginTransactionAsync();
        }
        public async Task CommitAsync()
        {
            try
            {
                await _transaction?.CommitAsync();
                await CloseConnectionAsync();
            }
            catch
            {
                await RollbackAsync();
                throw;
            }
            finally
            {
                _transaction?.Dispose();
                _transaction = null;
            }
        }

        // 트랜잭션 롤백
        public async Task RollbackAsync()
        {
            try
            {
                await _transaction?.RollbackAsync();
                await CloseConnectionAsync();
            }
            finally
            {
                _transaction?.Dispose();
                _transaction = null;
            }
        }
        public async Task CloseConnectionAsync()
        {
            await _connection.CloseAsync();
        }
        public PlayerInfoRepository PlayerInfos => _playerInfoRepo ??= new PlayerInfoRepository(_dbAddress);

        public ItemInfoRepository ItemInfos => _itemInfoRepo ??= new ItemInfoRepository(_dbAddress);

        public PlayerItemRepostory PlayerItems => _playerItemRepo ??= new PlayerItemRepostory(_dbAddress);

        public AuctionItemRepository AuctionItems => _auctionInfoRepo ??= new AuctionItemRepository(_dbAddress);

        public void Dispose()
        {
        }
    }
}
