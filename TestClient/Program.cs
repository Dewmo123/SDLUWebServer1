using Newtonsoft.Json;
//using ServerCode.Controllers;
using ServerCode.DAO;
using ServerCode.DTO;
using System.Net.Http.Json;
using System.Text;

namespace TestClient
{

    internal class Program
    {
        static HttpClient client = new HttpClient();
        static void Main(string[] args)
        {
            LogIn(3, UpgradeWeapon);
            while (true) { }
        }
        static async void UpgradeWeapon()
        {
            string url = "http://localhost:3303/api/player-data/upgrade-equipment";
            string json = JsonConvert.SerializeObject(EquipType.Weapon);
            HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage msg = await client.PatchAsync(url, content);
            string res = await msg.Content.ReadAsStringAsync();
            Console.WriteLine(res);
        }
        static async void AddItemToPlayer()
        {
            string url = "http://localhost:3303/api/update-item";
            PlayerItemVO itemInfo = new PlayerItemVO()
            {
                itemName = "sword",
                playerId = "qwweewq2",
                quantity = 100
            };
            string json = JsonConvert.SerializeObject(itemInfo);
            HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage msg = await client.PostAsync(url, content);
            string result = await msg.Content.ReadAsStringAsync();
            Console.WriteLine(result);

        }
        static async void CancelAuctionItem()
        {
            string url = "http://localhost:3303/api/auction/cancel?itemName=mantis&pricePerUnit=5";
            HttpResponseMessage msg = await client.DeleteAsync(url);
            string result = await msg.Content.ReadAsStringAsync();
            Console.WriteLine(result);
        }
        static async void AddItemToAuction()
        {
            string url = "http://localhost:3303/api/auction/post";
            var itemInfo = new AuctionItemVO()
            {
                itemName = "mantis",
                playerId = "qwer",
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
            var itemInfo = new AuctionItemVO()
            {
                itemName = "sword",
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
            var itemInfo = new BuyerDTO()
            {
                buyCount = 5,
                buyerId = "qwweewq1",
                itemInfo = new AuctionItemVO()
                {
                    itemName = "sword",
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
            string url = "http://localhost:3303/api/player-info/log-in";
            PlayerDTO pc = new PlayerDTO { id = $"qwweewq{id}", password = "qqwweedd" };
            string json = JsonConvert.SerializeObject(pc);
            HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");
            Console.WriteLine("LogIN");
            HttpResponseMessage msg = await client.PostAsync(url, content);
            string result = await msg.Content.ReadAsStringAsync();
            Console.WriteLine($"{result}");
            callback();
        }
        static async void GetUserInfo()
        {
            string url = "http://localhost:3303/api/player/get-my-data";
            HttpResponseMessage msg = await client.GetAsync(url);
            string result = await msg.Content.ReadAsStringAsync();
            Console.WriteLine(result);
            PlayerDataVO? info = JsonConvert.DeserializeObject<PlayerDataVO>(result);

        }
        static async void SignUp(int id)
        {
            string url = "http://localhost:3303/api/player-info/sign-up";
            PlayerVO pc = new PlayerVO { id = $"qwweewq{id}", password = "qqwweedd" };
            string json = JsonConvert.SerializeObject(pc);
            HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage msg = await client.PostAsync(url, content);
            string result = await msg.Content.ReadAsStringAsync();
            Console.WriteLine(result);
            LogIn(id, UpgradeWeapon);
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
