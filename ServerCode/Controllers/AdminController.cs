using Microsoft.AspNetCore.Mvc;
using ServerCode.DAO;
using Repositories;

namespace ServerCode.Controllers
{
    [ApiController]
    [Route("/admin")]
    public class AdminController : Controller
    {
        //private readonly DBManager _dbManager;
        //public AdminController(DBManager dbManager)
        //{
        //    _dbManager = dbManager;
        //}
        //[HttpGet]
        //public string Authorize()
        //{
        //    if (HttpContext.Session.GetString("User") == "admin")
        //    {
        //        HttpContext.Session.SetInt32("IsAdmin", 1);
        //        return "인증 완료";
        //    }
        //    return "인증 실패";
        //}
        //[HttpGet("add-item")]
        //public async Task<bool> AddItem(ItemInfo itemInfo)
        //{
        //    if (HttpContext.Session.GetInt32("IsAdmin") != 1)
        //        return false;

        //    return await _dbManager.AddItemInfo(itemInfo);
        //}
        //[HttpGet("remove-item")]
        //public string RemoveItem(int itemId)
        //{
        //    if (HttpContext.Session.GetInt32("IsAdmin") != 1)
        //        return "You are not admin";
        //    if (_dbManager.RemoveItemInfo(itemId))
        //        return "Success Remove Item";
        //    return "Remove Failed";
        //}
        //[HttpGet("item-info")]
        //public ItemInfos? GetItemInfo()
        //{
        //    if (HttpContext.Session.GetInt32("IsAdmin") != 1)
        //        return null;
        //    ItemInfos items = new(_dbManager.GetItemInfos());
        //    return items;
        //}
    }
}
