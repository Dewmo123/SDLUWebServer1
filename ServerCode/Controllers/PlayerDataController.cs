using Microsoft.AspNetCore.Mvc;
using ServerCode.Models;
using Repositories;

namespace ServerCode.Controllers
{
    [ApiController] //localhost:3303/api/userinfo?a=123&b=123123
    [Route("/api/player")]
    public class PlayerDataController : Controller
    {
        private readonly DBManager _dbManager;
        public PlayerDataController(DBManager manager)
        {
            _dbManager = manager;
        }
        [HttpPost("sign-up")]
        public async Task<bool> SignUp([FromBody] PlayerInfo info)
        {
            Console.WriteLine(info.id);
            if (info.id.Length > 8 && info.password.Length > 20)
                return false;
            return await _dbManager.SignUp(info);
        }
        [HttpPost("log-in")]
        public async Task<bool> Login([FromBody] PlayerInfo info)
        {
            Console.WriteLine($"LogIn: {info.id}");
            if (await _dbManager.LogIn(info))
            {
                HttpContext.Session.SetString("User", info.id);
                Console.WriteLine($"LogIn {info.id} : Success");
                return true;
            }
            Console.WriteLine($"LogIn {info.id} : Failed");
            return false;
        }
        [HttpGet("userinfo")]
        public string GetUserInfo()
        {
            string? name = HttpContext.Session.GetString("User");
            if (name == null)
                return "Please Login";
            return name;
        }

        [HttpPost("update-items")]
        public async Task<bool> UpdateItems([FromBody] List<PlayerItemInfo> request)
        {
            string? playerId = HttpContext.Session.GetString("User");
            if (playerId == null)
                return false;

            foreach (var itemUpdate in request)
                if (await _dbManager.ChangePlayerItemQuantityAsync(itemUpdate) == false)
                    return false;

            return true;
        }
        [HttpPost("update-item")]
        public async Task<bool> UpdateItem([FromBody] PlayerItemInfo inPlayerItemInfo)
        {
            string? playerId = HttpContext.Session.GetString("User");
            if (playerId != inPlayerItemInfo.playerId)
                return false;
            return await _dbManager.ChangePlayerItemQuantityAsync(inPlayerItemInfo);
        }
        [HttpGet("get-my-items")]
        public async Task<ActionResult<List<PlayerItemInfo>?>> GetItemsByPlayerId()
        {
            string? userId = HttpContext.Session.GetString("User");
            if (userId == null)
                return NotFound();
            return await _dbManager.GetItemsByPlayerId(userId);
        }
    }
}
