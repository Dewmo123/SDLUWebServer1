using MySqlConnector;
using Repositories;
using ServerCode.DAO;
using Newtonsoft.Json;
using System.Collections;
using System.Net.Http;
using ServerCode.DTO;
using AutoMapper;
namespace BusinessLayer.Services
{
    public class PlayerDataService : Service
    {
        public PlayerDataService(RepositoryManager repo, IMapper mapper, string dbAddress) : base(repo, mapper, dbAddress)
        {
        }

        public async Task<string> SetUpDefaultDictionary()
        {
            await using MySqlConnection connection = new MySqlConnection(_dbAddress);
            await connection.OpenAsync();
            var items = await _repositoryManager.ItemInfos.GetItemInfoWithType(ItemType.dictionary, connection);
            string json = JsonConvert.SerializeObject(items.ToDictionary(key => key.itemName, val => 0), Formatting.Indented);
            await connection.CloseAsync();
            return json;
        }
        public async Task<PlayerDataDTO> GetPlayerData(string playerId)
        {
            await using MySqlConnection connection = new MySqlConnection(_dbAddress);
            await connection.OpenAsync();
            PlayerDataVO myInfo = new PlayerDataVO() { playerId = playerId };
            myInfo = await _repositoryManager.PlayerData.GetItemByPrimaryKeysAsync(myInfo, connection);
            string defaultDictionaryJson = await SetUpDefaultDictionary();
            string playerDictionary = JsonConvert.SerializeObject(CompareDictionary(defaultDictionaryJson, myInfo.dictionary!));
            myInfo.dictionary = playerDictionary;
            return _mapper.Map<PlayerDataVO, PlayerDataDTO>(myInfo);
        }
        public async Task<bool> UpgradeDictionary(string playerId, DictionaryUpgradeDTO dto)
        {
            string defaultDictionaryJson = await SetUpDefaultDictionary();
            await using MySqlConnection connection = new MySqlConnection(_dbAddress);
            await connection.OpenAsync();
            string key = dto.dictionaryKey!;

            var playerData = await _repositoryManager.PlayerData.GetItemByPrimaryKeysAsync(
                new PlayerDataVO() { playerId = playerId }, connection);
            var playerItemData = await _repositoryManager.PlayerItems.GetItemByPrimaryKeysAsync(
                new PlayerItemVO() { itemName = key, playerId = playerId }, connection);

            if (playerData.dictionary == null)
                playerData.dictionary = defaultDictionaryJson;

            int stack = 0;
            Dictionary<string, int>? playerDictionary = CompareDictionary(defaultDictionaryJson, playerData.dictionary, key);
            if (playerDictionary == null)
                return false;
            stack = playerDictionary[key];

            if (stack != dto.level)
                return false;
            int afterLogic = DictionaryUpgradeLogic(stack, playerItemData.quantity);
            if (afterLogic < 0)
                return false;
            playerItemData.quantity = afterLogic;
            playerDictionary[key]++;

            playerData.dictionary = JsonConvert.SerializeObject(playerDictionary);
            await using MySqlTransaction transaction = await connection.BeginTransactionAsync();
            bool success = true;
            success &= await _repositoryManager.PlayerData.UpdateAsync(playerData, connection, transaction);
            success &= await _repositoryManager.PlayerItems.UpdateAsync(playerItemData, connection, transaction);

            if (success)
                await transaction.CommitAsync();
            else
                await transaction.RollbackAsync();
            return success;
        }

        private Dictionary<string, int>? CompareDictionary(string defaultDictionaryJson, string playerDictionaryJson, string? key = null)
        {
            Dictionary<string, int>? defaultDictionary = JsonConvert.DeserializeObject<Dictionary<string, int>>(defaultDictionaryJson)!;
            Dictionary<string, int>? playerDictionary = JsonConvert.DeserializeObject<Dictionary<string, int>>(playerDictionaryJson)!;

            foreach (var item in defaultDictionary.Keys)
                if (!playerDictionary.ContainsKey(item))
                    playerDictionary.Add(item, 0);
            if (key != null)
                if (playerDictionary.ContainsKey(key)) //if playerDictionary doesn't contain reqKey, compare defualtDic and add or fail
                    return playerDictionary;
                else
                {
                    if (defaultDictionary.ContainsKey(key))
                        playerDictionary.Add(key, 0);
                    else
                        return default;
                }
            return playerDictionary;
        }
        private int DictionaryUpgradeLogic(int stack, int quantity) => quantity -= stack * 3 + 5;
        public async Task<bool> UpgradeEquip(string playerId,EquipType equipType)
        {
            await using MySqlConnection connection = new MySqlConnection(_dbAddress);
            await connection.OpenAsync();
            PlayerDataVO playerDataDAO = new PlayerDataVO() { playerId = playerId };
            playerDataDAO = await _repositoryManager.PlayerData.GetItemByPrimaryKeysAsync(playerDataDAO, connection);

            int equipLevel = equipType == EquipType.Weapon ? playerDataDAO.weaponLevel : playerDataDAO.armorLevel;
            int remain = EquipUpgradeLogic( equipLevel, playerDataDAO.gold);
            if (remain < 0)
                return false;
            playerDataDAO.gold = remain;
            if (equipType == EquipType.Weapon)
                playerDataDAO.weaponLevel += 1;
            else
                playerDataDAO.armorLevel += 1;

            await using MySqlTransaction transaction = await connection.BeginTransactionAsync();
            bool success = true;
            success &= await _repositoryManager.PlayerData.UpdateAsync(playerDataDAO, connection, transaction);

            if (success)
                await transaction.CommitAsync();
            else
                await transaction.RollbackAsync();
            return success;
        }
        private int EquipUpgradeLogic(int stack, int gold) => gold -= stack * 2 + 3;
    }
}
