using BusinessLayer.Services;
using Microsoft.AspNetCore.Mvc;
using Repositories;
using ServerCode.DTO;

namespace ServerCode.Controllers
{
    [ApiController]
    [Route("api/auction")]
    public class AuctionDataController : Controller
    {
        private AuctionService auctionService;
        private FileLogger _fileLogger;
        public AuctionDataController(ServiceManager dbManager,FileLogger logger)
        {
            auctionService = dbManager.auctionService;
            _fileLogger = logger;
        }
        [HttpPost("post")]
        public async Task<bool> PostItemToAuction([FromBody]AuctionItemDTO auctionItemInfo)
        {
            string? user = HttpContext.Session.GetString("User");
            if (user == null)
                return false;
            auctionItemInfo.playerId = user;
            _fileLogger.LogInfo($"{user} PostItemToAuction: {auctionItemInfo.itemName}");
            return await auctionService.AddItemToAuction(auctionItemInfo);
        }
        [HttpPatch("purchase")]
        public async Task<bool> PurchaseItem([FromBody]BuyerDTO buyerInfo)
        {
            string? user = HttpContext.Session.GetString("User");
            if (buyerInfo.buyerId != user || user == buyerInfo.itemInfo.playerId)
            {
                Console.WriteLine("not same");
                return false;
            }
            _fileLogger.LogInfo($"{user} Try to purchase {buyerInfo.itemInfo.itemName}");
            return await auctionService.PurchaseItemInAuction(buyerInfo);
        }
        [HttpDelete("cancel")]
        public async Task<bool> CancelItem(string itemName, int pricePerUnit)
        {
            string? userId = HttpContext.Session.GetString("User");
            Console.WriteLine($"{userId} Cancel Auction Item: {itemName}");
            if (userId == null)
                return false;
            return await auctionService.CancelAuctionItem(userId,itemName,pricePerUnit);
        }
        [HttpGet("get-items")]
        public async Task<ActionResult<List<AuctionItemDTO>>?> GetItemsByItemName(string itemName)
        {
            string? playerId = HttpContext.Session.GetString("User");
            Console.WriteLine($"playerId Access: {itemName}");
            if (playerId == null)
                return NotFound();
            return await auctionService.GetAuctionItemByItemName(itemName);
        }
        [HttpGet("get-my-items")]
        public async Task<ActionResult<List<AuctionItemDTO>>?> GetItemsByPlayerName()
        {
            string? userId = HttpContext.Session.GetString("User");
            if (userId == null)
                return NotFound();
            return await auctionService.GetAuctionnItemByPlayerId(userId);
        }
    }
}
