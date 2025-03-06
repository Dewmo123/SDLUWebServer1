using MySqlConnector;
using ServerCode.Controllers;
using ServerCode.Models;
using System.Reflection.Metadata;

namespace ServerCode.Core
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
        public bool CheckIDDuplication(string playerId)
        {
            using (MySqlConnection conn = new MySqlConnection(_dbAddress))
            {
                conn.Open();
                MySqlCommand command = new MySqlCommand($"SELECT {PLAYER_ID} FROM {PLAYER_DATA_TABLE} WHERE {PLAYER_ID} = @playerId", conn);
                command.Parameters.AddWithValue("@playerId", playerId);
                var table = command.ExecuteReader();
                bool successRead = table.Read();
                conn.Close();
                return successRead;
            }
        }
        public bool SignUp(string playerId, string password)
        {
            if (CheckIDDuplication(playerId))
                return false;
            using (MySqlConnection conn = new MySqlConnection(_dbAddress))
            {
                conn.Open();
                MySqlCommand command = new MySqlCommand($"INSERT INTO {PLAYER_DATA_TABLE} ({PLAYER_ID},{PASSWORD}) VALUES (@playerId,@password)", conn);
                command.Parameters.AddWithValue("@playerId", playerId);
                command.Parameters.AddWithValue("@password", password);
                Console.WriteLine("Sign up new Player");
                var table = command.ExecuteNonQuery();
                conn.Close();
                if (table != 1)
                    return false;
                return true;
            }
        }
        public bool LogIn(string playerId, string password)
        {
            using (MySqlConnection conn = new MySqlConnection(_dbAddress))
            {
                conn.Open();
                MySqlCommand command = new MySqlCommand($"SELECT {PASSWORD} FROM {PLAYER_DATA_TABLE} WHERE {PLAYER_ID} = @playerId", conn);
                command.Parameters.AddWithValue("@playerId", playerId);
                Console.WriteLine("Check password");
                var table = command.ExecuteReader();
                string? pass = null;
                while (table.Read())
                    pass = table["password"].ToString();
                conn.Close();
                return pass == password;
            }
        }

        #endregion

        #region ItemControl
        public bool AddItemInfo(string itemName, ItemType type, int maxStack)
        {
            using (MySqlConnection conn = new MySqlConnection(_dbAddress))
            {
                conn.Open();
                MySqlCommand command = new MySqlCommand($"INSERT INTO {ITEM_DATA_TABLE} ({ITEM_NAME},{ITEM_TYPE},{ITEM_MAX_STACK}) VALUES (@itemName,@type,@maxStack)", conn);
                command.Parameters.AddWithValue("@itemName", itemName);
                command.Parameters.AddWithValue("@type", (int)type);
                command.Parameters.AddWithValue("@maxStack", maxStack);
                var table = command.ExecuteNonQuery();
                conn.Close();
                if (table != 1)
                    return false;
                return true;
            }
        }
        public bool RemoveItemInfo(int itemId)
        {
            using (MySqlConnection conn = new MySqlConnection(_dbAddress))
            {
                conn.Open();
                MySqlCommand command = new MySqlCommand($"DELETE FROM {ITEM_DATA_TABLE} WHERE {ITEM_ID}={itemId}", conn);
                var table = command.ExecuteReader();
                conn.Close();
                if (table.RecordsAffected != 1)
                    return false;
                return true;
            }
        }
        public List<ItemInfo> GetItemInfos()
        {
            using (MySqlConnection conn = new MySqlConnection(_dbAddress))
            {
                conn.Open();
                MySqlCommand command = new MySqlCommand($"SELECT * FROM {ITEM_DATA_TABLE}", conn);
                var table = command.ExecuteReader();
                List<ItemInfo> infos = new List<ItemInfo>();
                while (table.Read())
                {
                    int id = table.GetInt32(table.GetOrdinal(ITEM_ID));
                    string name = table.GetString(table.GetOrdinal(ITEM_NAME));
                    ItemType type = Enum.Parse<ItemType>(table.GetString(table.GetOrdinal(ITEM_TYPE)));
                    int maxStack = table.GetInt32(table.GetOrdinal(ITEM_MAX_STACK));
                    Console.WriteLine(name);
                    infos.Add(new ItemInfo(id, name, type, maxStack));
                }
                conn.Close();
                return infos;
            }
        }
        #endregion

        #region PlayerItemControl
        public bool AddItemToPlayer(string playerId, int itemId, int amount)
        {
            using var conn = new MySqlConnection(_dbAddress);
            try
            {
                conn.Open();
                using var transaction = conn.BeginTransaction();
                try
                {
                    if (TryUpdateExistingItem(conn, transaction, playerId, itemId, amount) ||
                        TryAddNewItem(conn, transaction, playerId, itemId, amount))
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
            string playerId, int itemId, int amount)
        {
            using var cmd = new MySqlCommand(Queries.SelectItemQuantity, conn, transaction);
            cmd.Parameters.AddWithValue("@playerId", playerId);
            cmd.Parameters.AddWithValue("@itemId", itemId);

            using var reader = cmd.ExecuteReader();
            if (!reader.Read()) return false;

            int currentQuantity = reader.GetInt32(0);
            reader.Close();

            int newQuantity = currentQuantity + amount;
            if (newQuantity < 0) return false;

            return newQuantity == 0
                ? DeleteItem(conn, transaction, playerId, itemId)
                : UpdateItemQuantity(conn, transaction, playerId, itemId, newQuantity);
        }

        private bool TryAddNewItem(MySqlConnection conn, MySqlTransaction transaction,
            string playerId, int itemId, int amount)
        {
            if (amount <= 0) return false;

            using var cmd = new MySqlCommand(Queries.InsertItem, conn, transaction);
            cmd.Parameters.AddWithValue("@playerId", playerId);
            cmd.Parameters.AddWithValue("@itemId", itemId);
            cmd.Parameters.AddWithValue("@quantity", amount);

            return cmd.ExecuteNonQuery() > 0;
        }

        private bool DeleteItem(MySqlConnection conn, MySqlTransaction transaction,
            string playerId, int itemId)
        {
            using var cmd = new MySqlCommand(Queries.DeleteItem, conn, transaction);
            cmd.Parameters.AddWithValue("@playerId", playerId);
            cmd.Parameters.AddWithValue("@itemId", itemId);

            return cmd.ExecuteNonQuery() > 0;
        }

        private bool UpdateItemQuantity(MySqlConnection conn, MySqlTransaction transaction,
            string playerId, int itemId, int quantity)
        {
            using var cmd = new MySqlCommand(Queries.UpdateItemQuantity, conn, transaction);
            cmd.Parameters.AddWithValue("@playerId", playerId);
            cmd.Parameters.AddWithValue("@itemId", itemId);
            cmd.Parameters.AddWithValue("@quantity", quantity);

            return cmd.ExecuteNonQuery() > 0;
        }

        #endregion

        #region AuctionControl
        public bool AddItemToAuction(string playerId, int itemId, int pricePerUnit, int quantity)
        {
            using (MySqlConnection conn = new MySqlConnection(_dbAddress))
            {
                conn.Open();
                using var transaction = conn.BeginTransaction();
                try
                {
                    //플레이어가 충분한 양을 갖고있는지 판별 & 빼주기

                    MySqlDataReader table = CheckAlreadyExistInAuction(conn, transaction, playerId, itemId);
                    if (!TryUpdateExistingItem(conn, transaction, playerId, itemId, quantity))
                        return false;
                    //옥션에 올리기
                    bool success = false;
                    if (table.Read())
                        success = AddAuctionItemQuantity(conn, transaction, playerId, itemId, quantity);
                    else
                        success = AddNewItemToAuction(conn, transaction, playerId, itemId, pricePerUnit, quantity);
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


        private bool AddNewItemToAuction(MySqlConnection conn, MySqlTransaction transaction, string playerId, int itemId, int pricePerUnit, int quantity)
        {
            MySqlCommand addNewItem = new MySqlCommand(Queries.AddNewItemToAuction, conn, transaction);
            addNewItem.Parameters.AddWithValue("@playerId", playerId);
            addNewItem.Parameters.AddWithValue("@itemId", itemId);
            addNewItem.Parameters.AddWithValue("@pricePerUnit", pricePerUnit);
            addNewItem.Parameters.AddWithValue("@quantity", quantity);
            return addNewItem.ExecuteNonQuery() > 0;
        }

        private bool AddAuctionItemQuantity(MySqlConnection conn, MySqlTransaction transaction, string playerId, int itemId, int quantity)
        {
            MySqlCommand addQuantity = new MySqlCommand(Queries.AddAuctionItemQuantity, conn, transaction);
            addQuantity.Parameters.AddWithValue("@quantity", quantity);
            addQuantity.Parameters.AddWithValue("@playerId", playerId);
            addQuantity.Parameters.AddWithValue("@itemId", itemId);
            return addQuantity.ExecuteNonQuery() > 0;

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
        private static class Queries
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
