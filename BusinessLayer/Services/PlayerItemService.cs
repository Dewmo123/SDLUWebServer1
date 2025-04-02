using MySqlConnector;
using Repositories;
using ServerCode.Models;

namespace BusinessLayer.Services
{
    public class PlayerItemService : Service
    {
        public PlayerItemService(RepositoryManager repo, string dbAddress) : base(repo, dbAddress)
        {
        }
        public async Task<bool> ChangePlayerItemQuantityAsync(PlayerItemInfo itemInfo)
        {
            await using var conn = new MySqlConnection(_dbAddress);
            await conn.OpenAsync();
            var info = await _repositoryManager.PlayerItems.GetItemByPrimaryKeysAsync(itemInfo, conn);
            await using var transaction = await conn.BeginTransactionAsync();
            try
            {
                return await _repositoryManager.PlayerItems.CheckConditionAndChangePlayerItem(itemInfo, info, conn, transaction);
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
            finally
            {
                if (transaction != null)
                    await transaction.CommitAsync();
            }
        }
        public async Task<bool> UpdatePlayerItemAsync(PlayerItemInfo itemInfo)
        {
            await using var conn = new MySqlConnection(_dbAddress);
            await conn.OpenAsync();
            var remainItem = await _repositoryManager.PlayerItems.GetItemByPrimaryKeysAsync(itemInfo, conn);
            await using var transaction = await conn.BeginTransactionAsync();
            if (remainItem == null)
                return await _repositoryManager.PlayerItems.AddAsync(itemInfo, conn, transaction);
            else
                return await _repositoryManager.PlayerItems.UpdateAsync(itemInfo, conn, transaction);
        }
        public async Task<List<PlayerItemInfo>> GetItemsByPlayerId(string playerId)
        {
            await using var conn = new MySqlConnection(_dbAddress);
            await conn.OpenAsync();
            return await _repositoryManager.PlayerItems.GetItemsByPlayerId(playerId, conn);
        }

        private bool DeleteItem(MySqlConnection conn, MySqlTransaction transaction,
            PlayerItemInfo itemInfo)
        {
            return true;
        }
    }
}
