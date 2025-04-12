using MySqlConnector;
using Repositories;
using ServerCode.DAO;
using static Repositories.DBConfig;

namespace DataAccessLayer.Repositories
{
    public interface IPlayerItemRepository : IRepository<PlayerItemDAO>
    {
        public Task<bool> CheckConditionAndChangePlayerItem(PlayerItemDAO itemInfo, PlayerItemDAO remainItem, MySqlConnection connection, MySqlTransaction transaction);
        public Task<List<PlayerItemDAO>> GetItemsByPlayerId(string playerId, MySqlConnection connection);
    }
    public class PlayerItemRepostory : IPlayerItemRepository
    {

        public async Task<bool> AddAsync(PlayerItemDAO itemInfo, MySqlConnection connection, MySqlTransaction transaction)
        {
            if (itemInfo.quantity <= 0) return false;

            var cmd = new MySqlCommand(
                $"INSERT INTO {PLAYER_ITEM_TABLE} ({PLAYER_ID}, {QUANTITY},{ITEM_NAME}) " +
                $"VALUES (@playerId, @quantity,@itemName)", connection, transaction);
            cmd.Parameters.AddWithValue("@playerId", itemInfo.playerId);
            cmd.Parameters.AddWithValue("@quantity", itemInfo.quantity);
            cmd.Parameters.AddWithValue("@itemName", itemInfo.itemName);

            return await cmd.ExecuteNonQueryAsync() > 0;
        }
        public async Task<bool> DeleteWithPrimaryKeysAsync(PlayerItemDAO itemInfo, MySqlConnection connection, MySqlTransaction transaction)
        {
            var cmd = new MySqlCommand(
                $"DELETE FROM {PLAYER_ITEM_TABLE} " +
                $"WHERE {PLAYER_ID} = @playerId AND {ITEM_NAME} = @itemName", connection, transaction);
            cmd.Parameters.AddWithValue("@playerId", itemInfo.playerId);
            cmd.Parameters.AddWithValue("@itemName", itemInfo.itemName);

            return await cmd.ExecuteNonQueryAsync() > 0;
        }

        public Task<List<PlayerItemDAO>> GetAllItemsAsync(MySqlConnection connection)
        {
            throw new NotImplementedException();
        }

        public async Task<PlayerItemDAO> GetItemByPrimaryKeysAsync(PlayerItemDAO itemInfo, MySqlConnection connection)
        {
            var cmd = new MySqlCommand(
                $"SELECT * FROM {PLAYER_ITEM_TABLE}" +
                $" WHERE {PLAYER_ID} = @playerId AND {ITEM_NAME} = @itemName", connection);
            cmd.Parameters.AddWithValue("@playerId", itemInfo.playerId);
            cmd.Parameters.AddWithValue("@itemName", itemInfo.itemName);

            var table = await cmd.ExecuteReaderAsync();
            PlayerItemDAO? info = null;
            if (await table.ReadAsync())
            {
                info = new PlayerItemDAO
                {
                    playerId = table.GetString(table.GetOrdinal(PLAYER_ID)),
                    quantity = table.GetInt32(table.GetOrdinal(QUANTITY)),
                    itemName = table.GetString(table.GetOrdinal(ITEM_NAME))
                };
            }
            await table.CloseAsync();
            return info;
        }

        public async Task<bool> UpdateAsync(PlayerItemDAO itemInfo, MySqlConnection connection, MySqlTransaction transaction)
        {
            var cmd = new MySqlCommand(
                $"UPDATE {PLAYER_ITEM_TABLE} " +
                $"SET {QUANTITY} = @quantity " +
                $"WHERE {PLAYER_ID} = @playerId AND {ITEM_NAME} = @itemName", connection, transaction);

            cmd.Parameters.AddWithValue("@playerId", itemInfo.playerId);
            cmd.Parameters.AddWithValue("@itemName", itemInfo.itemName);
            cmd.Parameters.AddWithValue("@quantity", itemInfo.quantity);

            return await cmd.ExecuteNonQueryAsync() > 0;
        }
        public async Task<bool> CheckConditionAndChangePlayerItem(PlayerItemDAO itemInfo, PlayerItemDAO remainItem, MySqlConnection connection, MySqlTransaction transaction)
        {
            if (remainItem == null && itemInfo.quantity > 0)
                return await AddAsync(itemInfo, connection, transaction);

            int quantity = remainItem.quantity + itemInfo.quantity;
            if (quantity < 0)
                return false;
            remainItem.quantity = quantity;
            return await UpdateAsync(remainItem, connection, transaction);
        }
        public async Task<List<PlayerItemDAO>> GetItemsByPlayerId(string playerId, MySqlConnection connection)
        {
            MySqlCommand selectItems = new MySqlCommand(
                $"SELECT * FROM {PLAYER_ITEM_TABLE}" +
                $" WHERE {PLAYER_ID} = @playerId", connection);
            selectItems.Parameters.AddWithValue("@playerId", playerId);
            var table = await selectItems.ExecuteReaderAsync();
            List<PlayerItemDAO> items = new List<PlayerItemDAO>();
            while (await table.ReadAsync())
            {
                items.Add(new PlayerItemDAO()
                {
                    playerId = table.GetString(table.GetOrdinal(PLAYER_ID)),
                    quantity = table.GetInt32(table.GetOrdinal(QUANTITY)),
                    itemName = table.GetString(table.GetOrdinal(ITEM_NAME))
                });
            }
            await table.CloseAsync();
            return items;
        }
    }
}
