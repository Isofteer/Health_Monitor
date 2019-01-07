using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RESOURCESERVER.ResourceMonitor
{
    public class ModelResource
    {

        public ModelResource() {
        }

        
       public string SERVERNAME { get; set; }

        public string COMPUTERNAME { get; set; }
        public double RAM { get; set; }
        public double CPU { get; set; }
        public double DISK { get; set; }
        public double MSMQ { get; set; }

        public long MSMQ_ALERTS{ get; set; }

        public long MSMQ_UTSIN { get; set; }

        public DateTime SendDate { get; set; }
        public string CORES { get; set; }



    }
}