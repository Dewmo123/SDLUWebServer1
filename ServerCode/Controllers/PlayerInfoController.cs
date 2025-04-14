using BusinessLayer.Services;
using Microsoft.AspNetCore.Mvc;
using Repositories;
using ServerCode.DTO;

namespace ServerCode.Controllers
{
    [Route("api/player-info")]
    public class PlayerInfoController : Controller
    {
        private readonly PlayerInfoService playerInfoService;
        private FileLogger _fileLogger;
        public PlayerInfoController(ServiceManager manager, FileLogger logger)
        {
            _fileLogger = logger;
            playerInfoService = manager.playerInfoService;
        }
        [HttpPost("sign-up")]
        public async Task<bool> SignUp([FromBody] PlayerDTO info)
        {
            _fileLogger.LogInfo($"SignUp: {info.id}");
            if (info.id.Length > 8 && info.password.Length > 20)
                return false;
            return await playerInfoService.SignUp(info);
        }
        [HttpPost("log-in")]
        public async Task<bool> Login([FromBody] PlayerDTO info)
        {
            _fileLogger.LogInfo($"LogIn: {info.id}");
            if (await playerInfoService.LogIn(info))
            {
                HttpContext.Session.SetString("User", info.id);
                _fileLogger.LogInfo($"LogIn {info.id} : Success");
                return true;
            }
            _fileLogger.LogWarning($"LogIn {info.id} : Failed");
            return false;
        }
    }
}
