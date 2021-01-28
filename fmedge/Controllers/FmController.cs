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

            try
            {
                var type = Request.Headers["Type"].ToString();   
                
                if(type == "")
                {
                    response.resultCode = "NOK";
                    return Ok(JsonConvert.SerializeObject(response));
                }

                Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} Event Status Received {value.ToString()}");

                //Status 데이터 생성 및 HttpClient를 통한 데이터 전송 후, 응답
                var statusRequest = new HttpRequestMessage(HttpMethod.Post, "/event/fm/status");
                string jsonData = JsonConvert.SerializeObject(value);
                statusRequest.Content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                statusRequest.Headers.Add("Type", type);
                var client = httpClientFactory.CreateClient("azurewebapp");

                Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} [FmController : PostWebAPI] {jsonData} Send To WebApp");

                var result = await client.SendAsync(statusRequest);

                Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} [FmController : PostWebAPI] {result.StatusCode} Received From WebApp");

                response.resultCode = "OK";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} [FmController : status] {ex.Message}");
                response.resultCode = "NOK";
            }

            Controller.lastReceveDateTime = DateTime.Now;

            return Ok(JsonConvert.SerializeObject(response));
        }

        // POST fm/health
        [HttpPost("event/fm/health")]
        public async Task<IActionResult> health([FromBody] EventHealth value)
        {
            EventResponse response = new EventResponse();

            try
            {
                var type = Request.Headers["Type"].ToString();

                if (type == "")
                {
                    response.resultCode = "NOK";
                    return Ok(JsonConvert.SerializeObject(response));
                }
                Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} Event Health Received :  {value.ToString()}");           

                //Health 데이터 생성 및 HttpClient를 통한 데이터 전송 후, 응답
                var statusRequest = new HttpRequestMessage(HttpMethod.Post, "/event/fm/health");
                string jsonData = JsonConvert.SerializeObject(value);
                statusRequest.Content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                statusRequest.Headers.Add("Type", type);
                var client = httpClientFactory.CreateClient("azurewebapp");

                Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} [FmController] {jsonData} Send To WebApp");

                var result = await client.SendAsync(statusRequest);

                Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} [FmController] {result.StatusCode} Received From WebApp");

                response.resultCode = "OK";      
            }
            catch(Exception ex)
            {
                Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} [FmController : health] {ex.Message}");
                response.resultCode = "NOK";
            }

            Controller.lastReceveDateTime = DateTime.Now;

            return Ok(JsonConvert.SerializeObject(response));
        }
    }
}
