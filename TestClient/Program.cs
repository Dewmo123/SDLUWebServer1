using Newtonsoft.Json;
//using ServerCode.Controllers;
using ServerCode.Models;
using System.Text;

namespace TestClient
{

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
            await GetUserInfo();
            UpdateItems();
        }
        static async Task GetUserInfo()
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

        static async void UpdateItems()
        {
            string url = "http://172.31.0.250:3303/api/update-items";
            //var request = new ItemUpdateRequest
            //{
            //    PlayerId = "asdaaa",
            //    Updates = new List<PlayerItemInfo>
            //    {
            //        new PlayerItemInfo("asdaaa",4,10),
            //        new PlayerItemInfo("asdaaa",5,10),
            //    }
            //};

            //string json = JsonConvert.SerializeObject(request);
            //Console.WriteLine(json);
            //HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");
            //HttpResponseMessage msg = await client.PostAsync(url, content);
            //string result = await msg.Content.ReadAsStringAsync();
            //Console.WriteLine($"아이템 업데이트 결과: {result}");
        }
    }
}
