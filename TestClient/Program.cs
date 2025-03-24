using Newtonsoft.Json;
//using ServerCode.Controllers;
using ServerCode.Models;
using System.Net.Http.Json;
using System.Text;

namespace TestClient
{

    internal class Program
    {
        static HttpClient client = new HttpClient();
        static void Main(string[] args)
        {
            for (int i = 0; i < 100; i++)
                LogIn(2, null);
            while (true) { }
        }
        static async void AddItemToPlayer()
        {
            string url = "http://localhost:3303/api/update-item";
            PlayerItemInfo itemInfo = new PlayerItemInfo()
            {
                itemId = 1,
                playerId = "qwweewq2",
                quantity = 100
            };
            string json = JsonConvert.SerializeObject(itemInfo);
            HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage msg = await client.PostAsync(url, content);
            string result = await msg.Content.ReadAsStringAsync();
            Console.WriteLine(result);

        }
        static async void AddItemToAuction()
        {
            string url = "http://localhost:3303/api/auction/post";
            var itemInfo = new AuctionItemInfo()
            {
                itemId = 1,
                playerId = "qwweewq2",
                pricePerUnit = 5,
                quantity = 20
            };
            string json = JsonConvert.SerializeObject(itemInfo);
            HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage msg = await client.PostAsync(url, content);
            string result = await msg.Content.ReadAsStringAsync();
            Console.WriteLine(result);
        }
        static async void CancelItemFromAuction()
        {
            string url = "http://localhost:3303/api/auction/cancel?playerId=qwweewq2&itemId=1&pricePerUnit=5&quantity=3";
            var itemInfo = new AuctionItemInfo()
            {
                itemId = 1,
                playerId = "qwweewq2",
                pricePerUnit = 5,
            };
            string json = JsonConvert.SerializeObject(itemInfo);
            HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage msg = await client.DeleteAsync(url);
            string result = await msg.Content.ReadAsStringAsync();
            Console.WriteLine(result);
        }
        static async void PurchaseItemFromAuction()
        {
            string url = "http://localhost:3303/api/auction/purchase-item";
            var itemInfo = new BuyerInfo()
            {
                buyCount = 5,
                buyerId = "qwweewq1",
                itemInfo = new AuctionItemInfo()
                {
                    itemId = 1,
                    playerId = "qwweewq2",
                    pricePerUnit = 5,
                    quantity = 10
                }
            };
            string json = JsonConvert.SerializeObject(itemInfo);
            HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage msg = await client.PostAsync(url, content);
            string result = await msg.Content.ReadAsStringAsync();
            Console.WriteLine(result);
        }
        static async void LogIn(int id, Action callback)
        {
            string url = "http://172.31.1.229:3303/api/player/log-in";
            PlayerInfo pc = new PlayerInfo { id = $"qwer", password = "1234" };
            string json = JsonConvert.SerializeObject(pc);
            HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");
            Console.WriteLine("LogIN");
            HttpResponseMessage msg = await client.PostAsync(url, content);
            string result = await msg.Content.ReadAsStringAsync();
            Console.WriteLine(result);
        }
        static async Task GetUserInfo()
        {
            string url = "http://172.31.0.250:3303/api/userinfo";
            HttpResponseMessage msg = await client.GetAsync(url);
            string result = await msg.Content.ReadAsStringAsync();
            Console.WriteLine(result);
        }
        static async void SignUp(int id)
        {
            string url = "http://localhost:3303/api/player/sign-up";
            PlayerInfo pc = new PlayerInfo { id = $"qwweewq{id}", password = "qqwweedd" };
            string json = JsonConvert.SerializeObject(pc);
            HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage msg = await client.PostAsync(url, content);
            string result = await msg.Content.ReadAsStringAsync();
            Console.WriteLine(result);
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
