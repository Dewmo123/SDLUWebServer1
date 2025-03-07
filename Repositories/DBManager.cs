using MySqlConnector;
using ServerCode.Core;
using ServerCode.Models;
using System.Reflection.Metadata;
using System.Threading.Tasks;

namespace Repositories
{

    public class DBManager : Singleton<DBManager>
    {
        string _dbAddress = null!;
        #region playerData
        /// <summary>
        /// 플레이어 데이터 테이블의 이름입니다. 플레이어 데이터 테이블은 playerid와 password를 속성으로 가집니다(varchar)
        /// </summary>
        public const string PLAYER_DATA_TABLE = "player_login_data";
        public const string PLAYER_ID = "player_id";
        public const string PASSWORD = "password";
        /// <summary>
        /// 플레이어 아이템 테이블의 이름입니다. 플레이어 아이템 테이블은 ItemId(int), quantity(int)를 속성으로 가집니다
        /// </summary>
        public const string PLAYER_ITEM_TABLE = "player_item_data";
        public const string QUANTITY = "quantity";
        #endregion

        #region ItemData
        /// <summary>
        /// 아이템 데이터 테이블의 이름입니다. 아이템 데이터 테이블은 itemId(int,auto_increment), itemName(varchar), itemType(enum), maxStack(int)을 속성으로 가집니다
        /// </summary>
        public const string ITEM_DATA_TABLE = "item_data";
        public const string ITEM_ID = "item_id";
        public const string ITEM_NAME = "item_name";
        public const string ITEM_TYPE = "item_type";
        public const string ITEM_MAX_STACK = "max_stack";
        #endregion

        #region AuctionData
        /// <summary>
        /// 경매 데이터 테이블의 이름입니다. 경매 데이터 테이블은 playerId(varchar), itemId(int), pricePerUnit(int), quantity(int)를 속성으로 가집니다
        /// </summary>
        public const string AUCTION_DATA_TABLE = "auction";
        public const string PRICE_PER_UNIT = "price_per_unit";
        #endregion
        public void ConnectDB(string connectionAddress)
        {
            _dbAddress = connectionAddress;
        }

        #region PlayerControl


        public bool SignUp(PlayerInfo playerInfo)
        {
            return true;
        }
        public bool LogIn(PlayerInfo playerInfo)
        {
            return true;
        }

        #endregion

        #region ItemControl
        public bool AddItemInfo(ItemInfo itemInfo)
        {
            return true;
        }
        public bool RemoveItemInfo(int itemId)
        {
            return true;
        }
        public List<ItemInfo> GetItemInfos()
        {
            return null!;
        }
        #endregion

        #region PlayerItemControl
        public bool AddItemToPlayer(PlayerItemInfo itemInfo)
        {
            using var conn = new MySqlConnection(_dbAddress);
            try
            {
                conn.Open();
                using var transaction = conn.BeginTransaction();
                try
                {
                    if (TryUpdateExistingItem(conn, transaction, itemInfo) ||
                        TryAddNewItem(conn, transaction, itemInfo))
                    {
                        transaction.Commit();
                        conn.Close();
                        return true;
                    }
                    conn.Close();
                    return false;
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
            catch (Exception ex)
            {
                conn.Close();
                Console.WriteLine($"Error in AddItemToPlayer: {ex.Message}");
                return false;
            }
        }

        private bool TryUpdateExistingItem(MySqlConnection conn, MySqlTransaction transaction,
            PlayerItemInfo itemInfo)
        {
            using var cmd = new MySqlCommand(Queries.SelectItemQuantity, conn, transaction);
            cmd.Parameters.AddWithValue("@playerId", itemInfo.playerId);
            cmd.Parameters.AddWithValue("@itemId", itemInfo.itemId);

            using var reader = cmd.ExecuteReader();
            if (!reader.Read()) return false;

            int currentQuantity = reader.GetInt32(0);
            reader.Close();

            int newQuantity = currentQuantity + itemInfo.quantity;
            if (newQuantity < 0) return false;

            return newQuantity == 0
                ? DeleteItem(conn, transaction, itemInfo)
                : UpdateItemQuantity(conn, transaction, itemInfo, currentQuantity);
        }

        private bool TryAddNewItem(MySqlConnection conn, MySqlTransaction transaction,
            PlayerItemInfo itemInfo)
        {
            return true;
        }

        private bool DeleteItem(MySqlConnection conn, MySqlTransaction transaction,
            PlayerItemInfo itemInfo)
        {
            return true;
        }

        private bool UpdateItemQuantity(MySqlConnection conn, MySqlTransaction transaction,
            PlayerItemInfo itemInfo, int currentQuantity)
        {
            //여기서 itemInfo에 더해서 보내주기
            itemInfo.quantity += currentQuantity;
            return true;
        }

        #endregion

        #region AuctionControl
        public bool AddItemToAuction(AuctionItemInfo auctionItemInfo)
        {
            using (MySqlConnection conn = new MySqlConnection(_dbAddress))
            {
                conn.Open();
                using var transaction = conn.BeginTransaction();
                try
                {
                    //플레이어가 충분한 양을 갖고있는지 판별 & 빼주기

                    MySqlDataReader table = CheckAlreadyExistInAuction(conn, transaction, auctionItemInfo.playerId, auctionItemInfo.itemId);
                    if (!TryUpdateExistingItem(conn, transaction, new PlayerItemInfo(auctionItemInfo)))
                        return false;
                    //옥션에 올리기
                    bool success = false;
                    if (table.Read())
                        success = AddAuctionItemQuantity(conn, transaction, auctionItemInfo);
                    else
                        success = AddNewItemToAuction(conn, transaction, auctionItemInfo);
                    transaction.Commit();
                    conn.Close();
                    return success;
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    conn.Close();
                    return false;
                }
            }
        }


        private bool AddNewItemToAuction(MySqlConnection conn, MySqlTransaction transaction, AuctionItemInfo auctionItemInfo)
        {
            return true;
        }

        private bool AddAuctionItemQuantity(MySqlConnection conn, MySqlTransaction transaction, AuctionItemInfo auctionItemInfo)
        {

            return true;
        }

        private MySqlDataReader CheckAlreadyExistInAuction(MySqlConnection conn, MySqlTransaction transaction, string playerId, int itemId)
        {
            MySqlCommand checkAlreadyExist = new MySqlCommand(Queries.CheckExistInAuction, conn, transaction);
            checkAlreadyExist.Parameters.AddWithValue("@playerId", playerId);
            checkAlreadyExist.Parameters.AddWithValue("@itemId", itemId);
            var table = checkAlreadyExist.ExecuteReader();
            return table;
        }
        #endregion
        #region SQL Queries
        public static class Queries
        {
            public static string SelectItemQuantity =>
                $"SELECT {QUANTITY} FROM {PLAYER_ITEM_TABLE} " +
                $"WHERE {PLAYER_ID} = @playerId AND {ITEM_ID} = @itemId";

            public static string DeleteItem =>
                $"DELETE FROM {PLAYER_ITEM_TABLE} " +
                $"WHERE {PLAYER_ID} = @playerId AND {ITEM_ID} = @itemId";

            public static string UpdateItemQuantity =>
                $"UPDATE {PLAYER_ITEM_TABLE} " +
                $"SET {QUANTITY} = @quantity " +
                $"WHERE {PLAYER_ID} = @playerId AND {ITEM_ID} = @itemId";

            public static string InsertItem =>
                $"INSERT INTO {PLAYER_ITEM_TABLE} ({PLAYER_ID}, {ITEM_ID}, {QUANTITY}) " +
                $"VALUES (@playerId, @itemId, @quantity)";
            public static string AddNewItemToAuction =>
                $"INSERT INTO {AUCTION_DATA_TABLE} ({PLAYER_ID},{ITEM_ID},{PRICE_PER_UNIT},{QUANTITY})" +
                $" VALUES (@playerId,@itemId,@pricePerUnit,@quantity)";
            public static string AddAuctionItemQuantity =>
                $"UPDATE {AUCTION_DATA_TABLE} SET {QUANTITY} = {QUANTITY} + @quantity " +
                $"WHERE {PLAYER_ID} = @playerId AND {ITEM_ID} = @itemId";

            public static string CheckExistInAuction =>
                $"SELECT * FROM {AUCTION_DATA_TABLE}" +
                $" WHERE {PLAYER_ID} = @playerId AND {ITEM_ID} = @itemId";
        }
        #endregion
    }

}
