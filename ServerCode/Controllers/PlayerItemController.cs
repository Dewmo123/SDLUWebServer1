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
        public PlayerItemController(DBManager dbManager)
        {
            _playerItemService = dbManager.playerItemService;
        }
        [HttpPost("update-items")]
        public async Task<bool> UpdateItems([FromBody] List<PlayerItemInfo> request)
        {
            string? playerId = HttpContext.Session.GetString("User");
            if (playerId == null)
                return false;

            foreach (var itemUpdate in request)
                if (await _playerItemService.ChangePlayerItemQuantityAsync(itemUpdate) == false)
                    return false;

            return true;
        }
        [HttpPost("update-item")]
        public async Task<bool> UpdateItem([FromBody] PlayerItemInfo inPlayerItemInfo)
        {
            string? playerId = HttpContext.Session.GetString("User");
            if (playerId != inPlayerItemInfo.playerId)
                return false;
            return await _playerItemService.ChangePlayerItemQuantityAsync(inPlayerItemInfo);
        }
        [HttpGet("get-my-items")]
        public async Task<ActionResult<List<PlayerItemInfo>?>> GetItemsByPlayerId()
        {
            string? userId = HttpContext.Session.GetString("User");
            if (userId == null)
                return NotFound();
            return await _playerItemService.GetItemsByPlayerId(userId);
        }
    }
}
