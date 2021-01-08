using fmedge.Model;
using fmedge.Util;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace fmedge.Controllers
{
    public class Board
    {
        public string title { get; set; }
        public string text { get; set; }
    }

    [ApiController]
    public class FmController : ControllerBase
    {
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

                Console.WriteLine("Event Status Received" );
                response.resultCode = "OK";

                string task = await HttpClientTransfer.PostEventStatus(value, type);

                Console.WriteLine(task);
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
                Console.WriteLine("Event Health Received");
                response.resultCode = "OK";

                string task = await HttpClientTransfer.PostMiddleWareHealth(value, type);

                Console.WriteLine(task);
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
