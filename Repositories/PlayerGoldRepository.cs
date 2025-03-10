using MySqlConnector;
using ServerCode.Models;

namespace Repositories
{
    public class PlayerGoldRepository : IRepository<PlayerGoldInfo>
    {
        public async Task<bool> AddAsync(PlayerGoldInfo inPlayerGoldInfo, MySqlConnection connection, MySqlTransaction transaction)
        {
            MySqlCommand addGoldInfo = new MySqlCommand(
                $"INSERT {DBConfig.PLAYER_GOLD_TABLE} ({DBConfig.PLAYER_ID},{DBConfig.GOLD})" +
                $" VALUES (@playerId,@gold)",
                connection,
                transaction
                );
            addGoldInfo.Parameters.AddWithValue("@playerId", inPlayerGoldInfo.playerId);
            addGoldInfo.Parameters.AddWithValue("@gold", inPlayerGoldInfo.gold);
            return await addGoldInfo.ExecuteNonQueryAsync() == 1;
        }

        public Task<bool> DeleteAsync(PlayerGoldInfo entity, MySqlConnection connection, MySqlTransaction transaction)
        {
            throw new NotImplementedException();
        }

        public Task<List<PlayerGoldInfo>> GetAllItemsAsync(MySqlConnection connection, MySqlTransaction transaction)
        {
            throw new NotImplementedException();
        }

        public async Task<PlayerGoldInfo> GetItemByPrimaryKeysAsync(PlayerGoldInfo inPlayerGoldInfo, MySqlConnection connection, MySqlTransaction transaction)
        {
            MySqlCommand getItem = new MySqlCommand($"SELECT * FROM {DBConfig.PLAYER_GOLD_TABLE} WHERE {DBConfig.PLAYER_ID} = @playerId");
            getItem.Parameters.AddWithValue("@playerId", inPlayerGoldInfo.playerId);
            var table = await getItem.ExecuteReaderAsync();

            PlayerGoldInfo? newInfo = null;
            if(await table.ReadAsync())
            {
                newInfo = new PlayerGoldInfo()
                {
                    playerId = table.GetString(table.GetOrdinal(DBConfig.PLAYER_ID)),
                    gold = table.GetInt32(table.GetOrdinal(DBConfig.GOLD))
                };
            }
            return newInfo;
        }

        public Task<bool> UpdateAsync(PlayerGoldInfo entity, MySqlConnection connection, MySqlTransaction transaction)
        {
            throw new NotImplementedException();
        }
    }
}
