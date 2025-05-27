using Microsoft.AspNetCore.Mvc;
using Repositories;
using BusinessLayer.Services;
using Newtonsoft.Json;
using ServerCode.DTO;
using ServerCode.DAO;

namespace ServerCode.Controllers
{
    [ApiController] //localhost:3303/api/userinfo?a=123&b=123123
    [Route("/api/player-data")]
    public class PlayerDataController : Controller
    {
        private readonly PlayerDataService playerDataService;
        private FileLogger _fileLogger;
        public PlayerDataController(ServiceManager manager, FileLogger logger)
        {
            _fileLogger = logger;
            playerDataService = manager.playerDataService;
        }
        [HttpGet("get-my-data")]
        public async Task<ActionResult<PlayerDataDTO>> GetMyData()
        {
            string? playerId = HttpContext.Session.GetString("User");

            if (string.IsNullOrEmpty(playerId))
                return NotFound();
            return await playerDataService.GetPlayerData(playerId);
        }
        [HttpPatch("stage-end")]
        public async Task<bool> StageEnd(StageEndDTO stageEndDTO)
        {
            string? playerId = HttpContext.Session.GetString("User");
            if (string.IsNullOrEmpty(playerId))
                return false;
            Console.WriteLine(stageEndDTO.stageCount);
            return await playerDataService.StageEnd(stageEndDTO.stageCount, playerId);
        }
        [HttpPatch("upgrade-dictionary")]
        public async Task<bool> UpgradeDictionary(DictionaryUpgradeDTO dto)
        {
            string? playerId = HttpContext.Session.GetString("User");
            if (string.IsNullOrEmpty(playerId))
                return false;
            Console.WriteLine($"{playerId} Request Upgrade Item: {dto.dictionaryKey}");
            bool success = await playerDataService.UpgradeDictionary(playerId, dto);
            return success;
        }
        [HttpPatch("upgrade-equipment")]
        public async Task<bool> UpgradeWeapon([FromBody] EquipType equipType)
        {
            string? playerId = HttpContext.Session.GetString("User");
            Console.WriteLine(playerId);
            if (string.IsNullOrEmpty(playerId))
                return false;
            Console.WriteLine($"{playerId} Request UpgradeWeapon");
            Console.WriteLine($"asd:{equipType}");
            bool success = await playerDataService.UpgradeEquip(playerId, equipType);
            return success;
        }
    }
}
