using Microsoft.AspNetCore.Mvc;
using Repositories;
using BusinessLayer.Services;
using Newtonsoft.Json;
using ServerCode.DTO;

namespace ServerCode.Controllers
{
    [ApiController] //localhost:3303/api/userinfo?a=123&b=123123
    [Route("/api/player")]
    public class PlayerDataController : Controller
    {
        private readonly PlayerDataService playerDataService;
        private FileLogger _fileLogger;
        public PlayerDataController(ServiceManager manager, FileLogger logger)
        {
            _fileLogger = logger;
            playerDataService = manager.playerLogInDataService;
        }
        [HttpPost("sign-up")]
        public async Task<bool> SignUp([FromBody] PlayerInfoDTO info)
        {
            _fileLogger.LogInfo($"SignUp: {info.id}");
            if (info.id.Length > 8 && info.password.Length > 20)
                return false;
            return await playerDataService.SignUp(info);
        }
        [HttpPost("log-in")]
        public async Task<bool> Login([FromBody] PlayerInfoDTO info)
        {
            _fileLogger.LogInfo($"LogIn: {info.id}");
            if (await playerDataService.LogIn(info))
            {
                HttpContext.Session.SetString("User", info.id);
                _fileLogger.LogInfo($"LogIn {info.id} : Success");
                return true;
            }
            _fileLogger.LogWarning($"LogIn {info.id} : Failed");
            return false;
        }
        [HttpGet("get-my-data")]
        public async Task<ActionResult<PlayerDataInfoDTO>> GetMyData()
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
    }
}
