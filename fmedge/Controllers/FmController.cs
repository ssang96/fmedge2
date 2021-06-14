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
        /// 생성자(DI)
        /// </summary>
        /// <param name="httpClientFactory"></param>
        public FmController(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;            
        }

        // POST fm/status
        [HttpPost("event/fm/status")]
        public IActionResult status([FromBody] EventStatus value)
        {
            EventResponse response = new EventResponse();
            response.resultCode = "OK";
            HttpClient client = null;

            try
            {
                client = httpClientFactory.CreateClient("azurewebapp");

                Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")} Event Status Received {value}");
                var type = Request.Headers["Type"].ToString();
                var jsonData = JsonConvert.SerializeObject(value);

                Task<bool> task = Task.Run<bool>(async () => await Controller.PostStatus(client, jsonData, type));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} [FmController : status error] {ex.Message}");
            }

            return Ok(JsonConvert.SerializeObject(response));
        }

        // POST fm/health
        [HttpPost("event/fm/health")]
        public IActionResult health([FromBody] EventHealth value)
        {
            EventResponse response = new EventResponse();
            HttpClient client = null;

            try
            {
                client = httpClientFactory.CreateClient("azurewebapp");

                Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")} Event Health Received {value}");

                var type = Request.Headers["Type"].ToString();

                value.type = type;
                String jsonData = JsonConvert.SerializeObject(value);

                Task<bool> task = Task.Run<bool>(async () => await Controller.PostHealth(client, jsonData, type));            
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
