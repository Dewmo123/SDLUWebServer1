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
        private PlayerDataService? _playerLogInDataService;
        public ServiceManager(IMapper mapper, string connectionAddress)
        {
            _mapper = mapper;
            _dbAddress = connectionAddress;
            _repositoryManager = new RepositoryManager(connectionAddress);
        }

        public AuctionService auctionService => _auctionService ??= new AuctionService(_repositoryManager, _mapper, _dbAddress);
        public ItemInfoService itemInfoService => _itemInfoService ??= new ItemInfoService(_repositoryManager, _mapper, _dbAddress);
        public PlayerItemService playerItemService => _playerItemService ??= new PlayerItemService(_repositoryManager, _mapper, _dbAddress);
        public PlayerDataService playerLogInDataService => _playerLogInDataService ??= new PlayerDataService(_repositoryManager, _mapper, _dbAddress);
    }
}
