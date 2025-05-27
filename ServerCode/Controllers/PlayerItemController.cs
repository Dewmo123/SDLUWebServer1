using BusinessLayer.Services;
using Microsoft.AspNetCore.Mvc;
using Repositories;
using ServerCode.DAO;
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
            string? playerId = HttpContext.Session.GetString("User");
            if (string.IsNullOrEmpty(playerId))
                return false;
            _fileLogger.LogInfo($"{playerId} Try Update Items");
            return await _playerItemService.UpdatePlayerItemsAsync(request, playerId) == false;
        }
        [HttpPost("update-item")]
        public async Task<bool> UpdateItem([FromBody] PlayerItemDTO inPlayerItemInfo)
        {
            string? playerId = HttpContext.Session.GetString("User");
            if (string.IsNullOrEmpty(playerId))
                return false;
            return await _playerItemService.UpdatePlayerItemAsync(inPlayerItemInfo, playerId);
        }
        [HttpPatch("add-items")]
        public async Task<bool> AddItems([FromBody] List<PlayerItemDTO> itemDeltas)
        {
            string? playerId = HttpContext.Session.GetString("User");
            if (string.IsNullOrEmpty(playerId))
                return false;
            return await _playerItemService.ChangePlayerItemsQuantityAsync(itemDeltas, playerId);
        }
        [HttpPatch("add-item")]
        public async Task<bool> AddItem([FromBody] PlayerItemDTO itemDelta)
        {
            string? playerId = HttpContext.Session.GetString("User");
            if (string.IsNullOrEmpty(playerId))
                return false;
            _fileLogger.LogInfo($"{playerId} Try Update Items");
            if (await _playerItemService.ChangePlayerItemQuantityAsync(itemDelta, playerId) == false)
                return false;

            return true;
        }

        [HttpGet("get-my-items")]
        public async Task<ActionResult<List<PlayerItemDTO>?>> GetItemsByPlayerId()
        {
            string? playerId = HttpContext.Session.GetString("User");
            Console.WriteLine($"Get MyItems Request");
            if (string.IsNullOrEmpty(playerId))
                return NotFound();
            Console.WriteLine($"Get Player Items: {playerId}");
            return await _playerItemService.GetItemsByPlayerId(playerId);
        }
    }
}
