using MySqlConnector;
using ServerCode.Models;
using static Repositories.DBManager;
using System.Transactions;
using System.Data.Common;

namespace Repositories
{
    public class AuctionItemRepository : RepositoryBase<AuctionItemInfo>
    {
        public override async Task<bool> AddAsync(AuctionItemInfo auctionItemInfo, MySqlConnection connection, MySqlTransaction transaction)
        {
            MySqlCommand addNewItem = new MySqlCommand(Queries.AddNewItemToAuction, connection, transaction);
            addNewItem.Parameters.AddWithValue("@playerId", auctionItemInfo.playerId);
            addNewItem.Parameters.AddWithValue("@itemId", auctionItemInfo.itemId);
            addNewItem.Parameters.AddWithValue("@pricePerUnit", auctionItemInfo.pricePerUnit);
            addNewItem.Parameters.AddWithValue("@quantity", auctionItemInfo.quantity);
            return await addNewItem.ExecuteNonQueryAsync() > 0;
        }

        public override Task<bool> DeleteAsync(AuctionItemInfo entity, MySqlConnection connection, MySqlTransaction transaction)
        {
            throw new NotImplementedException();
        }

        public override Task<List<AuctionItemInfo>> GetAllItemsAsync(MySqlConnection connection, MySqlTransaction transaction)
        {
            throw new NotImplementedException();
        }

        public override Task<AuctionItemInfo> GetByIdAsync(string id, MySqlConnection connection, MySqlTransaction transaction)
        {
            throw new NotImplementedException();
        }

        public override async Task<bool> UpdateAsync(AuctionItemInfo auctionItemInfo, MySqlConnection connection, MySqlTransaction transaction)
        {
            MySqlCommand addQuantity = new MySqlCommand(Queries.AddAuctionItemQuantity, connection, transaction);
            addQuantity.Parameters.AddWithValue("@quantity", auctionItemInfo.quantity);
            addQuantity.Parameters.AddWithValue("@playerId", auctionItemInfo.playerId);
            addQuantity.Parameters.AddWithValue("@itemId", auctionItemInfo.itemId);
            return await addQuantity.ExecuteNonQueryAsync() > 0;
        }
    }
}
