using MySqlConnector;
using ServerCode.Models;
using static Repositories.DBManager;

namespace Repositories
{
    public class PlayerItemRepostory : UseTransactionRepository<PlayerItemInfo>
    {
        public PlayerItemRepostory(string address) : base(address)
        {
        }

        public override async Task<bool> AddAsync(PlayerItemInfo itemInfo)
        {
            if (itemInfo.quantity <= 0) return false;

            var cmd = new MySqlCommand(Queries.InsertItem, _connection, _transaction);
            cmd.Parameters.AddWithValue("@playerId", itemInfo.playerId);
            cmd.Parameters.AddWithValue("@itemId", itemInfo.itemId);
            cmd.Parameters.AddWithValue("@quantity", itemInfo.quantity);

            return await cmd.ExecuteNonQueryAsync() > 0;
        }
        public async override Task<bool> DeleteAsync(PlayerItemInfo itemInfo)
        {
            var cmd = new MySqlCommand(Queries.DeleteItem, _connection, _transaction);
            cmd.Parameters.AddWithValue("@playerId", itemInfo.playerId);
            cmd.Parameters.AddWithValue("@itemId", itemInfo.itemId);

            return await cmd.ExecuteNonQueryAsync() > 0;
        }

        public override Task<List<PlayerItemInfo>> GetAllItemsAsync()
        {
            throw new NotImplementedException();
        }

        public override Task<PlayerItemInfo> GetByIdAsync(string id)
        {
            throw new NotImplementedException();
        }

        public override async Task<bool> UpdateAsync(PlayerItemInfo itemInfo)
        {
            var cmd = new MySqlCommand(Queries.UpdateItemQuantity, _connection, _transaction);
            cmd.Parameters.AddWithValue("@playerId", itemInfo.playerId);
            cmd.Parameters.AddWithValue("@itemId", itemInfo.itemId);
            cmd.Parameters.AddWithValue("@quantity", itemInfo.quantity);

            return await cmd.ExecuteNonQueryAsync() > 0;
        }
    }
}
