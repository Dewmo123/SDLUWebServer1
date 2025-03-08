using MySqlConnector;
using ServerCode.Models;
using static Repositories.DBManager;

namespace Repositories
{
    public class PlayerItemRepostory : RepositoryBase<PlayerItemInfo>
    {

        public override async Task<bool> AddAsync(PlayerItemInfo itemInfo, MySqlConnection connection, MySqlTransaction transaction)
        {
            if (itemInfo.quantity <= 0) return false;

            var cmd = new MySqlCommand(Queries.InsertItem, connection, transaction);
            cmd.Parameters.AddWithValue("@playerId", itemInfo.playerId);
            cmd.Parameters.AddWithValue("@itemId", itemInfo.itemId);
            cmd.Parameters.AddWithValue("@quantity", itemInfo.quantity);

            return await cmd.ExecuteNonQueryAsync() > 0;
        }
        public async override Task<bool> DeleteAsync(PlayerItemInfo itemInfo, MySqlConnection connection, MySqlTransaction transaction)
        {
            var cmd = new MySqlCommand(Queries.DeleteItem, connection, transaction);
            cmd.Parameters.AddWithValue("@playerId", itemInfo.playerId);
            cmd.Parameters.AddWithValue("@itemId", itemInfo.itemId);

            return await cmd.ExecuteNonQueryAsync() > 0;
        }

        public override Task<List<PlayerItemInfo>> GetAllItemsAsync(MySqlConnection connection, MySqlTransaction transaction)
        {
            throw new NotImplementedException();
        }

        public override Task<PlayerItemInfo> GetByIdAsync(string id, MySqlConnection connection, MySqlTransaction transaction)
        {
            throw new NotImplementedException();
        }

        public override async Task<bool> UpdateAsync(PlayerItemInfo itemInfo, MySqlConnection connection, MySqlTransaction transaction)
        {
            var cmd = new MySqlCommand(Queries.UpdateItemQuantity, connection, transaction);
            cmd.Parameters.AddWithValue("@playerId", itemInfo.playerId);
            cmd.Parameters.AddWithValue("@itemId", itemInfo.itemId);
            cmd.Parameters.AddWithValue("@quantity", itemInfo.quantity);

            return await cmd.ExecuteNonQueryAsync() > 0;
        }
    }
}
