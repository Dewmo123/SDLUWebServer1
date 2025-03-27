using MySqlConnector;
using Repositories;
using ServerCode.Models;
using static Repositories.DBConfig;
namespace DataAccessLayer.Repositories
{
    public interface IItemInfoRepository : IRepository<ItemInfo>
    {
        public Task<List<ItemInfo>> GetItemInfoWithType(ItemType type, MySqlConnection conn);
    }
    public class ItemInfoRepository : IItemInfoRepository
    {
        public async Task<bool> AddAsync(ItemInfo itemInfo, MySqlConnection connection, MySqlTransaction transaction)
        {
            MySqlCommand command = new MySqlCommand(
                $"INSERT INTO {ITEM_DATA_TABLE} ({ITEM_NAME},{ITEM_TYPE}) " +
                $"VALUES (@itemName,@type)", connection, transaction);
            command.Parameters.AddWithValue("@itemName", itemInfo.itemName);
            command.Parameters.AddWithValue("@type", (int)itemInfo.itemType);
            var table = await command.ExecuteNonQueryAsync();
            if (table != 1)
                return false;
            return true;
        }

        public async Task<bool> DeleteWithPrimaryKeysAsync(ItemInfo itemInfo, MySqlConnection connection, MySqlTransaction transaction)
        {
            MySqlCommand command = new MySqlCommand($"DELETE FROM {ITEM_DATA_TABLE} WHERE {ITEM_NAME}=@itemName", connection, transaction);
            command.Parameters.AddWithValue("@itemName", itemInfo.itemName);
            var table = await command.ExecuteNonQueryAsync();
            if (table != 1)
                return false;
            return true;
        }

        public async Task<List<ItemInfo>> GetAllItemsAsync(MySqlConnection connection)
        {
            MySqlCommand command = new MySqlCommand($"SELECT * FROM {ITEM_DATA_TABLE}", connection);
            var table = await command.ExecuteReaderAsync();
            List<ItemInfo> infos = new List<ItemInfo>();
            while (table.Read())
            {
                int id = table.GetInt32(table.GetOrdinal(ITEM_NAME));
                string name = table.GetString(table.GetOrdinal(ITEM_NAME));
                ItemType type = Enum.Parse<ItemType>(table.GetString(table.GetOrdinal(ITEM_TYPE)));
                int maxStack = table.GetInt32(table.GetOrdinal(ITEM_MAX_STACK));
                Console.WriteLine(name);
                infos.Add(new ItemInfo(name, type));
            }
            return infos;
        }

        public Task<ItemInfo> GetItemByPrimaryKeysAsync(ItemInfo itemInfo, MySqlConnection connection)
        {
            throw new NotImplementedException();
        }

        public async Task<List<ItemInfo>> GetItemInfoWithType(ItemType type, MySqlConnection conn)
        {
            MySqlCommand command = new MySqlCommand(
                $"SELECT * FROM {ITEM_DATA_TABLE} " +
                $"WHERE {ITEM_TYPE} = @itemType", conn);
            command.Parameters.AddWithValue("@itemType", (int)type);
            var table = await command.ExecuteReaderAsync();
            List<ItemInfo> items = new List<ItemInfo>();
            while (await table.ReadAsync())
            {
                ItemInfo item = new()
                {
                    itemType = Enum.Parse<ItemType>(table.GetString(table.GetOrdinal(ITEM_TYPE))),
                    itemName = table.GetString(table.GetOrdinal(ITEM_NAME))
                };
                items.Add(item);
            }
            await table.CloseAsync();
            return items;
        }

        public Task<bool> UpdateAsync(ItemInfo entity, MySqlConnection connection, MySqlTransaction transaction)
        {
            throw new NotImplementedException();
        }
    }
}
