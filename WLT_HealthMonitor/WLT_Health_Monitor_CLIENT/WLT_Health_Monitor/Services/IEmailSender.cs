using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WLT_Health_Monitor.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}
