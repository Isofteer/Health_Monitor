using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace RESOURCESERVER.Resource
{
    public static class Helpers
    {
        public  static TimeSpan UpTime
        {
            get
            {
                using (var uptime = new PerformanceCounter("System", "System Up Time"))
                {
                    uptime.NextValue();       //Call this an extra time before reading its value
                    return TimeSpan.FromSeconds(uptime.NextValue());
                }
            }
        }
        public static string CreateDate(TimeSpan span)
        {

            var _strDate = "";
            if (span.Days > 1)
            {
                _strDate = span.Days + "day/s " + span.Hours + ":" + span.Minutes + ":" + span.Seconds;

            }
            else
            {
                _strDate = span.Hours + ":" + span.Minutes + ":" + span.Seconds;

            }


            return _strDate;
        }
    }
}