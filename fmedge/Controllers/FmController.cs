﻿using fmedge.Model;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace fmedge.Controllers
{
    [ApiController]
    public class FmController : ControllerBase
    {
        /// <summary>
        /// HttpClient 관리 팩토리 클래스 
        /// </summary>
        private readonly IHttpClientFactory httpClientFactory;

        /// <summary>
        /// 생성자(DI)
        /// </summary>
        /// <param name="httpClientFactory"></param>
        public FmController(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        // POST fm/status
        [HttpPost("event/fm/status")]
        public async Task<IActionResult> status([FromBody] EventStatus value)
        {
            EventResponse response = new EventResponse();
            response.resultCode = "OK";
            HttpResponseMessage result = null;
            String jsonData = string.Empty;

            Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")} Event Status Received {value.ToString()}");

            try
            {
                Controller.lastReceveDateTime = DateTime.Now;

                var type = Request.Headers["Type"].ToString();
                jsonData = JsonConvert.SerializeObject(value);

                //Status 데이터 생성 및 HttpClient를 통한 데이터 전송 후, 응답
                var statusRequest       = new HttpRequestMessage(HttpMethod.Post, "/event/fm/status");
                statusRequest.Content   = new StringContent(jsonData, Encoding.UTF8, "application/json");
                statusRequest.Headers.Add("Type", type);
                var client = httpClientFactory.CreateClient("azurewebapp");

                result = await client.SendAsync(statusRequest);
                Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")} [FmController : status]  {jsonData} Send To WebApp and Receive {result.StatusCode}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} [FmController : status error] {ex.Message}");
                response.resultCode = "NOK";
            }
           
            return Ok(JsonConvert.SerializeObject(response));
        }

        // POST fm/health
        [HttpPost("event/fm/health")]
        public async Task<IActionResult> health([FromBody] EventHealth value)
        {
            EventResponse response = new EventResponse();

            try
            {
                Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")} Event Health Received {value.ToString()}");

                var type = Request.Headers["Type"].ToString();
                String jsonData = JsonConvert.SerializeObject(value);

                //Status 데이터 생성 및 HttpClient를 통한 데이터 전송 후, 응답
                var statusRequest = new HttpRequestMessage(HttpMethod.Post, "/event/fm/health");
                statusRequest.Content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                statusRequest.Headers.Add("Type", type);
                var client = httpClientFactory.CreateClient("azurewebapp");
                client.Timeout = TimeSpan.FromSeconds(30);
                var result = await client.SendAsync(statusRequest);

                Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")} [FmController : health]  {jsonData} Send To WebApp and Receive {result.StatusCode}");
            }
            catch(Exception ex)
            {
                Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} [FmController : health error] {ex.Message}");
                response.resultCode = "NOK";
            }

            Controller.lastReceveDateTime = DateTime.Now;

            return Ok(JsonConvert.SerializeObject(response));
        }
    }
}
