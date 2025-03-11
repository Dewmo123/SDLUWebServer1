using MySqlConnector;
using ServerCode.Models;

namespace Repositories
{
    public class PlayerDataRepository : IRepository<PlayerDataInfo>
    {
        public async Task<bool> AddAsync(PlayerDataInfo inPlayerGoldInfo, MySqlConnection connection, MySqlTransaction transaction)
        {
            MySqlCommand addGoldInfo = new MySqlCommand(
                $"INSERT {DBConfig.PLAYER_DATA_TABLE} ({DBConfig.PLAYER_ID},{DBConfig.GOLD})" +
                $" VALUES (@playerId,@gold)",
                connection,
                transaction
                );
            addGoldInfo.Parameters.AddWithValue("@playerId", inPlayerGoldInfo.playerId);
            addGoldInfo.Parameters.AddWithValue("@gold", inPlayerGoldInfo.gold);
            return await addGoldInfo.ExecuteNonQueryAsync() == 1;
        }

        public Task<bool> DeleteWithPrimaryKeysAsync(PlayerDataInfo entity, MySqlConnection connection, MySqlTransaction transaction)
        {
            throw new NotImplementedException();
        }

        public Task<List<PlayerDataInfo>> GetAllItemsAsync(MySqlConnection connection, MySqlTransaction transaction)
        {
            throw new NotImplementedException();
        }

        public async Task<PlayerDataInfo> GetItemByPrimaryKeysAsync(PlayerDataInfo inPlayerGoldInfo, MySqlConnection connection, MySqlTransaction transaction)
        {
            MySqlCommand getItem = new MySqlCommand($"SELECT * FROM {DBConfig.PLAYER_DATA_TABLE} WHERE {DBConfig.PLAYER_ID} = @playerId",connection,transaction);
            getItem.Parameters.AddWithValue("@playerId", inPlayerGoldInfo.playerId);
            var table = await getItem.ExecuteReaderAsync();

            PlayerDataInfo? newInfo = null;
            if (await table.ReadAsync())
            {
                newInfo = new PlayerDataInfo()
                {
                    playerId = table.GetString(table.GetOrdinal(DBConfig.PLAYER_ID)),
                    gold = table.GetInt32(table.GetOrdinal(DBConfig.GOLD))
                };
            }
            await table.CloseAsync();
            return newInfo;
        }

        public async Task<bool> UpdateAsync(PlayerDataInfo info, MySqlConnection connection, MySqlTransaction transaction)
        {
            MySqlCommand updateItem = new MySqlCommand($"UPDATE {DBConfig.PLAYER_DATA_TABLE} SET {DBConfig.GOLD} = @gold " +
                $"WHERE {DBConfig.PLAYER_ID} = @playerId", connection, transaction);
            updateItem.Parameters.AddWithValue("@playerId", info.playerId);
            updateItem.Parameters.AddWithValue("@gold", info.gold);
            return await updateItem.ExecuteNonQueryAsync() == 1;
        }
    }
}
