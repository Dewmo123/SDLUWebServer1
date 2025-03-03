using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using ServerCode.Core;



namespace ServerCode.Controllers
{
    [ApiController]
    [Route("/api")]
    public class PlayerDataController : Controller
    {
        [HttpGet("sign-up")]
        public bool SignUp(string id, string password)
        {
            if (id.Length > 8 && password.Length > 20)
                return false;
            return DBManager.Instance.SignUp(id, password);
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
