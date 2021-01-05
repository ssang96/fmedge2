using System;
using System.Threading.Tasks;
using System.Timers;

namespace fmedge.Controllers
{
    public class Controller
    {
        /// <summary>
        /// Azure Web App의 주소
        /// </summary>
        public string azureWebAppURL { get; set; } = "https://skt-stg-kc-fm-app.azurewebsites.net";

        /// <summary>
        /// 
        /// </summary>
        public static DateTime lastReceveDateTime;

        /// <summary>
        /// IBS와 엣지간의 통신 상태를 체크하는 Timer
        /// </summary>
        private Timer comCheckTimer = null;


        private int timeInterval { get; set; }

        /// <summary>
        /// 생성자
        /// 초기화
        /// </summary>
        public Controller()
        { 
            comCheckTimer = new Timer();
            comCheckTimer.Interval = timeInterval;

            comCheckTimer.Elapsed += new ElapsedEventHandler(CommCheck);
            comCheckTimer.Enabled = true;
        }

        /// <summary>
        /// 생성자
        /// 초기화int.Parse(hostPort), azureWebAppAddress, checkInterval
        /// </summary>
        public Controller(string webapp, int interval )
        {
            comCheckTimer = new Timer();

            timeInterval = interval;
            azureWebAppURL = webapp;

            comCheckTimer.Interval = timeInterval * 1000;

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
            Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} [Controller : CommCheck] Check Commmunication between IBS and Edge");

            if (lastReceveDateTime != null)
            {
                /*
                ComHttpPacket comHttpPacket = new ComHttpPacket();

                try
                {
                    var dataType = "";

                    comHttpPacket.building_id = this.buildingID;
                    comHttpPacket.inspection_datetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                    if (ClientSocket.IsConnected)
                    {
                        dataType = "general";
                        comHttpPacket.inspection_result_val = "connected";
                        comHttpPacket.inspection_result_cd = "0";

                        Task<string> task = Task.Run<string>(async () => await HttpClientTransfer.PostWebAPI(azureWebAppURL, comHttpPacket, this.buildingID, this.deviceID, dataType));
                    }
                    else
                    {
                        dataType = "emergency";
                        comHttpPacket.inspection_result_val = "disconnected";
                        comHttpPacket.inspection_result_cd = "1";

                        Task<string> task = Task.Run<string>(async () => await HttpClientTransfer.PostWebAPI(azureWebAppURL, comHttpPacket, this.buildingID, this.deviceID, dataType));
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} [Controller : CommCheck] {ex.Message}");
                }
                */
            }
        }

        /// <summary>
        /// 소켓 클라이언트 정리하는 메소드
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
