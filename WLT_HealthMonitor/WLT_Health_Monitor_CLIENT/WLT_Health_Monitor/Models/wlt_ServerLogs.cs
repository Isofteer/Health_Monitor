using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WLT_HealthMonitor.Models
{
    public class wlt_ServerLogs
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ipkServerlogID { get; set; }
        public int Connection_ID { get; set; }
        public int ServerID { get; set; }
        public string ServerName { get; set; }
        public double DISK { get; set; }
        public double CPU { get; set; }      
        public double RAM { get; set; }
        public double MSMQ { get; set; }
        public double MSMQ_ALERTS { get; set; }
        public double MSMQ_UTSIN { get; set; }
        public DateTime SendDate { get; set; }
        public DateTime DatabaseTime { get; set; }
        public int isRead { get; set; }

    }
}

