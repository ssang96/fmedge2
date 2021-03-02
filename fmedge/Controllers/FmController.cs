using fmedge.Model;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Net.Http;
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
        /// Http 데이터 전송을 위한 HttpClient 객체
        /// </summary>
        private HttpClient client;

        /// <summary>
        /// 생성자(DI)
        /// </summary>
        /// <param name="httpClientFactory"></param>
        public FmController(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
            client = httpClientFactory.CreateClient("azurewebapp");
        }

        // POST fm/status
        [HttpPost("event/fm/status")]
        public IActionResult status([FromBody] EventStatus value)
        {
            EventResponse response = new EventResponse();
            response.resultCode = "OK";
          
            try
            {
                Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")} Event Status Received {value}");
                var type = Request.Headers["Type"].ToString();
                var jsonData = JsonConvert.SerializeObject(value);

                Task<string> task = Task.Run<string>(async () => await Controller.PostStatus(client, jsonData, type));
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
        public IActionResult health([FromBody] EventHealth value)
        {
            EventResponse response = new EventResponse();

            try
            {
                Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")} Event Health Received {value.ToString()}");

                var type = Request.Headers["Type"].ToString();
                String jsonData = JsonConvert.SerializeObject(value);

                Task<string> task = Task.Run<string>(async () => await Controller.PostHealth(client, jsonData, type));
            }
            catch(Exception ex)
            {
                Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} [FmController : health error] {ex.Message}");
                response.resultCode = "NOK";
            }

            return Ok(JsonConvert.SerializeObject(response));
        }
    }
}
