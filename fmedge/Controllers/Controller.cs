using fmedge.Model;
using fmedge.Util;
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
        public Controller(string webapp, int interval, int dataTimeInterval )
        {
            comCheckTimer = new Timer();

            checkTimeInterval       = interval;
            azureWebAppURL          = webapp;
            checkDataTimeInterval   = dataTimeInterval;

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
                    Task<string> task = Task.Run<string>(async () => await HttpClientTransfer.PostComStatus(comHttpPacket, dataType));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} [Controller : CommCheck] {ex.Message}");
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
