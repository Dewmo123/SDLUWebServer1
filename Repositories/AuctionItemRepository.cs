using MySqlConnector;
using ServerCode.Models;
using static Repositories.DBConfig;
using System.Transactions;
using System.Data.Common;

namespace Repositories
{
    public class AuctionItemRepository : IRepository<AuctionItemInfo>
    {
        public async Task<bool> AddAsync(AuctionItemInfo auctionItemInfo, MySqlConnection connection, MySqlTransaction transaction)
        {
            MySqlCommand addNewItem = new MySqlCommand(Queries.AddNewItemToAuction, connection, transaction);
            addNewItem.Parameters.AddWithValue("@playerId", auctionItemInfo.playerId);
            addNewItem.Parameters.AddWithValue("@itemId", auctionItemInfo.itemId);
            addNewItem.Parameters.AddWithValue("@pricePerUnit", auctionItemInfo.pricePerUnit);
            addNewItem.Parameters.AddWithValue("@quantity", auctionItemInfo.quantity);
            return await addNewItem.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> DeleteAsync(AuctionItemInfo entity, MySqlConnection connection, MySqlTransaction transaction)
        {
            throw new NotImplementedException();
        }

        public async Task<List<AuctionItemInfo>> GetAllItemsAsync(MySqlConnection connection, MySqlTransaction transaction)
        {
            throw new NotImplementedException();
        }

        public async Task<AuctionItemInfo> GetByIdAsync(AuctionItemInfo auctionItem, MySqlConnection connection, MySqlTransaction transaction)
        {
            MySqlCommand getAuctionItem = new MySqlCommand(Queries.GetAuctionItemById, connection, transaction);
            getAuctionItem.Parameters.AddWithValue("@playerId", auctionItem.playerId);
            getAuctionItem.Parameters.AddWithValue("@itemId", auctionItem.itemId);
            getAuctionItem.Parameters.AddWithValue("@pricePerUnit", auctionItem.pricePerUnit);
            var table = await getAuctionItem.ExecuteReaderAsync();
            AuctionItemInfo? info = null;
            if (await table.ReadAsync())
            {
                info = new AuctionItemInfo()
                {
                    itemId = table.GetInt32(table.GetOrdinal(PRICE_PER_UNIT)),
                    playerId = table.GetString(table.GetOrdinal(PLAYER_ID)),
                    pricePerUnit = table.GetInt32(table.GetOrdinal(PRICE_PER_UNIT)),
                    quantity = table.GetInt32(table.GetOrdinal(QUANTITY))
                };
            }
            await table.CloseAsync();
            return info;
        }

        public async Task<bool> UpdateAsync(AuctionItemInfo auctionItemInfo, MySqlConnection connection, MySqlTransaction transaction)
        {
            MySqlCommand addQuantity = new MySqlCommand(Queries.UpdateAuctionItemQuantity, connection, transaction);
            addQuantity.Parameters.AddWithValue("@quantity", auctionItemInfo.quantity);
            addQuantity.Parameters.AddWithValue("@playerId", auctionItemInfo.playerId);
            addQuantity.Parameters.AddWithValue("@itemId", auctionItemInfo.itemId);
            return await addQuantity.ExecuteNonQueryAsync() > 0;
        }
    }
}
