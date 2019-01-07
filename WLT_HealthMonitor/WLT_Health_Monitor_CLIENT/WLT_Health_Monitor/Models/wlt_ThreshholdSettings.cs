using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WLT_HealthMonitor.Models
{
    public class wlt_ThreshholdSettings
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ipkThreshholdID { get; set; }     
        public string Resource { get; set; }        
        public string ifkUserID { get; set; }
        public int ifkServerID { get; set; }
        public double ResouceValue { get; set; }
      
    }
}
