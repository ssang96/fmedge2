using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace fmedge.UTIL
{
    public class HttpClientTransfer
    {
        public HttpClient httpClient { get; }

        public string azureWebAppURL { get; set; } = "https://skt-prd-kc-fm-app.azurewebsites.net";

        public HttpClientTransfer(HttpClient client)
        {   
            client.BaseAddress = new Uri(Startup.WebAppAddress);
            httpClient = client;
            Console.WriteLine($"Azure Web App Address : {httpClient.BaseAddress}");
        }

        public async Task<string> PostStatus(string data, string type)
        {
            string result = string.Empty;

            try
            {
                StringContent stringData = new StringContent(data, Encoding.UTF8, "application/json");

                httpClient.DefaultRequestHeaders.Add("type", type);
                var response = await httpClient.PostAsync("/event/fm/status", stringData);

                Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} [Controller : PostStatus] {data} Send To WebApp");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} [Contoller : PostStatus error] {ex.Message}");
                result = ex.Message;
            }

            return result;
        }

        public async Task<string> PostHealth(string data, string type)
        {
            string result = string.Empty;

            try
            {
                StringContent stringData = new StringContent(data, Encoding.UTF8, "application/json");

                httpClient.DefaultRequestHeaders.Add("type", type);
                var response = await httpClient.PostAsync("/event/fm/health", stringData);

                Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} [Controller : PostStatus] {data} Send To WebApp");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} [Contoller : PostHealth error] {ex.Message}");
                result = ex.Message;
            }

            return result;
        }
    }
}
