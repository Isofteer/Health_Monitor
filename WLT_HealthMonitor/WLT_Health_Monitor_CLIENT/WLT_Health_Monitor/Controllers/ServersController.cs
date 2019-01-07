using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WLT_Health_Monitor.Data;
using WLT_Health_Monitor.Models;
using Newtonsoft.Json;

namespace WLT_Health_Monitor.Controllers
{
    public class ServersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public ServersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)

        {
            _context = context;
            _userManager = userManager;

        }
        public IActionResult ServerOverview()
        {
            return View();
        }


      
        public  IActionResult Detail( int id)
        {

           var serverLogs =  _context.wlt_ServerLogs.Where(n=>n.Connection_ID == id).OrderByDescending(n => n.DatabaseTime).Take(100);

            ViewBag.serverLogs = JsonConvert.SerializeObject(serverLogs.OrderBy(n => n.DatabaseTime).ToListAsync());
            ViewBag.serverID = id;
         

            return View(serverLogs);
        }
    }
}