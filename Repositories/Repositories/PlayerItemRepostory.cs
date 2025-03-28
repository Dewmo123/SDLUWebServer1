using MySqlConnector;
using Repositories;
using ServerCode.Models;
using static Repositories.DBConfig;

namespace DataAccessLayer.Repositories
{
    public interface IPlayerItemRepository : IRepository<PlayerItemInfo>
    {
        public Task<bool> CheckConditionAndChangePlayerItem(PlayerItemInfo itemInfo,PlayerItemInfo remainItem, MySqlConnection conn, MySqlTransaction transaction);
        public Task<List<PlayerItemInfo>> GetItemsByPlayerId(string playerId, MySqlConnection connection);
    }
    public class PlayerItemRepostory : IPlayerItemRepository
    {

        public async Task<bool> AddAsync(PlayerItemInfo itemInfo, MySqlConnection connection, MySqlTransaction transaction)
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
        public async Task<bool> DeleteWithPrimaryKeysAsync(PlayerItemInfo itemInfo, MySqlConnection connection, MySqlTransaction transaction)
        {
            var cmd = new MySqlCommand(
                $"DELETE FROM {PLAYER_ITEM_TABLE} " +
                $"WHERE {PLAYER_ID} = @playerId AND {ITEM_NAME} = @itemName", connection, transaction);
            cmd.Parameters.AddWithValue("@playerId", itemInfo.playerId);
            cmd.Parameters.AddWithValue("@itemName", itemInfo.itemName);

            return await cmd.ExecuteNonQueryAsync() > 0;
        }

        public Task<List<PlayerItemInfo>> GetAllItemsAsync(MySqlConnection connection)
        {
            throw new NotImplementedException();
        }

        public async Task<PlayerItemInfo> GetItemByPrimaryKeysAsync(PlayerItemInfo itemInfo, MySqlConnection connection)
        {
            var cmd = new MySqlCommand(
                $"SELECT * FROM {PLAYER_ITEM_TABLE}" +
                $" WHERE {PLAYER_ID} = @playerId AND {ITEM_NAME} = @itemName", connection);
            cmd.Parameters.AddWithValue("@playerId", itemInfo.playerId);
            cmd.Parameters.AddWithValue("@itemName", itemInfo.itemName);

            var table = await cmd.ExecuteReaderAsync();
            PlayerItemInfo? info = null;
            if (await table.ReadAsync())
            {
                info = new PlayerItemInfo
                {
                    playerId = table.GetString(table.GetOrdinal(PLAYER_ID)),
                    quantity = table.GetInt32(table.GetOrdinal(QUANTITY)),
                    itemName = table.GetString(table.GetOrdinal(ITEM_NAME))
                };
            }
            await table.CloseAsync();
            return info;
        }

        public async Task<bool> UpdateAsync(PlayerItemInfo itemInfo, MySqlConnection connection, MySqlTransaction transaction)
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
        public async Task<bool> CheckConditionAndChangePlayerItem(PlayerItemInfo itemInfo,PlayerItemInfo remainItem, MySqlConnection conn, MySqlTransaction transaction)
        {
            if (remainItem == null && itemInfo.quantity > 0)
                return await AddAsync(itemInfo, conn, transaction);

            int quantity = remainItem.quantity + itemInfo.quantity;
            if (quantity < 0)
                return false;
            remainItem.quantity = quantity;
            return await UpdateAsync(remainItem, conn, transaction);
        }

        public async Task<List<PlayerItemInfo>> GetItemsByPlayerId(string playerId, MySqlConnection connection)
        {
            MySqlCommand selectItems = new MySqlCommand(
                $"SELECT * FROM {PLAYER_ITEM_TABLE}" +
                $" WHERE {PLAYER_ID} = @playerId", connection);
            selectItems.Parameters.AddWithValue("@playerId", playerId);
            var table = await selectItems.ExecuteReaderAsync();
            List<PlayerItemInfo> items = new List<PlayerItemInfo>();
            while (await table.ReadAsync())
            {
                items.Add(new PlayerItemInfo()
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
