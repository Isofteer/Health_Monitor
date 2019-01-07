using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WLT_Health_Monitor.Models.ViewModel
{
   
        public class ConnetionViewModel
        {
            public ConnetionViewModel()
            {
            }

        public int configuration_ID { get; set; }
        public int ServerID { get; set; }
        public int RefreshRate { get; set; }
        public int conn_ID { get; set; }

        public string Connection { get; set; }

        public String Host { get; set; }


    }
}
