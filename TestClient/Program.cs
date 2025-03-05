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
            
            while (true) { }
        }
        static async void LogIn()
        {

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
