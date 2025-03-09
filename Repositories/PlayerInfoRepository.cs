using MySqlConnector;
using ServerCode.Models;

namespace Repositories
{
    public class PlayerInfoRepository : IRepository<PlayerInfo>
    {

        public async Task<bool> AddAsync(PlayerInfo playerInfo, MySqlConnection connection, MySqlTransaction transaction)
        {
            MySqlCommand command = new MySqlCommand($"INSERT INTO {DBConfig.PLAYER_DATA_TABLE} ({DBConfig.PLAYER_ID},{DBConfig.PASSWORD}) VALUES (@playerId,@password)", connection, transaction);
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

        public async Task<PlayerInfo> GetByIdAsync(PlayerInfo playerInfo, MySqlConnection connection, MySqlTransaction transaction)//정보만 가지고 오고 로그인은 DBManager에서 구현
        {
            MySqlCommand command = new MySqlCommand($"SELECT * FROM {DBConfig.PLAYER_DATA_TABLE} WHERE {DBConfig.PLAYER_ID} = @playerId", connection, transaction);
            command.Parameters.AddWithValue("@playerId", playerInfo.id);
            var table = await command.ExecuteReaderAsync();
            PlayerInfo info = new PlayerInfo();
            while (await table.ReadAsync())
            {
                string playerId = table.GetString(table.GetOrdinal(DBConfig.PLAYER_ID));
                string password = table.GetString(table.GetOrdinal(DBConfig.PASSWORD));
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
