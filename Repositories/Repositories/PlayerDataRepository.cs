using MySqlConnector;
using Repositories;
using ServerCode.DAO;
using static Repositories.DBConfig;

namespace DataAccessLayer.Repositories
{
    public interface IPlayerDataRepository : IRepository<PlayerDataDAO>
    {
        public Task<bool> ChangeQuantityFromPlayer(PlayerDataDAO dataInfo, MySqlConnection conn, MySqlTransaction transaction);
    }
    public class PlayerDataRepository : IPlayerDataRepository
    {
        public async Task<bool> AddAsync(PlayerDataDAO inPlayerGoldInfo, MySqlConnection connection, MySqlTransaction transaction)
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

        public Task<bool> DeleteWithPrimaryKeysAsync(PlayerDataDAO entity, MySqlConnection connection, MySqlTransaction transaction)
        {
            throw new NotImplementedException();
        }

        public Task<List<PlayerDataDAO>> GetAllItemsAsync(MySqlConnection connection)
        {
            throw new NotImplementedException();
        }

        public async Task<PlayerDataDAO> GetItemByPrimaryKeysAsync(PlayerDataDAO inPlayerGoldInfo, MySqlConnection connection)
        {
            MySqlCommand getItem = new MySqlCommand($"SELECT * FROM {PLAYER_DATA_TABLE} WHERE {PLAYER_ID} = @playerId", connection);
            getItem.Parameters.AddWithValue("@playerId", inPlayerGoldInfo.playerId);
            var table = await getItem.ExecuteReaderAsync();
            PlayerDataDAO newItem = new();
            if (await table.ReadAsync())
            {
                newItem.playerId = table.GetString(table.GetOrdinal(PLAYER_ID));
                newItem.dictionary = table.GetString(table.GetOrdinal(DICTIONARY));
                newItem.gold = table.GetInt32(table.GetOrdinal(GOLD));
            }
            await table.CloseAsync();

            return newItem;
        }

        public async Task<bool> UpdateAsync(PlayerDataDAO info, MySqlConnection connection, MySqlTransaction transaction)
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
        public async Task<bool> ChangeQuantityFromPlayer(PlayerDataDAO dataInfo, MySqlConnection conn, MySqlTransaction transaction)
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
