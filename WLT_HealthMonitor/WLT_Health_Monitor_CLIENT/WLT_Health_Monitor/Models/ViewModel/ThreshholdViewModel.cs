using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WLT_Health_Monitor.Models.ViewModel
{
    public class ThreshholdViewModel
    {  
         
            public int ipkThreshholdID { get; set; }
            public string Resource { get; set; }
            public string ifkUserID { get; set; }
            public int ifkServerID { get; set; }
            public double ResouceValue { get; set; }
            public string Host { get; set; }
            public string Connection { get; set; }

    }
    }
