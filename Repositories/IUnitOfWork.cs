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
        private PlayerGoldRepository _playerGoldRepo;
        public UnitOfWork(string address)
        {
            _dbAddress = address;
        }

        public PlayerInfoRepository PlayerInfos => _playerInfoRepo ??= new PlayerInfoRepository();

        public ItemInfoRepository ItemInfos => _itemInfoRepo ??= new ItemInfoRepository();

        public PlayerItemRepostory PlayerItems => _playerItemRepo ??= new PlayerItemRepostory();

        public AuctionItemRepository AuctionItems => _auctionInfoRepo ??= new AuctionItemRepository();
        public PlayerGoldRepository PlayerGold => _playerGoldRepo ??= new PlayerGoldRepository();

        public void Dispose()
        {
        }
    }
}
