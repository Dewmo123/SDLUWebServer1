using MySqlConnector;
using Repositories;
using ServerCode.DTO;
using ServerCode.DAO;
using DataAccessLayer.Repositories;

namespace BusinessLayer.Services
{
    public class PlayerItemService : Service
    {
        public PlayerItemService(RepositoryManager repo, AutoMapper.IMapper mapper, string dbAddress) : base(repo, mapper, dbAddress)
        {
        }

        public async Task<bool> ChangePlayerItemQuantityAsync(PlayerItemDTO itemInfoDTO, string userId)
        {
            await using var connection = new MySqlConnection(_dbAddress);
            await connection.OpenAsync();
            var itemInfo = _mapper.Map<PlayerItemDTO, PlayerItemVO>(itemInfoDTO);
            itemInfo.playerId = userId;
            var info = await _repositoryManager.PlayerItems.GetItemByPrimaryKeysAsync(itemInfo, connection);
            await using var transaction = await connection.BeginTransactionAsync();
            bool success = await _repositoryManager.PlayerItems.CheckConditionAndChangePlayerItem(itemInfo, info, connection, transaction);

            if (success)
                await transaction.CommitAsync();
            else
                await transaction.RollbackAsync();
            return success;
        }
        public async Task<bool> ChangePlayerItemsQuantityAsync(List<PlayerItemDTO> itemInfoDTOs, string userId)
        {
            await using var connection = new MySqlConnection(_dbAddress);
            await connection.OpenAsync();
            bool success = true;
            await using var transaction = await connection.BeginTransactionAsync();
            foreach (var itemInfoDTO in itemInfoDTOs)
            {
                var itemInfo = _mapper.Map<PlayerItemDTO, PlayerItemVO>(itemInfoDTO);
                itemInfo.playerId = userId;
                var info = await _repositoryManager.PlayerItems.GetItemWithTransactionAsync(itemInfo, connection, transaction);
                success &= await _repositoryManager.PlayerItems.CheckConditionAndChangePlayerItem(itemInfo, info, connection, transaction);
            }

            if (success)
                await transaction.CommitAsync();
            else
                await transaction.RollbackAsync();
            return success;
        }
        public async Task<bool> UpdatePlayerItemAsync(PlayerItemDTO itemInfoDTO, string userId)
        {
            await using var connection = new MySqlConnection(_dbAddress);
            await connection.OpenAsync();

            PlayerItemVO itemInfo = _mapper.Map<PlayerItemDTO, PlayerItemVO>(itemInfoDTO);
            itemInfo.playerId = userId;

            var remainItem = await _repositoryManager.PlayerItems.GetItemByPrimaryKeysAsync(itemInfo, connection);
            await using var transaction = await connection.BeginTransactionAsync();
            bool success = true;
            if (remainItem == null)
                success &= await _repositoryManager.PlayerItems.AddAsync(itemInfo, connection, transaction);
            else
                success &= await _repositoryManager.PlayerItems.UpdateAsync(itemInfo, connection, transaction);
            if (success)
                await transaction.CommitAsync();
            else
                await transaction.RollbackAsync();
            return success;
        }
        public async Task<bool> UpdatePlayerItemsAsync(IEnumerable<PlayerItemDTO> items, string userId)
        {
            await using var connection = new MySqlConnection(_dbAddress);
            await connection.OpenAsync();
            await using var transaction = await connection.BeginTransactionAsync();

            try
            {
                foreach (var itemDTO in items)
                {
                    PlayerItemVO item = _mapper.Map<PlayerItemDTO, PlayerItemVO>(itemDTO);
                    item.playerId = userId;

                    var existingItem = await _repositoryManager.PlayerItems.GetItemWithTransactionAsync(item, connection, transaction);
                    bool success;

                    if (existingItem == null)
                        success = await _repositoryManager.PlayerItems.AddAsync(item, connection, transaction);
                    else
                        success = await _repositoryManager.PlayerItems.UpdateAsync(item, connection, transaction);

                    if (!success)
                        throw new Exception($"Failed to update or insert item: {item.itemName}");
                }

                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return false;
            }
        }

        public async Task<List<PlayerItemDTO>> GetItemsByPlayerId(string playerId)
        {
            await using var connection = new MySqlConnection(_dbAddress);
            await connection.OpenAsync();
            var playerItemInfos = await _repositoryManager.PlayerItems.GetItemsByPlayerId(playerId, connection);
            List<PlayerItemDTO> items = new List<PlayerItemDTO>();
            playerItemInfos.ForEach(item => items.Add(_mapper.Map<PlayerItemVO, PlayerItemDTO>(item)));
            Console.WriteLine(items.Count);
            return items;
        }

        private bool DeleteItem(MySqlConnection connection, MySqlTransaction transaction,
            PlayerItemVO itemInfo)
        {
            return true;
        }
    }
}
