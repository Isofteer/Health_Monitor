using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WLT_Health_Monitor.Models
{
    public class NotificationViewModel
    {
        public int Server_ID { get; set; }
        public string Server { get; set; }
        public string Resource { get; set; }
        public double ResourceValue { get; set; }
        public int isSent { get; set; }
        public DateTime SentDate { get; set; }
        public string  MailAddress {get;set;}


    }
}
