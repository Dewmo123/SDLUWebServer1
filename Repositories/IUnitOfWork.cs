using DataAccessLayer.Repositories;
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

        private PlayerInfoRepository _playerInfoRepo;
        private PlayerItemRepostory _playerItemRepo;
        private AuctionItemRepository _auctionInfoRepo;
        private ItemInfoRepository _itemInfoRepo;
        private PlayerDataRepository _playerGoldRepo;
        public UnitOfWork(string address)
        {
            _dbAddress = address;
        }

        public PlayerInfoRepository PlayerInfos => _playerInfoRepo ??= new PlayerInfoRepository();

        public ItemInfoRepository ItemInfos => _itemInfoRepo ??= new ItemInfoRepository();

        public PlayerItemRepostory PlayerItems => _playerItemRepo ??= new PlayerItemRepostory();

        public AuctionItemRepository AuctionItems => _auctionInfoRepo ??= new AuctionItemRepository();
        public PlayerDataRepository PlayerData => _playerGoldRepo ??= new PlayerDataRepository();

        public void Dispose()
        {
        }
    }
}
