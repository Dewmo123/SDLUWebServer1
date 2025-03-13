using MySqlConnector;
using ServerCode.Models;
using static Repositories.DBConfig;

namespace Repositories
{
    public class PlayerDataRepository : IRepository<PlayerDataInfo>
    {
        public async Task<bool> AddAsync(PlayerDataInfo inPlayerGoldInfo, MySqlConnection connection, MySqlTransaction transaction)
        {
            MySqlCommand addGoldInfo = new MySqlCommand(
                $"INSERT {PLAYER_DATA_TABLE} ({PLAYER_ID},{GOLD})" +
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
            MySqlCommand getItem = new MySqlCommand($"SELECT * FROM {PLAYER_DATA_TABLE} WHERE {PLAYER_ID} = @playerId",connection,transaction);
            getItem.Parameters.AddWithValue("@playerId", inPlayerGoldInfo.playerId);
            var table = await getItem.ExecuteReaderAsync();

            PlayerDataInfo? newInfo = null;
            if (await table.ReadAsync())
            {
                newInfo = new PlayerDataInfo()
                {
                    playerId = table.GetString(table.GetOrdinal(PLAYER_ID)),
                    gold = table.GetInt32(table.GetOrdinal(GOLD))
                };
            }
            await table.CloseAsync();
            return newInfo;
        }

        public async Task<bool> UpdateAsync(PlayerDataInfo info, MySqlConnection connection, MySqlTransaction transaction)
        {
            MySqlCommand updateItem = new MySqlCommand($"UPDATE {PLAYER_DATA_TABLE} SET {GOLD} = @gold " +
                $"WHERE {PLAYER_ID} = @playerId", connection, transaction);
            updateItem.Parameters.AddWithValue("@playerId", info.playerId);
            updateItem.Parameters.AddWithValue("@gold", info.gold);
            return await updateItem.ExecuteNonQueryAsync() == 1;
        }
        public async Task<bool> ChangeQuantityFromPlayer(PlayerDataInfo dataInfo, MySqlConnection conn, MySqlTransaction transaction)
        {
            var info = await GetItemByPrimaryKeysAsync(dataInfo, conn, transaction);

            int gold = dataInfo.gold + info.gold;
            if (gold < 0)
                return false;
            info.gold = gold;
            return await UpdateAsync(info, conn, transaction);
        }
    }
}
