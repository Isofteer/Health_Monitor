using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WLT_HealthMonitor.Models
{
    public class wlt_Connections
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int conn_ID { get; set; }

        public string Connection { get; set; }

        public String Host { get; set; }




    }
}
