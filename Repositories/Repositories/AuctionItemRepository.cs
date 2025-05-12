using MySqlConnector;
using ServerCode.DAO;
using static Repositories.DBConfig;
using System.Transactions;
using System.Data.Common;
using Repositories;

namespace DataAccessLayer.Repositories
{
    public interface IAuctionRepository : IRepository<AuctionItemVO>
    {
        public Task<List<AuctionItemVO>> GetItemsByItemName(string itemName, MySqlConnection connection);
        public Task<List<AuctionItemVO>> GetItemsByPlayerId(string playerId, MySqlConnection connection);
    }
    public class AuctionItemRepository : IAuctionRepository
    {
        public async Task<bool> AddAsync(AuctionItemVO auctionItemInfo, MySqlConnection connection, MySqlTransaction transaction)
        {
            MySqlCommand addNewItem = new MySqlCommand(
                $"INSERT INTO {AUCTION_DATA_TABLE} ({PLAYER_ID},{PRICE_PER_UNIT},{QUANTITY},{ITEM_NAME})" +
                $" VALUES (@playerId,@pricePerUnit,@quantity,@itemName)", connection, transaction);
            addNewItem.Parameters.AddWithValue("@playerId", auctionItemInfo.playerId);
            addNewItem.Parameters.AddWithValue("@pricePerUnit", auctionItemInfo.pricePerUnit);
            addNewItem.Parameters.AddWithValue("@quantity", auctionItemInfo.quantity);
            addNewItem.Parameters.AddWithValue("@itemName", auctionItemInfo.itemName);
            return await addNewItem.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> DeleteWithPrimaryKeysAsync(AuctionItemVO entity, MySqlConnection connection, MySqlTransaction transaction)
        {
            MySqlCommand deleteAuctionItem = new MySqlCommand(
                $"DELETE FROM {AUCTION_DATA_TABLE}" +
                $" WHERE {PLAYER_ID} = @playerId AND {PRICE_PER_UNIT} = @pricePerUnit AND {ITEM_NAME} = @itemName", connection, transaction);
            deleteAuctionItem.Parameters.AddWithValue("@playerId", entity.playerId);
            deleteAuctionItem.Parameters.AddWithValue("@pricePerUnit", entity.pricePerUnit);
            deleteAuctionItem.Parameters.AddWithValue("@itemName", entity.itemName);
            return await deleteAuctionItem.ExecuteNonQueryAsync() == 1;
        }

        public Task<List<AuctionItemVO>> GetAllItemsAsync(MySqlConnection connection)
        {
            throw new NotImplementedException();
        }

        public async Task<AuctionItemVO> GetItemByPrimaryKeysAsync(AuctionItemVO auctionItem, MySqlConnection connection)
        {
            MySqlCommand getAuctionItem = new MySqlCommand(
                $"SELECT * FROM {AUCTION_DATA_TABLE}" +
                $" WHERE {PLAYER_ID} = @playerId AND {ITEM_NAME} = @itemName AND {PRICE_PER_UNIT} = @pricePerUnit", connection);
            getAuctionItem.Parameters.AddWithValue("@playerId", auctionItem.playerId);
            getAuctionItem.Parameters.AddWithValue("@pricePerUnit", auctionItem.pricePerUnit);
            getAuctionItem.Parameters.AddWithValue("@itemName", auctionItem.itemName);
            var table = await getAuctionItem.ExecuteReaderAsync();
            AuctionItemVO info = new();
            if (await table.ReadAsync())
            {
                info = new AuctionItemVO()
                {
                    playerId = table.GetString(table.GetOrdinal(PLAYER_ID)),
                    pricePerUnit = table.GetInt32(table.GetOrdinal(PRICE_PER_UNIT)),
                    quantity = table.GetInt32(table.GetOrdinal(QUANTITY)),
                    itemName = table.GetString(table.GetOrdinal(ITEM_NAME))
                };
            }
            await table.CloseAsync();
            return info;
        }

        public async Task<bool> UpdateAsync(AuctionItemVO auctionItemInfo, MySqlConnection connection, MySqlTransaction transaction)
        {
            MySqlCommand addQuantity = new MySqlCommand(
                $"UPDATE {AUCTION_DATA_TABLE} SET {QUANTITY} = @quantity " +
                $"WHERE {PLAYER_ID} = @playerId AND {ITEM_NAME} = @itemName AND {PRICE_PER_UNIT} = @pricePerUnit", connection, transaction);
            addQuantity.Parameters.AddWithValue("@quantity", auctionItemInfo.quantity);
            addQuantity.Parameters.AddWithValue("@playerId", auctionItemInfo.playerId);
            addQuantity.Parameters.AddWithValue("@itemName", auctionItemInfo.itemName);
            addQuantity.Parameters.AddWithValue("@pricePerUnit", auctionItemInfo.pricePerUnit);
            return await addQuantity.ExecuteNonQueryAsync() > 0;
        }
        public async Task<List<AuctionItemVO>> GetItemsByItemName(string itemName, MySqlConnection connection)
        {
            MySqlCommand getItems = new MySqlCommand($"SELECT * FROM {AUCTION_DATA_TABLE} WHERE {ITEM_NAME} = @itemName",connection);
            getItems.Parameters.AddWithValue("@itemName", itemName);
            var table = await getItems.ExecuteReaderAsync();
            List<AuctionItemVO> items = new List<AuctionItemVO>();
            while (await table.ReadAsync())
            {
                items.Add(new AuctionItemVO()
                {
                    playerId = table.GetString(table.GetOrdinal(PLAYER_ID)),
                    quantity = table.GetInt32(table.GetOrdinal(QUANTITY)),
                    pricePerUnit = table.GetInt32(table.GetOrdinal(PRICE_PER_UNIT)),
                    itemName = itemName
                });
            }
            await table.CloseAsync();
            return items;
        }

        public async Task<List<AuctionItemVO>> GetItemsByPlayerId(string playerId, MySqlConnection connection)
        {
            MySqlCommand getItems = new MySqlCommand($"SELECT * FROM {AUCTION_DATA_TABLE} WHERE {PLAYER_ID} = @playerId", connection);
            getItems.Parameters.AddWithValue("@playerId", playerId);
            var table = await getItems.ExecuteReaderAsync();
            List<AuctionItemVO> items = new List<AuctionItemVO>();
            while (await table.ReadAsync())
            {
                items.Add(new AuctionItemVO()
                {
                    playerId = table.GetString(table.GetOrdinal(PLAYER_ID)),
                    quantity = table.GetInt32(table.GetOrdinal(QUANTITY)),
                    pricePerUnit = table.GetInt32(table.GetOrdinal(PRICE_PER_UNIT)),
                    itemName = table.GetString(table.GetOrdinal(ITEM_NAME))
                });
            }
            await table.CloseAsync();
            return items;
        }
    }
}
