using AutoMapper;
using MySqlConnector;
using Newtonsoft.Json;
using Repositories;
using ServerCode.DAO;
using ServerCode.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Services
{
    public class PlayerInfoService : Service
    {
        public PlayerInfoService(RepositoryManager repo, IMapper mapper, string dbAddress) : base(repo, mapper, dbAddress)
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
        public async Task<bool> SignUp(PlayerDTO playerInfoDTO)
        {
            await using MySqlConnection connection = new MySqlConnection(_dbAddress);
            await connection.OpenAsync();
            var playerInfo = _mapper.Map<PlayerDTO, PlayerDAO>(playerInfoDTO);
            var info = await _repositoryManager.PlayerInfos.GetItemByPrimaryKeysAsync(playerInfo, connection);
            await using MySqlTransaction transaction = await connection.BeginTransactionAsync();
            try
            {
                if (info.id == playerInfo.id)
                {
                    Console.WriteLine($"{info.id}:Duplicate");
                    return false;
                }
                bool success = await _repositoryManager.PlayerInfos.AddAsync(playerInfo, connection, transaction);
                string json = await SetUpDefaultDictionary();
                success &= await _repositoryManager.PlayerData.AddAsync(new PlayerDataDAO() { playerId = playerInfo.id, gold = 0, dictionary = json }, connection, transaction);
                await transaction.CommitAsync();
                Console.WriteLine(success);
                return success;
            }
            catch (MySqlException)
            {
                Console.WriteLine("Rollbakcc");
                await transaction.RollbackAsync();
                return false;
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine(ex);
                await transaction.RollbackAsync();
                return false;
            }
        }
        public async Task<bool> LogIn(PlayerDTO playerInfoDTO)
        {
            await using MySqlConnection connection = new MySqlConnection(_dbAddress);
            await connection.OpenAsync();
            var playerInfo = _mapper.Map<PlayerDTO, PlayerDAO>(playerInfoDTO);
            try
            {
                var info = await _repositoryManager.PlayerInfos.GetItemByPrimaryKeysAsync(playerInfo, connection);
                return !string.IsNullOrEmpty(info.password);
            }
            catch (MySqlException)
            {
                return false;
            }
        }

    }
}
