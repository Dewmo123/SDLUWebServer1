using BusinessLayer.Services;
using Microsoft.AspNetCore.Mvc;
using Repositories;
using ServerCode.DAO;
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
            string? playerId = HttpContext.Session.GetString("User");
            if (string.IsNullOrEmpty(playerId))
                return false;
            auctionItemInfo.playerId = playerId;
            _fileLogger.LogInfo($"{playerId} PostItemToAuction: {auctionItemInfo.itemName}");
            return await auctionService.AddItemToAuction(auctionItemInfo);
        }
        [HttpPatch("purchase")]
        public async Task<bool> PurchaseItem([FromBody]BuyerDTO buyerInfo)
        {
            string? playerId = HttpContext.Session.GetString("User");
            if (string.IsNullOrEmpty(playerId))
                return false;
            if (buyerInfo.buyerId != playerId || playerId == buyerInfo.itemInfo.playerId)
            {
                Console.WriteLine("not same");
                return false;
            }
            _fileLogger.LogInfo($"{playerId} Try to purchase {buyerInfo.itemInfo.itemName}");
            return await auctionService.PurchaseItemInAuction(buyerInfo);
        }
        [HttpDelete("cancel")]
        public async Task<bool> CancelItem(string itemName, int pricePerUnit)
        {
            string? playerId = HttpContext.Session.GetString("User");
            Console.WriteLine($"{playerId} Cancel Auction Item: {itemName}");
            if (string.IsNullOrEmpty(playerId))
                return false;
            return await auctionService.CancelAuctionItem(playerId,itemName,pricePerUnit);
        }
        [HttpGet("get-items")]
        public async Task<ActionResult<List<AuctionItemDTO>>?> GetItemsByItemName(string itemName)
        {
            string? playerId = HttpContext.Session.GetString("User");
            Console.WriteLine($"playerId Access: {itemName}");
            if (string.IsNullOrEmpty(playerId))
                return NotFound();
            return await auctionService.GetAuctionItemByItemName(itemName);
        }
        [HttpGet("get-my-items")]
        public async Task<ActionResult<List<AuctionItemDTO>>?> GetItemsByPlayerName()
        {
            string? playerId = HttpContext.Session.GetString("User");
            if (string.IsNullOrEmpty(playerId))
                return NotFound();
            return await auctionService.GetAuctionnItemByPlayerId(playerId);
        }
    }
}
