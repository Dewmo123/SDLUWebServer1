using Microsoft.AspNetCore.Mvc;
using ServerCode.Core;
using ServerCode.Models;

namespace ServerCode.Controllers
{
    [ApiController]
    [Route("/admin")]
    public class AdminController : Controller
    {
        [HttpGet]
        public string Authorize()
        {
            if (HttpContext.Session.GetString("User") == "admin")
            {
                HttpContext.Session.SetInt32("IsAdmin", 1);
                return "인증 완료";
            }
            return "인증 실패";
        }
        [HttpGet("add-item")]
        public string AddItem(string itemName,ItemType type,int maxStack)
        {
            if (HttpContext.Session.GetInt32("IsAdmin") != 1)
                return "You are not admin";

            if (DBManager.Instance.AddItemInfo(itemName, type, maxStack))
                return "Success AddItem";
            
            return "Add Failed";
        }
        [HttpGet("remove-item")]
        public string RemoveItem(int itemId)
        {
            if (HttpContext.Session.GetInt32("IsAdmin") != 1)
                return "You are not admin";
            if (DBManager.Instance.RemoveItemInfo(itemId))
                return "Success Remove Item";
            return "Remove Failed";
        }
        [HttpGet("item-info")]
        public List<ItemInfo>? GetItemInfo()
        {
            if (HttpContext.Session.GetInt32("IsAdmin") != 1)
                return null;

            return DBManager.Instance.GetItemInfos();
        }
    }
}
