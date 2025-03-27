using MySqlConnector;
using Repositories;
using ServerCode.Models;
using Newtonsoft.Json;
namespace BusinessLayer.Services
{
    public class PlayerLogInDataService : Service
    {

        public PlayerLogInDataService(RepositoryManager repo, string dbAddress) : base(repo, dbAddress)
        {
        }
        private async Task<string> SetUpDefaultDictionary()
        {
            await using MySqlConnection connection = new MySqlConnection(_dbAddress);
            await connection.OpenAsync();
            var items = await _repositoryManager.ItemInfos.GetItemInfoWithType(ItemType.dictionary, connection);
            string json = JsonConvert.SerializeObject(items.ToDictionary(key => key, val => val), Formatting.Indented);
            await connection.CloseAsync();
            return json;
        }
        public async Task<bool> SignUp(PlayerInfo playerInfo)
        {
            await using MySqlConnection conn = new MySqlConnection(_dbAddress);
            await conn.OpenAsync();
            await using MySqlTransaction transaction = await conn.BeginTransactionAsync();
            try
            {
                var info = await _repositoryManager.PlayerInfos.GetItemByPrimaryKeysAsync(playerInfo, conn);
                if (info.id == playerInfo.id)
                {
                    Console.WriteLine($"{info.id}:Duplicate");
                    return false;
                }
                bool success = await _repositoryManager.PlayerInfos.AddAsync(playerInfo, conn, transaction);
                string json = await SetUpDefaultDictionary();
                success &= await _repositoryManager.PlayerData.AddAsync(new PlayerDataInfo() { playerId = playerInfo.id, gold = 0, dictionary = json }, conn, transaction);
                await transaction.CommitAsync();
                Console.WriteLine(success);
                return success;
            }
            catch (MySqlException)
            {
                Console.WriteLine("Rollbakcc");
                await transaction.RollbackAsync();
                return false;
            }
            catch (InvalidOperationException)
            {
                Console.WriteLine("Rollback");
                await transaction.RollbackAsync();
                return false;
            }
        }
        public async Task<bool> LogIn(PlayerInfo playerInfo)
        {
            await using MySqlConnection conn = new MySqlConnection(_dbAddress);
            await conn.OpenAsync();
            try
            {
                var info = await _repositoryManager.PlayerInfos.GetItemByPrimaryKeysAsync(playerInfo, conn);
                return info.password == playerInfo.password;
            }
            catch (MySqlException)
            {
                return false;
            }
        }
        public async Task<PlayerDataInfo> GetPlayerData(string playerId)
        {
            await using MySqlConnection connection = new MySqlConnection(_dbAddress);
            await connection.OpenAsync();

            PlayerDataInfo myInfo = new PlayerDataInfo() { playerId = playerId };
            return await _repositoryManager.PlayerData.GetItemByPrimaryKeysAsync(myInfo, connection);
        }
    }
}
