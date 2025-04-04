using Microsoft.AspNetCore.Mvc;
using ServerCode.Models;
using Repositories;
using BusinessLayer.Services;
using Newtonsoft.Json;

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
        public async Task<bool> SignUp([FromBody] PlayerInfo info)
        {
            _fileLogger.LogInfo($"SignUp: {info.id}");
            if (info.id.Length > 8 && info.password.Length > 20)
                return false;
            return await playerDataService.SignUp(info);
        }
        [HttpPost("log-in")]
        public async Task<bool> Login([FromBody] PlayerInfo info)
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
        public async Task<ActionResult<PlayerDataInfo>> GetMyData()
        {
            string? playerId = HttpContext.Session.GetString("User");

            if (playerId == null)
                return NotFound();
            return await playerDataService.GetPlayerData(playerId);
        }
        [HttpPatch("upgrade-dictionary")]
        public async Task<bool> UpgradeDictionary(string key)
        {
            string? playerId = HttpContext.Session.GetString("User");
            if (playerId == null)
                return false;
            var data = await playerDataService.GetPlayerData(playerId);
            string defaultDictionaryJson = await playerDataService.SetUpDefaultDictionary();

            if (data.dictionary == null)
                data.dictionary = defaultDictionaryJson;

            Dictionary<string, int>? defaultDictionary = JsonConvert.DeserializeObject<Dictionary<string, int>>(defaultDictionaryJson)!;
            Dictionary<string, int>? playerDictionary = JsonConvert.DeserializeObject<Dictionary<string, int>>(data.dictionary)!;
            int stack = 0;

            if (playerDictionary.ContainsKey(key))
                stack = playerDictionary[key];
            else
            {
                if (defaultDictionary.ContainsKey(key))
                    playerDictionary.Add(key, 0);
                else
                    return false;
            }
            int afterLogic = DictionaryUpgradeLogic(stack, data.gold);
            if (afterLogic < 0)
                return false;
            data.gold = afterLogic;
            return await playerDataService.UpdatePlayerData(data);
        }

        private int DictionaryUpgradeLogic(int stack, int gold) => gold -= stack * 3 + 5;
    }
}
