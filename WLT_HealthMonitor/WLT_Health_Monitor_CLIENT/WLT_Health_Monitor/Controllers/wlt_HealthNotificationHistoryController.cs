using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WLT_HealthMonitor.Models;
using WLT_Health_Monitor.Models;
using WLT_Health_Monitor.Data;

using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace WLT_Health_Monitor.Controllers
{
    public class wlt_HealthNotificationHistoryController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public wlt_HealthNotificationHistoryController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;

        }

        // GET: wlt_HealthNotificationHistory
        public async Task<IActionResult> Index()
        {
           
            var _NotificationViewModel = await  (from m in _context.wlt_HealthNotificationHistory
                              join r in _context.wlt_Connections on m.ifkServerID equals r.conn_ID
                               where m.ifkUserID == User.FindFirstValue(ClaimTypes.NameIdentifier)
                               select new NotificationViewModel()
                              {
                                  Server = r.Host,
                                  Resource = m.Resource,
                                  ResourceValue = m.ResourceValue,
                                  MailAddress = _context.Users.Where(n => n.Id ==m.ifkUserID).SingleOrDefault().Email,
                                  SentDate = m.DateSent,
                                  isSent = m.isSent
                                  

                              }).OrderByDescending(n =>n.SentDate ).ToListAsync();




            //return View(await _context.wlt_HealthNotificationHistory.OrderByDescending(n=>n.DateSent).ToListAsync());

            return View("NotificationHistory", _NotificationViewModel);

           /// return View( _NotificationViewModel);
                 
        }
        private Tuple<string,string> SpitOutUser(string id)
        {
            var _Users = _context.Users.Where(n => n.Id == id).SingleOrDefault();


            return new Tuple<string, string>("","");
        }

        // GET: wlt_HealthNotificationHistory/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var wlt_HealthNotificationHistory = await _context.wlt_HealthNotificationHistory
                .SingleOrDefaultAsync(m => m.ipkHistoryID == id);
            if (wlt_HealthNotificationHistory == null)
            {
                return NotFound();
            }

            return View(wlt_HealthNotificationHistory);
        }

        // GET: wlt_HealthNotificationHistory/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: wlt_HealthNotificationHistory/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ipkHistoryID,DateSent,ifkUserID,ifkServerID,isSent,Resource")] wlt_HealthNotificationHistory wlt_HealthNotificationHistory)
        {
            if (ModelState.IsValid)
            {
                _context.Add(wlt_HealthNotificationHistory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(wlt_HealthNotificationHistory);
        }

        // GET: wlt_HealthNotificationHistory/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var wlt_HealthNotificationHistory = await _context.wlt_HealthNotificationHistory.SingleOrDefaultAsync(m => m.ipkHistoryID == id);
            if (wlt_HealthNotificationHistory == null)
            {
                return NotFound();
            }
            return View(wlt_HealthNotificationHistory);
        }

        // POST: wlt_HealthNotificationHistory/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ipkHistoryID,DateSent,ifkUserID,ifkServerID,isSent,Resource")] wlt_HealthNotificationHistory wlt_HealthNotificationHistory)
        {
            if (id != wlt_HealthNotificationHistory.ipkHistoryID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(wlt_HealthNotificationHistory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!wlt_HealthNotificationHistoryExists(wlt_HealthNotificationHistory.ipkHistoryID))
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
            return View(wlt_HealthNotificationHistory);
        }

        // GET: wlt_HealthNotificationHistory/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var wlt_HealthNotificationHistory = await _context.wlt_HealthNotificationHistory
                .SingleOrDefaultAsync(m => m.ipkHistoryID == id);
            if (wlt_HealthNotificationHistory == null)
            {
                return NotFound();
            }

            return View(wlt_HealthNotificationHistory);
        }

        // POST: wlt_HealthNotificationHistory/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var wlt_HealthNotificationHistory = await _context.wlt_HealthNotificationHistory.SingleOrDefaultAsync(m => m.ipkHistoryID == id);
            _context.wlt_HealthNotificationHistory.Remove(wlt_HealthNotificationHistory);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool wlt_HealthNotificationHistoryExists(int id)
        {
            return _context.wlt_HealthNotificationHistory.Any(e => e.ipkHistoryID == id);
        }
    }
}
