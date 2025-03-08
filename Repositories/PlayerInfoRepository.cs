using MySqlConnector;
using ServerCode.Models;

namespace Repositories
{
    public class PlayerInfoRepository : IRepository<PlayerInfo>
    {

        public async Task<bool> AddAsync(PlayerInfo playerInfo, MySqlConnection connection, MySqlTransaction transaction)
        {
            MySqlCommand command = new MySqlCommand($"INSERT INTO {DBManager.PLAYER_DATA_TABLE} ({DBManager.PLAYER_ID},{DBManager.PASSWORD}) VALUES (@playerId,@password)", connection, transaction);
            command.Parameters.AddWithValue("@playerId", playerInfo.id);
            command.Parameters.AddWithValue("@password", playerInfo.password);
            var table = await command.ExecuteNonQueryAsync();
            return table == 1;
        }
        public Task<bool> DeleteAsync(PlayerInfo playerInfo, MySqlConnection connection, MySqlTransaction transaction)
        {
            throw new NotImplementedException();
        }

        public Task<List<PlayerInfo>> GetAllItemsAsync(MySqlConnection connection, MySqlTransaction transaction)
        {
            throw new NotImplementedException();
        }

        public async Task<PlayerInfo> GetByIdAsync(string id, MySqlConnection connection, MySqlTransaction transaction)//정보만 가지고 오고 로그인은 DBManager에서 구현
        {
            MySqlCommand command = new MySqlCommand($"SELECT * FROM {DBManager.PLAYER_DATA_TABLE} WHERE {DBManager.PLAYER_ID} = @playerId", connection, transaction);
            command.Parameters.AddWithValue("@playerId", id);
            var table = await command.ExecuteReaderAsync();
            PlayerInfo info = new PlayerInfo();
            while (await table.ReadAsync())
            {
                string playerId = table.GetString(table.GetOrdinal(DBManager.PLAYER_ID));
                string password = table.GetString(table.GetOrdinal(DBManager.PASSWORD));
                info.id = playerId;
                info.password = password;
            }
            await table.CloseAsync();
            return info;
        }

        public  Task<bool> UpdateAsync(PlayerInfo entity, MySqlConnection connection, MySqlTransaction transaction)
        {
            throw new NotImplementedException();
        }
    }

}
