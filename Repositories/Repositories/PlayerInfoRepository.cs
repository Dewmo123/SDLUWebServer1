using MySqlConnector;
using Repositories;
using ServerCode.DAO;
using static Repositories.DBConfig;

namespace DataAccessLayer.Repositories
{
    public interface IPlayerInfoRepository : IRepository<PlayerVO>
    {

    }
    public class PlayerInfoRepository : IPlayerInfoRepository
    {

        public async Task<bool> AddAsync(PlayerVO playerInfo, MySqlConnection connection, MySqlTransaction transaction)
        {
            MySqlCommand command = new MySqlCommand(
                $"INSERT INTO {PLAYER_LOGIN_DATA_TABLE} ({PLAYER_ID},{PASSWORD})" +
                $" VALUES (@playerId, SHA2(@password,256))", connection, transaction);
            command.Parameters.AddWithValue("@playerId", playerInfo.id);
            command.Parameters.AddWithValue("@password", playerInfo.password);
            var table = await command.ExecuteNonQueryAsync();
            return table == 1;
        }
        public Task<bool> DeleteWithPrimaryKeysAsync(PlayerVO playerInfo, MySqlConnection connection, MySqlTransaction transaction)
        {
            throw new NotImplementedException();
        }

        public Task<List<PlayerVO>> GetAllItemsAsync(MySqlConnection connection)
        {
            throw new NotImplementedException();
        }
        
        public async Task<PlayerVO> GetItemByPrimaryKeysAsync(PlayerVO playerInfo, MySqlConnection connection)//정보만 가지고 오고 로그인은 DBManager에서 구현
        {
            MySqlCommand command = new MySqlCommand(
                $"SELECT * FROM {PLAYER_LOGIN_DATA_TABLE}" +
                $" WHERE {PLAYER_ID} = @playerId AND `{PASSWORD}` = SHA2(@password, 256)", connection);
            command.Parameters.AddWithValue("@playerId", playerInfo.id);
            command.Parameters.AddWithValue("@password", playerInfo.password);
            var table = await command.ExecuteReaderAsync();
            PlayerVO newInfo = new PlayerVO();
            while (await table.ReadAsync())
            {
                newInfo.id = table.GetString(table.GetOrdinal(PLAYER_ID));
                newInfo.password = table.GetString(table.GetOrdinal(PASSWORD));
            }
            await table.CloseAsync();
            return newInfo;
        }

        public Task<bool> UpdateAsync(PlayerVO entity, MySqlConnection connection, MySqlTransaction transaction)
        {
            throw new NotImplementedException();
        }
    }

}
