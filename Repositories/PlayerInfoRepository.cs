using MySqlConnector;
using ServerCode.Models;

namespace Repositories
{
    public class PlayerInfoRepository : IRepository<PlayerInfo>
    {
        private readonly string _dbAddress;
        public PlayerInfoRepository(string address)
        {
            _dbAddress = address;
        }
        public async Task<bool> AddAsync(PlayerInfo playerInfo)
        {
            if (CheckIDDuplication(playerInfo.id))
                return false;
            using (MySqlConnection conn = new MySqlConnection(_dbAddress))
            {
                conn.Open();
                MySqlCommand command = new MySqlCommand($"INSERT INTO {DBManager.PLAYER_DATA_TABLE} ({DBManager.PLAYER_ID},{DBManager.PASSWORD}) VALUES (@playerId,@password)", conn);
                command.Parameters.AddWithValue("@playerId", playerInfo.id);
                command.Parameters.AddWithValue("@password", playerInfo.password);
                Console.WriteLine("Sign up new Player");
                var table = await command.ExecuteNonQueryAsync();
                conn.Close();
                if (table != 1)
                    return false;
                return true;
            }
        }
        public bool CheckIDDuplication(string playerId)
        {
            using (MySqlConnection conn = new MySqlConnection(_dbAddress))
            {
                conn.Open();
                MySqlCommand command = new MySqlCommand($"SELECT {DBManager.PLAYER_ID} FROM {DBManager.PLAYER_DATA_TABLE} WHERE {DBManager.PLAYER_ID} = @playerId", conn);
                command.Parameters.AddWithValue("@playerId", playerId);
                var table = command.ExecuteReader();
                bool successRead = table.Read();
                conn.Close();
                return successRead;
            }
        }
        public Task<bool> DeleteAsync(PlayerInfo playerInfo)
        {
            throw new NotImplementedException();
        }

        public Task<List<PlayerInfo>> GetAllItemsAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<PlayerInfo> GetByIdAsync(string id)//정보만 가지고 오고 로그인은 DBManager에서 구현
        {
            using (MySqlConnection conn = new MySqlConnection(_dbAddress))
            {
                conn.Open();
                MySqlCommand command = new MySqlCommand($"SELECT {DBManager.PASSWORD} FROM {DBManager.PLAYER_DATA_TABLE} WHERE {DBManager.PLAYER_ID} = @playerId", conn);
                command.Parameters.AddWithValue("@playerId", id);
                Console.WriteLine("Check password");
                var table = await command.ExecuteReaderAsync();
                PlayerInfo info = new PlayerInfo();
                while (table.Read())
                {
                    string playerId = table.GetString(table.GetOrdinal(DBManager.PLAYER_ID));
                    string password = table.GetString(table.GetOrdinal(DBManager.PASSWORD));
                    info.id = playerId;
                    info.password = password;
                }
                conn.Close();
                return info;
            }
        }

        public Task<bool> UpdateAsync(PlayerInfo entity)
        {
            throw new NotImplementedException();
        }
    }
}
