using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WLT_HealthMonitor.Models
{
    public class wlt_HealthNotificationHistory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ipkHistoryID { get; set; }

        public DateTime DateSent { get; set; }
        public string ifkUserID { get; set; }
        public int ifkServerID { get; set; }
        public int isSent { get; set; }
        public string  Resource { get; set; }
        public double ResourceValue { get; set; }



    }
}
