using Microsoft.AspNetCore.Mvc;
using Repositories;
using BusinessLayer.Services;
using Newtonsoft.Json;
using ServerCode.DTO;

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

            if (playerId == null)
                return NotFound();
            return await playerDataService.GetPlayerData(playerId);
        }
        [HttpPatch("upgrade-dictionary")]
        public async Task<bool> UpgradeDictionary(DictionaryUpgradeDTO dto)
        {
            string? playerId = HttpContext.Session.GetString("User");
            if (playerId == null)
                return false;
            Console.WriteLine($"{playerId} Request Upgrade Item: {dto.dictionaryKey}, {dto.level}");
            bool success = await playerDataService.UpgradeDictionary(playerId, dto);
            return success;
        }
        [HttpPatch("upgrade-equipment")]
        public async Task<bool> UpgradeWeapon([FromBody] EquipType equipType)
        {
            string? playerId = HttpContext.Session.GetString("User");
            Console.WriteLine(playerId);
            if (playerId == null)
                return false;
            Console.WriteLine($"{playerId} Request UpgradeWeapon");
            Console.WriteLine($"asd:{equipType}");
            bool success = await playerDataService.UpgradeEquip(playerId, equipType);
            return success;
        }
    }
}
