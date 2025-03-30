using BusinessLayer.Services;
using Microsoft.AspNetCore.Mvc;
using Repositories;
using ServerCode.Models;

namespace ServerCode.Controllers
{
    [ApiController]
    [Route("api/auction")]
    public class AuctionDataController : Controller
    {
        private AuctionService auctionService;
        public AuctionDataController(ServiceManager dbManager)
        {
            auctionService = dbManager.auctionService;
        }
        [HttpPost("post")]
        public async Task<bool> PostItemToAuction(AuctionItemInfo auctionItemInfo)
        {
            string? user = HttpContext.Session.GetString("User");
            if (auctionItemInfo.playerId != user)
                return false;
            return await auctionService.AddItemToAuction(auctionItemInfo);
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
            return await auctionService.PurchaseItemInAuction(buyerInfo);
        }
        [HttpDelete("cancel")]
        public async Task<bool> CancelItem(string itemName, int pricePerUnit)
        {
            string? userId = HttpContext.Session.GetString("User");
            Console.WriteLine($"{userId} Cancel Auction Item: {itemName}");
            if (userId == null)
                return false;
            return await auctionService.CancelAuctionItem(
                new AuctionItemInfo() { playerId = userId, itemName = itemName, pricePerUnit = pricePerUnit });
        }
        [HttpGet("get-items")]
        public async Task<ActionResult<List<AuctionItemInfo>>?> GetItemsByItemName(string itemName)
        {
            string? playerId = HttpContext.Session.GetString("User");
            Console.WriteLine($"playerId Access: {itemName}");
            if (playerId == null)
                return NotFound();
            return await auctionService.GetAuctionItemByItemName(itemName);
        }
        [HttpGet("get-my-items")]
        public async Task<ActionResult<List<AuctionItemInfo>>?> GetItemsByPlayerName()
        {
            string? userId = HttpContext.Session.GetString("User");
            if (userId == null)
                return NotFound();
            return await auctionService.GetAuctionnItemByPlayerId(userId);
        }
    }
}
