using Microsoft.AspNetCore.Mvc;
using Repositories;
using ServerCode.Models;

namespace ServerCode.Controllers
{
    [ApiController]
    [Route("api/auction")]
    public class AuctionDataController : Controller
    {
        private DBManager _dbManager;
        public AuctionDataController(DBManager dbManager)
        {
            _dbManager = dbManager;
        }
        [HttpPost("post")]
        public async Task<bool> PostItemToAuction(AuctionItemInfo auctionItemInfo)
        {
            string? user = HttpContext.Session.GetString("User");
            if (auctionItemInfo.playerId != user)
                return false;
            return await _dbManager.AddItemToAuction(auctionItemInfo);
        }
        [HttpPost("purchase")]
        public async Task<bool> PurchaseItem(BuyerInfo buyerInfo)
        {
            string? user = HttpContext.Session.GetString("User");
            if (buyerInfo.buyerId != user && user == buyerInfo.itemInfo.playerId)
            {
                Console.WriteLine("not same");
                return false;
            }
            return await _dbManager.PurchaseItemInAuction(buyerInfo);
        }
        [HttpDelete("cancel")]
        public async Task<bool> CancelItem(string playerId, int itemId, int pricePerUnit)
        {
            string? userId = HttpContext.Session.GetString("User");
            if (userId != playerId)
                return false;
            return await _dbManager.CancelAuctionItem(
                new AuctionItemInfo() { playerId = playerId, itemId = itemId, pricePerUnit = pricePerUnit });
        }
        [HttpGet("get-items")]
        public async Task<ActionResult<List<AuctionItemInfo>>?> GetItemsByItemName(string itemName)
        {
            string? playerId = HttpContext.Session.GetString("User");
            Console.WriteLine($"playerId Access: {itemName}");
            if (playerId == null)
                return NotFound();
            return await _dbManager.GetAuctionItemByItemName(itemName);
        }
        [HttpGet("get-my-items")]
        public async Task<ActionResult<List<AuctionItemInfo>>?> GetItemsByPlayerName(string playerId)
        {
            string? userId = HttpContext.Session.GetString("User");
            if (playerId == null || playerId != userId)
                return NotFound();
            return await _dbManager.GetAuctionnItemByPlayerId(playerId);
        }
    }
}
