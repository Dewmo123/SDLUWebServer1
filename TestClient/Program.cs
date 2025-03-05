using Newtonsoft.Json;
using System.Text;

namespace TestClient
{
    public class PlayerInfo()
    {
        public string? id;
        public string? password;
    }
    internal class Program
    {
        static HttpClient client = new HttpClient();
        static void Main(string[] args)
        {
            //SignUp();
            LogIn();
            while (true) { }
        }
        static async void LogIn()
        {
            string url = "http://172.31.0.250:3303/api/log-in";
            PlayerInfo pc = new PlayerInfo { id = "asdaaa", password = "qqwweedd" };
            string json = JsonConvert.SerializeObject(pc);
            HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage msg = await client.PostAsync(url, content);
            string result = await msg.Content.ReadAsStringAsync();
            Console.WriteLine(result);
            GetUserInfo();
        }
        static async void GetUserInfo()
        {
            string url = "http://172.31.0.250:3303/api/userinfo";
            HttpResponseMessage msg = await client.GetAsync(url);
            string result = await msg.Content.ReadAsStringAsync();
            Console.WriteLine(result);
        }
        static async void SignUp()
        {
            string url = "http://172.31.0.250:3303/api/sign-up";
            PlayerInfo pc = new PlayerInfo { id = "asdaaa", password = "qqwweedd" };
            string json = JsonConvert.SerializeObject(pc);
            HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage msg = await client.PostAsync(url, content);
            Console.WriteLine(msg.StatusCode);

        }
    }
}
