using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WLT_HealthMonitor.Models
{
    public class wlt_Configurations
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int configuration_ID { get; set; }
        public int ServerID { get; set; }
        public int RefreshRate { get; set; }
    }
}
