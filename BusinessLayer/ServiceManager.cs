using BusinessLayer.Services;
using MySqlConnector;
using ServerCode.Models;
using static Repositories.DBConfig;

namespace Repositories
{

    public class ServiceManager
    {
        string _dbAddress;
        private RepositoryManager _repositoryManager;

        private AuctionService _auctionService;
        private ItemInfoService _itemInfoService;
        private PlayerItemService _playerItemService;
        private PlayerLogInDataService _playerLogInDataService;
        public ServiceManager(string connectionAddress)
        {
            _dbAddress = connectionAddress;
            _repositoryManager = new RepositoryManager(connectionAddress);
        }

        public AuctionService auctionService => _auctionService ??= new AuctionService(_repositoryManager, _dbAddress);
        public ItemInfoService itemInfoService => _itemInfoService ??= new ItemInfoService(_repositoryManager, _dbAddress);
        public PlayerItemService playerItemService => _playerItemService ??= new PlayerItemService(_repositoryManager, _dbAddress);
        public PlayerLogInDataService playerLogInDataService => _playerLogInDataService ??= new PlayerLogInDataService(_repositoryManager, _dbAddress);
    }
}
