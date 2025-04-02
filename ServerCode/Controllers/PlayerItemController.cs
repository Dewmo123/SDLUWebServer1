using BusinessLayer.Services;
using Microsoft.AspNetCore.Mvc;
using Repositories;
using ServerCode.Models;

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
        public async Task<bool> UpdateItems([FromBody] List<PlayerItemInfo> request)
        {
            string? userId = HttpContext.Session.GetString("User");
            if (userId == null)
                return false;
            _fileLogger.LogInfo($"{userId} Try Update Items");
            foreach (var itemUpdate in request)
            {
                if (itemUpdate.playerId != userId)
                    return false;
                if (await _playerItemService.UpdatePlayerItemAsync(itemUpdate) == false)
                    return false;
            }

            return true;
        }
        [HttpPatch("add-item")]
        public async Task<bool> AddItem([FromBody] PlayerItemInfo itemDelta)
        {
            string? userId = HttpContext.Session.GetString("User");
            if (userId == null)
                return false;
            _fileLogger.LogInfo($"{userId} Try Update Items");
            if (await _playerItemService.ChangePlayerItemQuantityAsync(itemDelta) == false)
                return false;

            return true;
        }
        [HttpPost("update-item")]
        public async Task<bool> UpdateItem([FromBody] PlayerItemInfo inPlayerItemInfo)
        {
            string? playerId = HttpContext.Session.GetString("User");
            if (playerId != inPlayerItemInfo.playerId)
                return false;
            return await _playerItemService.UpdatePlayerItemAsync(inPlayerItemInfo);
        }
        [HttpGet("get-my-items")]
        public async Task<ActionResult<List<PlayerItemInfo>?>> GetItemsByPlayerId()
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
