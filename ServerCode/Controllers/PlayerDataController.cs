using Microsoft.AspNetCore.Mvc;
using ServerCode.Models;
using Repositories;
using BusinessLayer.Services;

namespace ServerCode.Controllers
{
    [ApiController] //localhost:3303/api/userinfo?a=123&b=123123
    [Route("/api/player")]
    public class PlayerDataController : Controller
    {
        private readonly PlayerLogInDataService playerDataService;
        public PlayerDataController(ServiceManager manager)
        {
            playerDataService = manager.playerLogInDataService;
        }
        [HttpPost("sign-up")]
        public async Task<bool> SignUp([FromBody] PlayerInfo info)
        {
            Console.WriteLine(info.id);
            if (info.id.Length > 8 && info.password.Length > 20)
                return false;
            return await playerDataService.SignUp(info);
        }
        [HttpPost("log-in")]
        public async Task<bool> Login([FromBody] PlayerInfo info)
        {
            Console.WriteLine($"LogIn: {info.id}");
            if (await playerDataService.LogIn(info))
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
    }
}
