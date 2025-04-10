using BusinessLayer.Services;
using Microsoft.AspNetCore.Mvc;
using Repositories;
using ServerCode.DTO;

namespace ServerCode.Controllers
{
    [Route("/api/player-item")]
    public class PlayerItemController : Controller
    {
        private PlayerItemService _playerItemService;
        private FileLogger _fileLogger;
        public PlayerItemController(ServiceManager dbManager, FileLogger logger)
        {
            _playerItemService = dbManager.playerItemService;
            _fileLogger = logger;
        }
        [HttpPatch("update-items")]
        public async Task<bool> UpdateItems([FromBody] List<PlayerItemDTO> request) //얘는 아예 덮어씌우는거라 나중에 던전에서 얻을걸 더해주는 걸로 해야할듯
        {
            string? userId = HttpContext.Session.GetString("User");
            if (userId == null)
                return false;
            _fileLogger.LogInfo($"{userId} Try Update Items");
            foreach (var itemUpdate in request)
            {
                if (await _playerItemService.UpdatePlayerItemAsync(itemUpdate, userId) == false)
                    return false;
            }

            return true;
        }
        [HttpPatch("add-item")]
        public async Task<bool> AddItem([FromBody] PlayerItemDTO itemDelta)
        {
            string? userId = HttpContext.Session.GetString("User");
            if (userId == null)
                return false;
            _fileLogger.LogInfo($"{userId} Try Update Items");
            if (await _playerItemService.ChangePlayerItemQuantityAsync(itemDelta, userId) == false)
                return false;

            return true;
        }
        [HttpPost("update-item")]
        public async Task<bool> UpdateItem([FromBody] PlayerItemDTO inPlayerItemInfo)
        {
            string? userId = HttpContext.Session.GetString("User");
            if (userId == null)
                return false;
            return await _playerItemService.UpdatePlayerItemAsync(inPlayerItemInfo, userId);
        }
        [HttpGet("get-my-items")]
        public async Task<ActionResult<List<PlayerItemDTO>?>> GetItemsByPlayerId()
        {
            string? userId = HttpContext.Session.GetString("User");
            Console.WriteLine($"Get MyItems Request");
            if (userId == null)
                return NotFound();
            Console.WriteLine($"Get Player Items: {userId}");
            return await _playerItemService.GetItemsByPlayerId(userId);
        }
    }
}
