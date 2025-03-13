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
        [HttpPost("post-to-auction")]
        public async Task<bool> PostItemToAuction(AuctionItemInfo auctionItemInfo)
        {
            string? user = HttpContext.Session.GetString("User");
            if (auctionItemInfo.playerId != user)
                return false;
            return await _dbManager.AddItemToAuction(auctionItemInfo);
        }
        [HttpPost("purchase-item")]
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
        [HttpDelete("cancel-auction-item")]
        public async Task<bool> CancelItem([FromQuery] AuctionItemInfo inAuctionItemInfo)
        {
            string? playerId = HttpContext.Session.GetString("User");
            if (playerId != inAuctionItemInfo.playerId)
                return false;
            return await _dbManager.CancelAuctionItem(inAuctionItemInfo);
        }
    }
}
