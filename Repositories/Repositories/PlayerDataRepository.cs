using MySqlConnector;
using Repositories;
using ServerCode.Models;
using static Repositories.DBConfig;

namespace DataAccessLayer.Repositories
{
    public interface IPlayerDataRepository : IRepository<PlayerDataInfo>
    {
        public Task<bool> ChangeQuantityFromPlayer(PlayerDataInfo dataInfo, MySqlConnection conn, MySqlTransaction transaction);
    }
    public class PlayerDataRepository : IPlayerDataRepository
    {
        public async Task<bool> AddAsync(PlayerDataInfo inPlayerGoldInfo, MySqlConnection connection, MySqlTransaction transaction)
        {
            MySqlCommand addGoldInfo = new MySqlCommand(
                $"INSERT {PLAYER_DATA_TABLE} ({PLAYER_ID},{GOLD},{DICTIONARY})" +
                $" VALUES (@playerId,@gold,@dictionary)",
                connection,
                transaction
                );
            addGoldInfo.Parameters.AddWithValue("@playerId", inPlayerGoldInfo.playerId);
            addGoldInfo.Parameters.AddWithValue("@gold", inPlayerGoldInfo.gold);
            addGoldInfo.Parameters.AddWithValue("@dictionary", inPlayerGoldInfo.dictionary);
            return await addGoldInfo.ExecuteNonQueryAsync() == 1;
        }

        public Task<bool> DeleteWithPrimaryKeysAsync(PlayerDataInfo entity, MySqlConnection connection, MySqlTransaction transaction)
        {
            throw new NotImplementedException();
        }

        public Task<List<PlayerDataInfo>> GetAllItemsAsync(MySqlConnection connection)
        {
            throw new NotImplementedException();
        }

        public async Task<PlayerDataInfo> GetItemByPrimaryKeysAsync(PlayerDataInfo inPlayerGoldInfo, MySqlConnection connection)
        {
            MySqlCommand getItem = new MySqlCommand($"SELECT * FROM {PLAYER_DATA_TABLE} WHERE {PLAYER_ID} = @playerId", connection);
            getItem.Parameters.AddWithValue("@playerId", inPlayerGoldInfo.playerId);
            var table = await getItem.ExecuteReaderAsync();

            if (await table.ReadAsync())
            {
                inPlayerGoldInfo.dictionary = table.GetString(table.GetOrdinal(DICTIONARY));
                inPlayerGoldInfo.gold = table.GetInt32(table.GetOrdinal(GOLD));
            }
            
            await table.CloseAsync();

            return inPlayerGoldInfo;
        }

        public async Task<bool> UpdateAsync(PlayerDataInfo info, MySqlConnection connection, MySqlTransaction transaction)
        {
            MySqlCommand updateItem = new MySqlCommand(
                $"UPDATE {PLAYER_DATA_TABLE}" +
                $" SET {GOLD} = @gold,{DICTIONARY} = @dictionary " +
                $"WHERE {PLAYER_ID} = @playerId", connection, transaction);
            updateItem.Parameters.AddWithValue("@playerId", info.playerId);
            updateItem.Parameters.AddWithValue("@gold", info.gold);
            updateItem.Parameters.AddWithValue("@dictionary", info.dictionary);
            return await updateItem.ExecuteNonQueryAsync() == 1;
        }
        public async Task<bool> ChangeQuantityFromPlayer(PlayerDataInfo dataInfo, MySqlConnection conn, MySqlTransaction transaction)
        {
            var info = await GetItemByPrimaryKeysAsync(dataInfo, conn);

            int gold = dataInfo.gold + info.gold;
            if (gold < 0)
                return false;
            info.gold = gold;
            return await UpdateAsync(info, conn, transaction);
        }
    }
}
