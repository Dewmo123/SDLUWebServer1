using System.Collections.Generic;

namespace Repositories
{
    public static class DBConfig
    {
        #region playerData

        /// <summary>
        /// 플레이어 로그인 데이터 테이블의 이름입니다. 플레이어 로그인 데이터 테이블은 playerid와 password를 속성으로 가집니다(varchar)
        /// </summary>
        public const string PLAYER_LOGIN_DATA_TABLE = "player_login_data";
        public const string PLAYER_ID = "player_id";
        public const string PASSWORD = "password";
        /// <summary>
        /// 플레이어 아이템 테이블의 이름입니다. 플레이어 아이템 테이블은 ItemId(int), quantity(int)를 속성으로 가집니다
        /// </summary>
        public const string PLAYER_ITEM_TABLE = "player_item_data";
        public const string QUANTITY = "quantity";
        /// <summary>
        /// 플레이어 데이터 테이블의 이름입니다. PlayerId, Gold를 속성으로 가집니다
        /// </summary>
        public const string PLAYER_DATA_TABLE = "player_data";
        public const string GOLD = "gold";
        public const string DICTIONARY = "dictionary";
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
        #region SQL Queries
        public static class Queries
        {
            public static string SelectItemQuantity =>
                $"SELECT {QUANTITY} FROM {PLAYER_ITEM_TABLE} " +
                $"WHERE {PLAYER_ID} = @playerId AND {ITEM_ID} = @itemId";

            public static string DeleteItem =>
                $"DELETE FROM {PLAYER_ITEM_TABLE} " +
                $"WHERE {PLAYER_ID} = @playerId AND {ITEM_ID} = @itemId";



            public static string AddNewItemToAuction =>
                $"INSERT INTO {AUCTION_DATA_TABLE} ({PLAYER_ID},{ITEM_ID},{PRICE_PER_UNIT},{QUANTITY},{ITEM_NAME})" +
                $" VALUES (@playerId,@itemId,@pricePerUnit,@quantity,@itemName)";
            public static string UpdateAuctionItemQuantity =>
                $"UPDATE {AUCTION_DATA_TABLE} SET {QUANTITY} = {QUANTITY} + @quantity " +
                $"WHERE {PLAYER_ID} = @playerId AND {ITEM_ID} = @itemId AND {PRICE_PER_UNIT} = @pricePerUnit";
            public static string GetAuctionItemById =>
                $"SELECT * FROM {AUCTION_DATA_TABLE}" +
                $" WHERE {PLAYER_ID} = @playerId AND {ITEM_ID} = @itemId AND {PRICE_PER_UNIT} = @pricePerUnit";
            public static string GetPlayerItemDataInfo =>
                $"SELECT * FROM {PLAYER_ITEM_TABLE}" +
                $" WHERE {PLAYER_ID} = @playerId AND {ITEM_ID} = @itemId";
        }
        #endregion
    }
}
