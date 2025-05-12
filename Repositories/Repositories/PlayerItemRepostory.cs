using MySqlConnector;
using Repositories;
using ServerCode.DAO;
using static Repositories.DBConfig;

namespace DataAccessLayer.Repositories
{
    public interface IPlayerItemRepository : IRepository<PlayerItemVO>
    {
        public Task<bool> CheckConditionAndChangePlayerItem(PlayerItemVO itemInfo, PlayerItemVO remainItem, MySqlConnection connection, MySqlTransaction transaction);
        public Task<List<PlayerItemVO>> GetItemsByPlayerId(string playerId, MySqlConnection connection);
        public Task<PlayerItemVO> GetItemWithTransactionAsync(PlayerItemVO entity, MySqlConnection connection, MySqlTransaction transaction);

    }
    public class PlayerItemRepostory : IPlayerItemRepository
    {

        public async Task<bool> AddAsync(PlayerItemVO itemInfo, MySqlConnection connection, MySqlTransaction transaction)
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
        public async Task<bool> DeleteWithPrimaryKeysAsync(PlayerItemVO itemInfo, MySqlConnection connection, MySqlTransaction transaction)
        {
            var cmd = new MySqlCommand(
                $"DELETE FROM {PLAYER_ITEM_TABLE} " +
                $"WHERE {PLAYER_ID} = @playerId AND {ITEM_NAME} = @itemName", connection, transaction);
            cmd.Parameters.AddWithValue("@playerId", itemInfo.playerId);
            cmd.Parameters.AddWithValue("@itemName", itemInfo.itemName);

            return await cmd.ExecuteNonQueryAsync() > 0;
        }

        public Task<List<PlayerItemVO>> GetAllItemsAsync(MySqlConnection connection)
        {
            throw new NotImplementedException();
        }

        public async Task<PlayerItemVO?> GetItemByPrimaryKeysAsync(PlayerItemVO itemInfo, MySqlConnection connection)
        {
            var cmd = new MySqlCommand(
                $"SELECT * FROM {PLAYER_ITEM_TABLE}" +
                $" WHERE {PLAYER_ID} = @playerId AND {ITEM_NAME} = @itemName", connection);
            cmd.Parameters.AddWithValue("@playerId", itemInfo.playerId);
            cmd.Parameters.AddWithValue("@itemName", itemInfo.itemName);

            var table = await cmd.ExecuteReaderAsync();
            PlayerItemVO? info = null;
            if (await table.ReadAsync())
            {
                info = new PlayerItemVO
                {
                    playerId = table.GetString(table.GetOrdinal(PLAYER_ID)),
                    quantity = table.GetInt32(table.GetOrdinal(QUANTITY)),
                    itemName = table.GetString(table.GetOrdinal(ITEM_NAME))
                };
            }
            await table.CloseAsync();
            return info;
        }

        public async Task<bool> UpdateAsync(PlayerItemVO itemInfo, MySqlConnection connection, MySqlTransaction transaction)
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
        public async Task<bool> CheckConditionAndChangePlayerItem(PlayerItemVO itemInfo, PlayerItemVO remainItem, MySqlConnection connection, MySqlTransaction transaction)
        {
            if (remainItem == null && itemInfo.quantity > 0)
                return await AddAsync(itemInfo, connection, transaction);

            int quantity = remainItem.quantity + itemInfo.quantity;
            if (quantity < 0)
                return false;
            remainItem.quantity = quantity;
            return await UpdateAsync(remainItem, connection, transaction);
        }
        public async Task<List<PlayerItemVO>> GetItemsByPlayerId(string playerId, MySqlConnection connection)
        {
            MySqlCommand selectItems = new MySqlCommand(
                $"SELECT * FROM {PLAYER_ITEM_TABLE}" +
                $" WHERE {PLAYER_ID} = @playerId", connection);
            selectItems.Parameters.AddWithValue("@playerId", playerId);
            var table = await selectItems.ExecuteReaderAsync();
            List<PlayerItemVO> items = new List<PlayerItemVO>();
            while (await table.ReadAsync())
            {
                items.Add(new PlayerItemVO()
                {
                    playerId = table.GetString(table.GetOrdinal(PLAYER_ID)),
                    quantity = table.GetInt32(table.GetOrdinal(QUANTITY)),
                    itemName = table.GetString(table.GetOrdinal(ITEM_NAME))
                });
            }
            await table.CloseAsync();
            return items;
        }

        public async Task<PlayerItemVO> GetItemWithTransactionAsync(PlayerItemVO entity, MySqlConnection connection, MySqlTransaction transaction)
        {
            MySqlCommand getItem = new MySqlCommand(
                $"SELECT * FROM {PLAYER_ITEM_TABLE} " +
                $"WHERE {PLAYER_ID} = @playerId AND {ITEM_NAME} = @itemName", connection, transaction);
            getItem.Parameters.AddWithValue("@playerId", entity.playerId);
            getItem.Parameters.AddWithValue("@itemName", entity.itemName);

            var table = await getItem.ExecuteReaderAsync();
            PlayerItemVO? info = null;
            if (await table.ReadAsync())
            {
                info = new PlayerItemVO
                {
                    playerId = table.GetString(table.GetOrdinal(PLAYER_ID)),
                    quantity = table.GetInt32(table.GetOrdinal(QUANTITY)),
                    itemName = table.GetString(table.GetOrdinal(ITEM_NAME))
                };
            }
            await table.CloseAsync();
            return info;
        }
    }
}
