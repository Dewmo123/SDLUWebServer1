using Microsoft.AspNetCore.Mvc;
using ServerCode.Models;
using Repositories;

namespace ServerCode.Controllers
{
    [ApiController] //localhost:3303/api/userinfo?a=123&b=123123
    [Route("/api")]
    public class PlayerDataController : Controller
    {
        private readonly DBManager _dbManager;
        public PlayerDataController(DBManager manager)
        {
            _dbManager = manager;
        }
        [HttpPost("sign-up")]
        public async Task<bool> SignUp([FromBody] PlayerInfo info)
        {
            Console.WriteLine(info.id);
            if (info.id.Length > 8 && info.password.Length > 20)
                return false;
            return await _dbManager.SignUp(info);
        }
        [HttpPost("log-in")]
        public async Task<bool> Login([FromBody] PlayerInfo info)
        {
            Console.WriteLine(info.id);
            if (await _dbManager.LogIn(info))
            {
                HttpContext.Session.SetString("User", info.id);
                return true;
            }
            return false;
        }
        [HttpGet("userinfo")]
        public string GetUserInfo()
        {
            string? name = HttpContext.Session.GetString("User");
            if (name == null)
                return "Please Login";
            return name;
        }

        [HttpPost("update-items")]
        public async Task<IActionResult> UpdateItems([FromBody] ItemUpdateRequest request)
        {
            string? playerId = HttpContext.Session.GetString("User");
            if (playerId == null)
                return Unauthorized(new { message = "로그인이 필요합니다." });

            if (playerId != request.PlayerId)
                return StatusCode(403, new { message = "다른 플레이어의 아이템을 수정할 수 없습니다." });

            try
            {
                foreach (var itemUpdate in request.Updates)
                    if (await _dbManager.ChangePlayerItemQuantityAsync(itemUpdate) == false)
                        return BadRequest(new { message = $"아이템 {itemUpdate.itemId} 업데이트 실패" });

                return Ok(new { message = "아이템이 성공적으로 업데이트되었습니다." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "서버 오류가 발생했습니다.", error = ex.Message });
            }
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
        [HttpPost("update-item")]
        public async Task<bool> UpdateItem([FromBody]PlayerItemInfo inPlayerItemInfo)
        {
            string? playerId = HttpContext.Session.GetString("User");
            if (playerId == null || playerId != inPlayerItemInfo.playerId)
                return false;
            return await _dbManager.ChangePlayerItemQuantityAsync(inPlayerItemInfo);
        }
        [HttpDelete("cancel-auction-item")]
        public async Task<bool> CancelItem([FromQuery]AuctionItemInfo inAuctionItemInfo)
        {
            string? playerId = HttpContext.Session.GetString("User");
            if (playerId == null || playerId != inAuctionItemInfo.playerId)
                return false;
            return await _dbManager.CancelAuctionItem(inAuctionItemInfo);
        }
    }

    public class ItemUpdateRequest
    {
        public string PlayerId { get; set; } = null!;
        public List<PlayerItemInfo> Updates { get; set; } = null!;
    }
}
