using fmedge.Controllers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Client.Transport.Mqtt;
using Microsoft.Azure.Devices.Shared;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System;
using System.Runtime.Loader;
using System.Threading;
using System.Threading.Tasks;

namespace fmedge
{
    public class Program
    {
        private static ModuleClient ioTHubModuleClient;

        /// <summary>
        /// 엘리베이터 데이터 수신 및 WebApp으로 데이터 전송하는 클래스 
        /// </summary>
        private static Controller controller;

        /// <summary>
        /// 엘리베이터 서버 포트
        /// </summary>
        private static string hostPort { get; set; } = "8080";

        /// <summary>
        /// 이벤트 데이터를 전송 할 Azure Web App의 주소
        /// </summary>
        private static string azureWebAppAddress;

        /// <summary>
        /// 통신 상태 Interval
        /// </summary>
        private static string checkInterval;

        public static void Main(string[] args)
        {
            Init().Wait();

            // Wait until the app unloads or is cancelled
            var cts = new CancellationTokenSource();
            AssemblyLoadContext.Default.Unloading += (ctx) => cts.Cancel();
            Console.CancelKeyPress += (sender, cpe) => cts.Cancel();
            WhenCancelled(cts.Token).Wait();
        }

        /// <summary>
        /// Handles cleanup operations when app is cancelled or unloads
        /// </summary>
        public static Task WhenCancelled(CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<bool>();
            cancellationToken.Register(s => ((TaskCompletionSource<bool>)s).SetResult(true), tcs);
            return tcs.Task;
        }

        /// <summary>
        /// Initializes the ModuleClient and sets up the callback to receive
        /// messages containing temperature information
        /// </summary>
        static async Task Init()
        {
            MqttTransportSettings mqttSetting = new MqttTransportSettings(TransportType.Mqtt_Tcp_Only);
            ITransportSettings[] settings = { mqttSetting };

            // Open a connection to the Edge runtime
            ioTHubModuleClient = await ModuleClient.CreateFromEnvironmentAsync(settings);
            await ioTHubModuleClient.OpenAsync();
            Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} IoT Hub module client initialized.");

            // Read the TemperatureThreshold value from the module twin's desired properties
            var moduleTwin = await ioTHubModuleClient.GetTwinAsync();
            await OnDesiredPropertiesUpdate(moduleTwin.Properties.Desired, ioTHubModuleClient);

            // Attach a callback for updates to the module twin's desired properties.
            await ioTHubModuleClient.SetDesiredPropertyUpdateCallbackAsync(OnDesiredPropertiesUpdate, null);
        }

        static Task OnDesiredPropertiesUpdate(TwinCollection desiredProperties, object userContext)
        {
            try
            {
                Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} Desired property change:");
                Console.WriteLine(JsonConvert.SerializeObject(desiredProperties));

                var reportedProperties = new TwinCollection();

                if (desiredProperties["HostPort"] != null)
                {
                    hostPort = desiredProperties["HostPort"];
                    reportedProperties["HostPort"] = hostPort;

                    Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} HostPort : {hostPort}");
                }
                
                if (desiredProperties["AzureWebAppAddress"] != null)
                {
                    azureWebAppAddress = desiredProperties["AzureWebAppAddress"];
                    reportedProperties["AzureWebAppAddress"] = azureWebAppAddress;

                    Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} Azure Web App Address : {azureWebAppAddress}");
                }

                if (desiredProperties["CheckInterval"] != null)
                {
                    checkInterval = desiredProperties["CheckInterval"];
                    reportedProperties["CheckInterval"] = checkInterval;

                    Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} Check Interval : {checkInterval}");
                }

                if (reportedProperties.Count > 0)
                {
                    ioTHubModuleClient.UpdateReportedPropertiesAsync(reportedProperties).ConfigureAwait(false);
                }

                if (controller != null)
                {
                    controller.Dispose();
                    Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} Controller Class Disposed");
                    controller = null;
                }

                controller = new Controller(azureWebAppAddress, int.Parse(checkInterval));
          
                CreateHostBuilder(int.Parse(hostPort)).Build().StopAsync();
                CreateHostBuilder(int.Parse(hostPort)).Build().StartAsync();
            }
            catch (AggregateException ex)
            {
                foreach (Exception exception in ex.InnerExceptions)
                {
                    Console.WriteLine();
                    Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} Error when receiving desired property: {0}", exception);
                }
            }

            return Task.CompletedTask;
        }

        public static IHostBuilder CreateHostBuilder(int port) =>
            Host.CreateDefaultBuilder()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseUrls($"http://*:{port}");

                    Console.WriteLine($"UseURls : http://*:{port}");
                    webBuilder.UseStartup<Startup>();                 
                });
    }
}
