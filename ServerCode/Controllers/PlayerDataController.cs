using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using ServerCode.Core;
using ServerCode.Models;

namespace ServerCode.Controllers
{
    [ApiController]
    [Route("/api")]
    public class PlayerDataController : Controller
    {
        [HttpPost("sign-up")]
        public bool SignUp([FromBody]PlayerInfo info)
        {
            if (info.id.Length > 8 && info.password.Length > 20)
                return false;
            return DBManager.Instance.SignUp(info.id, info.password);
        }
        [HttpPost("log-in")]
        public string Login([FromBody]PlayerInfo info)
        {
            string? msg;
            if (DBManager.Instance.LogIn(info.id, info.password))
            {
                msg = $"Hello {info.id}";
                HttpContext.Session.SetString("User", info.id);
            }
            else
            {
                msg = $"LogIn Failed";
            }
            return msg;
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
        public IActionResult UpdateItems([FromBody] ItemUpdateRequest request)
        {
            string? playerId = HttpContext.Session.GetString("User");
            if (playerId == null)
                return Unauthorized(new { message = "로그인이 필요합니다." });

            if (playerId != request.PlayerId)
                return StatusCode(403, new { message = "다른 플레이어의 아이템을 수정할 수 없습니다." });

            try
            {
                foreach (var itemUpdate in request.ItemUpdates)
                {
                    if (!DBManager.Instance.AddItemToPlayer(playerId, itemUpdate.Key, itemUpdate.Value))
                    {
                        return BadRequest(new { message = $"아이템 {itemUpdate.Key} 업데이트 실패" });
                    }
                }

                return Ok(new { message = "아이템이 성공적으로 업데이트되었습니다." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "서버 오류가 발생했습니다.", error = ex.Message });
            }
        }
    }

    public class ItemUpdateRequest
    {
        public string PlayerId { get; set; } = null!;
        public Dictionary<int, int> ItemUpdates { get; set; } = new();
    }
}
