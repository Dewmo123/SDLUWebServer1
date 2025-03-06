using Newtonsoft.Json;
using System.Text;

namespace TestClient
{
    public class PlayerInfo()
    {
        public string? id;
        public string? password;
    }

    public class ItemUpdateRequest
    {
        public string PlayerId { get; set; } = null!;
        public Dictionary<int, int> ItemUpdates { get; set; } = new();
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
            var request = new ItemUpdateRequest
            {
                PlayerId = "asdaaa",
                ItemUpdates = new Dictionary<int, int>
                {
                    { 4, -11 },    
                    { 5, -9 }      
                }
            };

            string json = JsonConvert.SerializeObject(request);
            HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage msg = await client.PostAsync(url, content);
            string result = await msg.Content.ReadAsStringAsync();
            Console.WriteLine($"아이템 업데이트 결과: {result}");
        }
    }
}
