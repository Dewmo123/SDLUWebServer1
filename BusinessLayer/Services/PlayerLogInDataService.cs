using MySqlConnector;
using Repositories;
using ServerCode.Models;

namespace BusinessLayer.Services
{
    public class PlayerLogInDataService : Service
    {
        private Dictionary<string, int> _defaultDictionary;
        public PlayerLogInDataService(RepositoryManager repo, string dbAddress) : base(repo, dbAddress)
        {
            _defaultDictionary = new Dictionary<string, int>();
            SetUpDefaultDictionary();
        }
        private async void SetUpDefaultDictionary()
        {
            await using MySqlConnection connection = new MySqlConnection(_dbAddress);
            var items = await _repositoryManager.ItemInfos.GetItemInfoWithType(ItemType.Dictionary, connection);
            items.ForEach(item => _defaultDictionary.Add(item.itemName, 0));
        }
        public async Task<bool> SignUp(PlayerInfo playerInfo)
        {
            await using MySqlConnection conn = new MySqlConnection(_dbAddress);
            await conn.OpenAsync();
            await using MySqlTransaction transaction = await conn.BeginTransactionAsync();
            try
            {
                var info = await _repositoryManager.PlayerInfos.GetItemByPrimaryKeysAsync(playerInfo, conn, transaction);
                if (info.id == playerInfo.id)
                {
                    Console.WriteLine($"{info.id}:Duplicate");
                    return false;
                }
                bool success = await _repositoryManager.PlayerInfos.AddAsync(playerInfo, conn, transaction);
                success &= await _repositoryManager.PlayerData.AddAsync(new PlayerDataInfo() { playerId = playerInfo.id, gold = 0 }, conn, transaction);
                await transaction.CommitAsync();
                Console.WriteLine(success);
                return success;
            }
            catch (MySqlException)
            {
                await transaction.RollbackAsync();
                return false;
            }
            catch (InvalidOperationException)
            {
                await transaction.RollbackAsync();
                return false;
            }
        }
        public async Task<bool> LogIn(PlayerInfo playerInfo)
        {
            await using MySqlConnection conn = new MySqlConnection(_dbAddress);
            await conn.OpenAsync();
            await using MySqlTransaction transaction = await conn.BeginTransactionAsync();
            try
            {
                var info = await _repositoryManager.PlayerInfos.GetItemByPrimaryKeysAsync(playerInfo, conn, transaction);
                await transaction.CommitAsync();
                return info.password == playerInfo.password;
            }
            catch (MySqlException)
            {
                await transaction.RollbackAsync();
                return false;
            }
        }
    }
}
