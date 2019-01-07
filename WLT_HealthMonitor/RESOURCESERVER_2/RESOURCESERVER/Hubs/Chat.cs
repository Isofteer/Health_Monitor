using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RESOURCESERVER.Resource;
using Newtonsoft.Json;

using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;
using RESOURCESERVER.ResourceMonitor;

namespace RESOURCESERVER
{
   
    namespace SignalRChat
    {
        public class ChatHub : Hub
        {
            public static System.Timers.Timer _timer = new System.Timers.Timer();
            public static int REFRESHRATE = 2;   //refresh rate  * 10000

            private static IHubContext hubContext;

            public ChatHub()
            {

                hubContext = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();


            }
            public override Task OnConnected()
            {
             
                

                _timer.Dispose();

                _timer = new System.Timers.Timer();

                _timer.Interval = REFRESHRATE *1000;

                _timer.Elapsed += TimerElapsed;

                _timer.Start();

                return base.OnConnected();
            }

        
            public void Send(string name, string message)

            {
                var Values = JsonConvert.DeserializeObject<Reception>(message);

                REFRESHRATE = Convert.ToInt32( Values.RefreshRate);

                Clients.All.GetClienturl(Values.Client);

            }
           

            public void SendUptime()
            {

                Clients.All.Uptime(Helpers.CreateDate(Helpers.UpTime));
                 //Clients.All.Uptime(ResourceService.PrintString());


            }
            public void TimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
            {
               // var HubContext = GlobalHost.ConnectionManager.GetHubContext("ChatHub");

                var windowsPerfomanceMonitors = new WindowsResourceMonitors();

                var readings = windowsPerfomanceMonitors.getAllReadings();


                Clients.All.broadcastMessage("reading..", readings);
                
                SendUptime();
                
            }

        }
    }
    public class Reception {

        public Reception() { }
        public string RefreshRate { get; set; }
        public string Client { get; set; }

    }
}