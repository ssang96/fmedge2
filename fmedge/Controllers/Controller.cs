using fmedge.Model;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace fmedge.Controllers
{
    public class Controller
    {
        /// <summary>
        /// Azure Web App의 주소
        /// </summary>
        public static string azureWebAppURL { get; set; } = "https://skt-prd-kc-fm-app.azurewebsites.net";

        /// <summary>
        /// 
        /// </summary>
        public static DateTime lastReceveDateTime;

        /// <summary>
        /// KEP 서버 미들웨어와  엣지간의 통신 상태를 체크하는 Timer
        /// </summary>
        private Timer comCheckTimer = null;

        /// <summary>
        /// Timer Interval
        /// </summary>
        private int checkTimeInterval { get; set; } = 30;

        /// <summary>
        /// 마지막으로 수신한 데이터의 시간차 기준 시간
        /// </summary>
        private int checkDataTimeInterval { get; set; } = 5;

        /// <summary>
        /// 생성자
        /// 초기화
        /// </summary>
        public Controller()
        {
            comCheckTimer = new Timer();
            comCheckTimer.Interval = checkTimeInterval * 1000;

            comCheckTimer.Elapsed += new ElapsedEventHandler(CommCheck);
            comCheckTimer.Enabled = true;
        }

        /// <summary>
        /// 생성자
        /// 초기화int.Parse(hostPort), azureWebAppAddress, checkInterval
        /// </summary>
        public Controller(string webapp, int interval, int dataTimeInterval)
        {
            comCheckTimer = new Timer();

            checkTimeInterval = interval;
            azureWebAppURL = webapp;
            checkDataTimeInterval = dataTimeInterval;

            comCheckTimer.Interval = checkTimeInterval * 1000;

            comCheckTimer.Elapsed += new ElapsedEventHandler(CommCheck);
            comCheckTimer.Enabled = true;
        }

        /// <summary>
        /// IBS와 엣지간의 통신 상태를 체크하는 Timer 메소드
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CommCheck(object sender, ElapsedEventArgs e)
        {
            Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} [Controller : CommCheck] Check Commmunication between Middleware and Edge");

            try
            {
                if (lastReceveDateTime != null)
                {
                    string dataType = "";

                    TimeSpan timeDiff = DateTime.Now - lastReceveDateTime;

                    ComHttpPacket comHttpPacket = new ComHttpPacket();

                    comHttpPacket.building_id = "nise";
                    comHttpPacket.inspection_datetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                    //수신한 데이터가 없음
                    if (timeDiff.TotalMinutes >= checkDataTimeInterval)
                    {
                        dataType = "emergency";
                        comHttpPacket.inspection_result_val = "message not received";
                        comHttpPacket.inspection_result_cd = "1";
                    }
                    else //정상
                    {
                        dataType = "general";
                        comHttpPacket.inspection_result_val = "message received";
                        comHttpPacket.inspection_result_cd = "0";
                    }
                    Task<bool> task = Task.Run<bool>(async () => await PostComStatus(comHttpPacket, dataType));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} [Controller : CommCheck] {ex.Message}");
            }
        }

        /// <summary>
        /// 정해진 시간마다 타이머에 의해서 실행되며, nise와의 통신 체크를 하는 메소드로 가장
        /// 최근에 수신한 데이터의 시간을 기준으로 체크
        /// </summary>
        /// <param name="comStatus"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static async Task<bool> PostComStatus(ComHttpPacket comStatus, string type)
        {
            bool result = true;
            HttpClient client = null;

            try
            {
                string json = JsonConvert.SerializeObject(comStatus);

                StringContent data = new StringContent(json, Encoding.UTF8, "application/json");

                client = new HttpClient();

                client.DefaultRequestHeaders.Add("type", type);
                client.Timeout = TimeSpan.FromMinutes(2);

                var response = await client.PostAsync(new Uri(azureWebAppURL + "/event/fm/health"), data);

                Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} [Controller : PostComStatus] {json} Send To WebApp and Receive {response.StatusCode}");

                client.Dispose();
                client = null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} [Controller : PostComStatus error] {ex.Message}");
                result = false;
            }
            finally
            {
                if (client != null)
                    client.Dispose();
            }

            return result;
        }

        /// <summary>
        /// NISE에서 전송한 FM 데이터를 Web App으로 전송하는 메소드
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static async Task<bool> PostStatus(HttpClient client, string data, string type)
        {
            bool result = true;

            StringContent stringData = null;

            try
            {
                lastReceveDateTime = DateTime.Now;

                stringData = new StringContent(data, Encoding.UTF8, "application/json");

                client.DefaultRequestHeaders.Add("type", type);
               
                var response = await client.PostAsync(new Uri(azureWebAppURL + "/event/fm/status"), stringData);
                Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} [Controller : PostStatus] {data} Send To WebApp");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} [Contoller : PostStatus error] Data : {data}");
                Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} [Contoller : PostStatus error] {ex.Message}");

                result = false;
            }

            return result;
        }

        /// <summary>
        /// nise에서 전송한 각 건물의 통신 상태 데이터를 Web App으로 전송하는 메소드 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static async Task<bool> PostHealth(HttpClient client, string data, string type)
        {
            bool result = true;
            
            try
            {
                StringContent stringData = new StringContent(data, Encoding.UTF8, "application/json");

                client.DefaultRequestHeaders.Add("type", type);

                var response = await client.PostAsync(new Uri(azureWebAppURL + "/event/fm/health"), stringData);
                Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} [Controller : PostHealth] {data} Send To WebApp");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} [Contoller : PostHealth error] Data : {data}");
                Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} [Contoller : PostHealth error] {ex.Message}");
                result = false;
            }

            return result;
        }

        /// <summary>
        /// 타이머를 정리하는 메소드 
        /// </summary>
        public void Dispose()
        {
            if (this.comCheckTimer != null && this.comCheckTimer.Enabled)
            {
                this.comCheckTimer.Enabled = false;
                this.comCheckTimer.Dispose();
            }

            // Suppress finalization.
            GC.SuppressFinalize(this);
        }
    }
}
