using MySqlConnector;
using Repositories;
using ServerCode.DTO;
using ServerCode.DAO;

namespace BusinessLayer.Services
{
    public class PlayerItemService : Service
    {
        public PlayerItemService(RepositoryManager repo, AutoMapper.IMapper mapper, string dbAddress) : base(repo, mapper, dbAddress)
        {
        }

        public async Task<bool> ChangePlayerItemQuantityAsync(PlayerItemDTO itemInfoDTO,string userId)
        {
            await using var conn = new MySqlConnection(_dbAddress);
            await conn.OpenAsync();
            var itemInfo = _mapper.Map<PlayerItemDTO,PlayerItemDAO>(itemInfoDTO);
            itemInfo.playerId = userId;
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
        public async Task<bool> UpdatePlayerItemAsync(PlayerItemDTO itemInfoDTO,string userId)
        {
            await using var conn = new MySqlConnection(_dbAddress);
            await conn.OpenAsync();

            PlayerItemDAO itemInfo = _mapper.Map<PlayerItemDTO, PlayerItemDAO>(itemInfoDTO);
            itemInfo.playerId = userId;

            var remainItem = await _repositoryManager.PlayerItems.GetItemByPrimaryKeysAsync(itemInfo, conn);
            await using var transaction = await conn.BeginTransactionAsync();
            bool success = true;
            if (remainItem == null)
                success &= await _repositoryManager.PlayerItems.AddAsync(itemInfo, conn, transaction);
            else
                success &= await _repositoryManager.PlayerItems.UpdateAsync(itemInfo, conn, transaction);
            if (success)
                await transaction.CommitAsync();
            else
                await transaction.RollbackAsync();
            return success;
        }
        public async Task<List<PlayerItemDTO>> GetItemsByPlayerId(string playerId)
        {
            await using var conn = new MySqlConnection(_dbAddress);
            await conn.OpenAsync();
            var playerItemInfos = await _repositoryManager.PlayerItems.GetItemsByPlayerId(playerId, conn);
            List<PlayerItemDTO> items = new List<PlayerItemDTO>();
            playerItemInfos.ForEach(item => items.Add(_mapper.Map<PlayerItemDAO,PlayerItemDTO>(item)));
            return items;
        }

        private bool DeleteItem(MySqlConnection conn, MySqlTransaction transaction,
            PlayerItemDAO itemInfo)
        {
            return true;
        }
    }
}
