using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WLT_HealthMonitor.Models;
using WLT_Health_Monitor.Data;
using System.Reflection;
using WLT_Health_Monitor.Mail;
using Microsoft.AspNetCore.Identity;
using WLT_Health_Monitor.Models;
using WLT_Health_Monitor.Services;
using Microsoft.Extensions.Logging;
using WLT_Health_Monitor.Helpers;
using WLT_Health_Monitor.Models.ViewModel;

namespace WLT_Health_Monitor.Controllers
{
    public class wlt_ServerLogsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public wlt_ServerLogsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager   )

        {
            _context = context;
            _userManager = userManager;
          
        }

        // GET: wlt_ServerLogs
        public async Task<IActionResult> Index(string QueryDate,int signal)
        {
          var _QueryDate = QueryDate  ==null? DateTime.Now :  Convert.ToDateTime(QueryDate);

            List<wlt_ServerLogs> serverLogs = new List<wlt_ServerLogs>();

            ViewData["signal"] =  String.IsNullOrEmpty(signal.ToString()) ? 2 : signal;
            ViewData["QueryDate"] = String.IsNullOrEmpty(QueryDate) ? DateTime.Now : _QueryDate;


            if (String.IsNullOrEmpty(QueryDate))
            {
                serverLogs = await _context.wlt_ServerLogs.OrderByDescending(n => n.SendDate).Take(300).ToListAsync();
            }
            else if (signal == 1)
            {
                serverLogs = await (from log in _context.wlt_ServerLogs
                                    where log.SendDate.ToString("yyyy-MM-dd HH:mm") == _QueryDate.ToString("yyyy-MM-dd HH:mm")
                                    select log).OrderByDescending(n => n.SendDate).ToListAsync();

            }
            else if (signal ==2)
            {
                serverLogs = await (from log in _context.wlt_ServerLogs
                                    where  log.SendDate.ToString("yyyy-MM-dd HH") == _QueryDate.ToString("yyyy-MM-dd HH")
                                    select log).OrderByDescending(n => n.SendDate).ToListAsync();

            }
         
            else if (signal == 3)
            {
                serverLogs = await (from log in _context.wlt_ServerLogs
                                    where log.SendDate.ToString("yyyy-MM-dd") == _QueryDate.ToString("yyyy-MM-dd")
                                    select log).OrderByDescending(n => n.SendDate).ToListAsync();

            }



            return View(serverLogs);
        }
        [HttpPost]
        public void Getserverlogs([FromBody]  object QueryDate)
        {
            var _date = Convert.ToDateTime(QueryDate);

            var utcdate = TimeHelpers.ConertTimeLocalTimeToUTC(_date,"");

           
             _context.wlt_ServerLogs.OrderByDescending(n => n.SendDate ).Take(300).ToListAsync();
        }
        [HttpPost]
        public async Task<IActionResult> Save([FromBody]  wlt_ServerLogs wlt_ServerLogs)
        {
            if (ModelState.IsValid)
            {
                wlt_ServerLogs.DatabaseTime = DateTime.UtcNow;
                _context.Add(wlt_ServerLogs);

                await _context.SaveChangesAsync();

                //var userSettings = _context.wlt_ThreshholdSettings
                //.Where(server => server.ifkServerID == wlt_ServerLogs.Connection_ID);

                var userSettings = from m in _context.wlt_ThreshholdSettings
                                   join d in _context.wlt_Connections on m.ifkServerID equals d.conn_ID
                                   where m.ifkServerID == wlt_ServerLogs.Connection_ID
                                   select new ThreshholdViewModel()
                                   {
                                       ResouceValue = m.ResouceValue,
                                       Resource = m.Resource,
                                       ifkUserID = m.ifkUserID,
                                       ifkServerID = m.ifkServerID,
                                       Host = d.Host,
                                       Connection = d.Connection,

                                   };






                var properties = wlt_ServerLogs.GetType().GetProperties();


               var _notificationList = new Dictionary<int, Tuple<wlt_HealthNotificationHistory,string,double,string>>();

                // var _notificationList = new List<wlt_HealthNotificationHistory>();
                int key = 0;
                foreach (var setting in userSettings)
                {
                    foreach (var property in properties)
                    {
                       
                        if (property.Name == setting.Resource)
                        {
                            var arrivingValue = Convert.ToDouble( property.GetValue(wlt_ServerLogs, null));

                            if (arrivingValue >= setting.ResouceValue)
                            {
                                key++;

                                var user = await _userManager.FindByIdAsync(setting.ifkUserID);

                                _notificationList.Add(key, new Tuple<wlt_HealthNotificationHistory, string, double, string>
                                   (
                                    new wlt_HealthNotificationHistory
                                    {
                                        ifkUserID = setting.ifkUserID,

                                        ifkServerID = wlt_ServerLogs.Connection_ID,

                                        DateSent = DateTime.UtcNow,

                                        isSent = 1,

                                        Resource = property.Name,

                                        ResourceValue = arrivingValue




                                    },
                                    user.Email ,

                                    setting.ResouceValue,

                                    setting.Host

                                    )
                                );                                                                        
                                

                            }

                        }

                    }
                }

                foreach (var item in _notificationList)
                {                  

                  var _mail = new EMail();

                  var threshhold = "The Server  "+ item.Value.Item4 + "   has  its " + item.Value.Item1.Resource + " execeeding " + item.Value.Item3 + " %";

                    var result = false;

                    var _canSendMail = CanSendMail(item.Value.Item1.Resource, item.Value.Item3);

                    if (_canSendMail.Item1)
                    {
                         result = _mail.Send(item.Value.Item2, threshhold);

                        foreach (var serverLogs in _canSendMail.Item2)
                        {
                            serverLogs.isRead = 1;

                            _context.wlt_ServerLogs.Update(serverLogs);
                        }

                      
                        
                    }
                    if (!result)
                    {
                        item.Value.Item1.isSent = 0;
                    }
                    else {
                        item.Value.Item1.isSent = 1;
                    }

                    _context.Add(item.Value.Item1);
                }
               

                _context.SaveChanges();



            }
            return Ok(wlt_ServerLogs);
        }


        private Tuple<bool,IQueryable<wlt_ServerLogs>> CanSendMail( string _Resource,double _Value )
        {

            // go get the history of the 90  and above

            var seltContext = _context.wlt_ServerLogs;
            

            var LowerLimitTime = DateTime.UtcNow.AddSeconds(-120);

            var recentServerLog = seltContext.Where(n => n.DatabaseTime >= LowerLimitTime  && n.isRead ==0);

            var count = 0;

            try
            {
                count = recentServerLog.Where(n => Convert.ToDouble(n.GetType().GetProperty(_Resource).GetValue(n, null)) >= _Value).Count();
            }
            catch (Exception ex)
            {

            }
            var result = false;    

            if (count >= 20)
            {
                result = true;
            }


            return new Tuple<bool, IQueryable<wlt_ServerLogs>>(result, recentServerLog);      
        }
        
        // GET: wlt_ServerLogs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var wlt_ServerLogs = await _context.wlt_ServerLogs
                .SingleOrDefaultAsync(m => m.ipkServerlogID == id);
            if (wlt_ServerLogs == null)
            {
                return NotFound();
            }

            return View(wlt_ServerLogs);
        }

        // GET: wlt_ServerLogs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: wlt_ServerLogs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ipkServerlogID,ServerID,ServerName,DISK,CPU,RAM,MSMQ,SendDate")] wlt_ServerLogs wlt_ServerLogs)
        {
            if (ModelState.IsValid)
            {
                _context.Add(wlt_ServerLogs);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(wlt_ServerLogs);
        }

        // GET: wlt_ServerLogs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var wlt_ServerLogs = await _context.wlt_ServerLogs.SingleOrDefaultAsync(m => m.ipkServerlogID == id);
            if (wlt_ServerLogs == null)
            {
                return NotFound();
            }
            return View(wlt_ServerLogs);
        }

        // POST: wlt_ServerLogs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ipkServerlogID,ServerID,ServerName,DISK,CPU,RAM,MSMQ,SendDate")] wlt_ServerLogs wlt_ServerLogs)
        {
            if (id != wlt_ServerLogs.ipkServerlogID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(wlt_ServerLogs);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!wlt_ServerLogsExists(wlt_ServerLogs.ipkServerlogID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(wlt_ServerLogs);
        }

        // GET: wlt_ServerLogs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var wlt_ServerLogs = await _context.wlt_ServerLogs
                .SingleOrDefaultAsync(m => m.ipkServerlogID == id);
            if (wlt_ServerLogs == null)
            {
                return NotFound();
            }

            return View(wlt_ServerLogs);
        }

        // POST: wlt_ServerLogs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var wlt_ServerLogs = await _context.wlt_ServerLogs.SingleOrDefaultAsync(m => m.ipkServerlogID == id);
            _context.wlt_ServerLogs.Remove(wlt_ServerLogs);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool wlt_ServerLogsExists(int id)
        {
            return _context.wlt_ServerLogs.Any(e => e.ipkServerlogID == id);
        }
    }
}
