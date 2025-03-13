using MySqlConnector;
using Repositories;
using ServerCode.Models;

namespace DataAccessLayer.Repositories
{
    public class ItemInfoRepository : IRepository<ItemInfo>
    {
        public async Task<bool> AddAsync(ItemInfo itemInfo, MySqlConnection connection, MySqlTransaction transaction)
        {
            using (MySqlConnection conn = new MySqlConnection())
            {
                conn.Open();
                MySqlCommand command = new MySqlCommand($"INSERT INTO {DBConfig.ITEM_DATA_TABLE} ({DBConfig.ITEM_NAME},{DBConfig.ITEM_TYPE},{DBConfig.ITEM_MAX_STACK}) VALUES (@itemName,@type,@maxStack)", conn);
                command.Parameters.AddWithValue("@itemName", itemInfo.itemName);
                command.Parameters.AddWithValue("@type", (int)itemInfo.itemType);
                command.Parameters.AddWithValue("@maxStack", itemInfo.itemMaxStack);
                var table = await command.ExecuteNonQueryAsync();
                conn.Close();
                if (table != 1)
                    return false;
                return true;
            }
        }

        public async Task<bool> DeleteWithPrimaryKeysAsync(ItemInfo itemInfo, MySqlConnection connection, MySqlTransaction transaction)
        {
            MySqlCommand command = new MySqlCommand($"DELETE FROM {DBConfig.ITEM_DATA_TABLE} WHERE {DBConfig.ITEM_ID}=@itemId", connection, transaction);
            command.Parameters.AddWithValue("@itemId", itemInfo.itemId);
            var table = await command.ExecuteNonQueryAsync();
            if (table != 1)
                return false;
            return true;
        }

        public async Task<List<ItemInfo>> GetAllItemsAsync(MySqlConnection connection, MySqlTransaction transaction)
        {
            MySqlCommand command = new MySqlCommand($"SELECT * FROM {DBConfig.ITEM_DATA_TABLE}", connection);
            var table = await command.ExecuteReaderAsync();
            List<ItemInfo> infos = new List<ItemInfo>();
            while (table.Read())
            {
                int id = table.GetInt32(table.GetOrdinal(DBConfig.ITEM_ID));
                string name = table.GetString(table.GetOrdinal(DBConfig.ITEM_NAME));
                ItemType type = Enum.Parse<ItemType>(table.GetString(table.GetOrdinal(DBConfig.ITEM_TYPE)));
                int maxStack = table.GetInt32(table.GetOrdinal(DBConfig.ITEM_MAX_STACK));
                Console.WriteLine(name);
                infos.Add(new ItemInfo(id, name, type, maxStack));
            }
            return infos;
        }

        public Task<ItemInfo> GetItemByPrimaryKeysAsync(ItemInfo itemInfo, MySqlConnection connection, MySqlTransaction transaction)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(ItemInfo entity, MySqlConnection connection, MySqlTransaction transaction)
        {
            throw new NotImplementedException();
        }
    }
}
