﻿using fmedge.Controllers;
using fmedge.Model;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace fmedge.Util
{
    /// <summary>
    /// HttpClient를 사용해서 Azure Portal의 Web App으로 데이터를 전송하는 클래스
    /// </summary>
    class HttpClientTransfer
    {   
        public static async Task<string> PostComStatus(ComHttpPacket comStatus, string type)
        {
            string result = string.Empty;

            HttpClient client = null;

            try
            {
                var webappURL = Controller.azureWebAppURL;
                
                string json = JsonConvert.SerializeObject(comStatus);
                
                StringContent data = new StringContent(json, Encoding.UTF8, "application/json");

                client = new HttpClient();

                client.DefaultRequestHeaders.Add("type", type);          
                client.Timeout = TimeSpan.FromSeconds(60);

                var response = await client.PostAsync(new Uri(webappURL + "/event/fm/health"), data);

                Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} [HttpClientTransfer : PostWebAPI] {json} Send To WebApp and Receive {response.StatusCode}");

                client.Dispose();
                client = null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} [PostWebAPI connected error] {ex.Message}");
                result = ex.Message;
            }
            finally
            {
                if (client != null)
                    client.Dispose();
            }

            return result;
        }
    }
}

