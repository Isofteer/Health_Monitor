using Microsoft.AspNet.SignalR;
using RESOURCESERVER.ResourceMonitor;
using RESOURCESERVER.SignalRChat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RESOURCESERVER.Resource
{
    public static class ResourceService
    {
       // public IHubContext<ChatHub> _hubContext;


      
        
      public static string PrintString()
        {
            Helpers.CreateDate(Helpers.UpTime);

            return "ResourceService string ";
        }
      

    }
}