using MySqlConnector;
using ServerCode.Models;
using static Repositories.DBManager;
using System.Transactions;
using System.Data.Common;

namespace Repositories
{
    public class AuctionItemRepository : UseTransactionRepository<AuctionItemInfo>
    {
        public AuctionItemRepository(string address) : base(address)
        {
        }

        public override async Task<bool> AddAsync(AuctionItemInfo auctionItemInfo)
        {
            MySqlCommand addNewItem = new MySqlCommand(Queries.AddNewItemToAuction, _connection, _transaction);
            addNewItem.Parameters.AddWithValue("@playerId", auctionItemInfo.playerId);
            addNewItem.Parameters.AddWithValue("@itemId", auctionItemInfo.itemId);
            addNewItem.Parameters.AddWithValue("@pricePerUnit", auctionItemInfo.pricePerUnit);
            addNewItem.Parameters.AddWithValue("@quantity", auctionItemInfo.quantity);
            return await addNewItem.ExecuteNonQueryAsync() > 0;
        }

        public override Task<bool> DeleteAsync(AuctionItemInfo entity)
        {
            throw new NotImplementedException();
        }

        public override Task<List<AuctionItemInfo>> GetAllItemsAsync()
        {
            throw new NotImplementedException();
        }

        public override Task<AuctionItemInfo> GetByIdAsync(string id)
        {
            throw new NotImplementedException();
        }

        public override async Task<bool> UpdateAsync(AuctionItemInfo auctionItemInfo)
        {
            MySqlCommand addQuantity = new MySqlCommand(Queries.AddAuctionItemQuantity, _connection, _transaction);
            addQuantity.Parameters.AddWithValue("@quantity", auctionItemInfo.quantity);
            addQuantity.Parameters.AddWithValue("@playerId", auctionItemInfo.playerId);
            addQuantity.Parameters.AddWithValue("@itemId", auctionItemInfo.itemId);
            return await addQuantity.ExecuteNonQueryAsync() > 0;
        }
    }
}
