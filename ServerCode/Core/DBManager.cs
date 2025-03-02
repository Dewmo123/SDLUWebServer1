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
                MySqlCommand command = new MySqlCommand($"SELECT {PLAYER_ID} FROM {PLAYER_DATA_TABLE} WHERE {PLAYER_ID} = '{playerId}'", conn);
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
                MySqlCommand command = new MySqlCommand($"INSERT INTO {PLAYER_DATA_TABLE} ({PLAYER_ID},{PASSWORD}) VALUES ('{playerId}','{password}')", conn);
                Console.WriteLine("Sign up new Player");
                var table = command.ExecuteReader();
                conn.Close();
                if (table.RecordsAffected != 1)
                    return false;
                return true;
            }
        }
        public bool LogIn(string playerId, string password)
        {
            using (MySqlConnection conn = new MySqlConnection(_dbAddress))
            {
                conn.Open();
                MySqlCommand command = new MySqlCommand($"SELECT {PASSWORD} FROM {PLAYER_DATA_TABLE} WHERE {PLAYER_ID} = '{playerId}'", conn);
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
                MySqlCommand command = new MySqlCommand($"INSERT INTO {ITEM_DATA_TABLE} ({ITEM_NAME},{ITEM_TYPE},{ITEM_MAX_STACK}) VALUES ('{itemName}','{(int)type}','{maxStack}')", conn);
                var table = command.ExecuteReader();
                conn.Close();
                if (table.RecordsAffected != 1)
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
    }
}
