using MySqlConnector;
using ServerCode.Models;

namespace Repositories
{
    public class ItemInfoRepository : IRepository<ItemInfo>
    {
        private readonly string _dbAddress;
        public ItemInfoRepository(string address)
        {
            _dbAddress = address;
        }
        public async Task<bool> AddAsync(ItemInfo itemInfo)
        {
            using (MySqlConnection conn = new MySqlConnection())
            {
                conn.Open();
                MySqlCommand command = new MySqlCommand($"INSERT INTO {DBManager.ITEM_DATA_TABLE} ({DBManager.ITEM_NAME},{DBManager.ITEM_TYPE},{DBManager.ITEM_MAX_STACK}) VALUES (@itemName,@type,@maxStack)", conn);
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

        public async Task<bool> DeleteAsync(ItemInfo itemInfo)
        {
            using (MySqlConnection conn = new MySqlConnection(_dbAddress))
            {
                conn.Open();
                MySqlCommand command = new MySqlCommand($"DELETE FROM {DBManager.ITEM_DATA_TABLE} WHERE {DBManager.ITEM_ID}=@itemId", conn);
                command.Parameters.AddWithValue("@itemId", itemInfo.itemId);
                var table = await command.ExecuteNonQueryAsync();
                conn.Close();
                if (table != 1)
                    return false;
                return true;
            }
        }

        public async Task<List<ItemInfo>> GetAllItemsAsync()
        {
            using (MySqlConnection conn = new MySqlConnection(_dbAddress))
            {
                conn.Open();
                MySqlCommand command = new MySqlCommand($"SELECT * FROM {DBManager.ITEM_DATA_TABLE}", conn);
                var table = await command.ExecuteReaderAsync();
                List<ItemInfo> infos = new List<ItemInfo>();
                while (table.Read())
                {
                    int id = table.GetInt32(table.GetOrdinal(DBManager.ITEM_ID));
                    string name = table.GetString(table.GetOrdinal(DBManager.ITEM_NAME));
                    ItemType type = Enum.Parse<ItemType>(table.GetString(table.GetOrdinal(DBManager.ITEM_TYPE)));
                    int maxStack = table.GetInt32(table.GetOrdinal(DBManager.ITEM_MAX_STACK));
                    Console.WriteLine(name);
                    infos.Add(new ItemInfo(id, name, type, maxStack));
                }
                conn.Close();
                return infos;
            }
        }

        public Task<ItemInfo> GetByIdAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(ItemInfo entity)
        {
            throw new NotImplementedException();
        }
    }
}
