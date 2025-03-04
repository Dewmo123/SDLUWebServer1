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
            Console.WriteLine(info.id);
            Console.WriteLine(info.password);
            if (info.id.Length > 8 && info.password.Length > 20)
                return false;
            return DBManager.Instance.SignUp(info.id, info.password);
        }
        [HttpGet("log-in")]
        public string Login(string id, string password)
        {
            string? msg;
            if (DBManager.Instance.LogIn(id, password))
            {
                msg = $"Hello {id}";
                HttpContext.Session.SetString("User", id);
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
    }
}
