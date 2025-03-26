using DataAccessLayer.Repositories;
using MySqlConnector;

namespace Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IPlayerInfoRepository PlayerInfos { get; }
        IItemInfoRepository ItemInfos { get; }
        IPlayerItemRepository PlayerItems{ get; }
        IAuctionRepository AuctionItems{ get; }
        IPlayerDataRepository PlayerData { get; }
    }
}
