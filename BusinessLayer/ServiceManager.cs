using AutoMapper;
using BusinessLayer.Services;
using MySqlConnector;
using ServerCode.DAO;
using static Repositories.DBConfig;

namespace Repositories
{

    public class ServiceManager
    {
        string _dbAddress;
        private IMapper _mapper;
        private RepositoryManager _repositoryManager;

        private AuctionService? _auctionService;
        private ItemInfoService? _itemInfoService;
        private PlayerItemService? _playerItemService;
        private PlayerDataService? _playerDataService;
        private PlayerInfoService? _playerInfoService;
        public ServiceManager(IMapper mapper, string connectionAddress)
        {
            _mapper = mapper;
            _dbAddress = connectionAddress;
            _repositoryManager = new RepositoryManager();
        }

        public AuctionService auctionService => _auctionService ??= new AuctionService(_repositoryManager, _mapper, _dbAddress);
        public ItemInfoService itemInfoService => _itemInfoService ??= new ItemInfoService(_repositoryManager, _mapper, _dbAddress);
        public PlayerItemService playerItemService => _playerItemService ??= new PlayerItemService(_repositoryManager, _mapper, _dbAddress);
        public PlayerDataService playerDataService => _playerDataService ??= new PlayerDataService(_repositoryManager, _mapper, _dbAddress);
        public PlayerInfoService playerInfoService => _playerInfoService ??= new PlayerInfoService(_repositoryManager, _mapper, _dbAddress);
    }
}
