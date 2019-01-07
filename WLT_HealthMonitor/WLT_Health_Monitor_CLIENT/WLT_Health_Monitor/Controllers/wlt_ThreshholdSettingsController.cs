using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WLT_HealthMonitor.Models;
using WLT_Health_Monitor.Data;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using WLT_Health_Monitor.Models.ViewModel;

namespace WLT_Health_Monitor.Controllers
{
    public class wlt_ThreshholdSettingsController : Controller
    {
        private readonly ApplicationDbContext _context;
       

        public wlt_ThreshholdSettingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: wlt_ThreshholdSettings
        public async Task<IActionResult> Index()
        {


            var threshholdsettings = (from m in _context.wlt_ThreshholdSettings
                                     join d in _context.wlt_Connections on m.ifkServerID equals d.conn_ID
                                     where m.ifkUserID == User.FindFirstValue(ClaimTypes.NameIdentifier)
                                     select  new ThreshholdViewModel()
                                     {
                                         ResouceValue = m.ResouceValue,
                                         Resource = m.Resource,
                                         ifkUserID = m.ifkUserID,
                                         ifkServerID = m.ifkServerID,
                                         Host = d.Host,
                                         Connection = d.Connection,
                                         ipkThreshholdID =m.ipkThreshholdID

                                     }).ToList();

           // await _context.wlt_ThreshholdSettings.Where(v => v.ifkUserID == User.FindFirstValue(ClaimTypes.NameIdentifier)).ToListAsync()


            return View(threshholdsettings);
        }


        public async Task<IActionResult> FrequencySettings()
        {
            ViewData["selectionlist"] = SelectListItems();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetFrequecy()
        {

            var serverID = Convert.ToInt32(HttpContext.Request.Form["Server"]);
            var ReshfreshRate = Convert.ToInt32(HttpContext.Request.Form["settingValue"]);
          


            var currentSettings = _context.wlt_Configurations.Where(n => n.ServerID == serverID).FirstOrDefault();


            if (currentSettings == null)
            {
                var setData = new wlt_Configurations();

                setData.ServerID = serverID;

                setData.RefreshRate = ReshfreshRate;            

                _context.Add(setData);
            }
            else
            {
                currentSettings.RefreshRate = ReshfreshRate;
                _context.Update(currentSettings);



            }
          

            await _context.SaveChangesAsync();
            ViewData["selectionlist"] = SelectListItems();
            return RedirectToAction("index", "wlt_ThreshholdSettings");
        }


        // GET: wlt_ThreshholdSettings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var wlt_ThreshholdSettings = await _context.wlt_ThreshholdSettings
                .SingleOrDefaultAsync(m => m.ipkThreshholdID == id);
            if (wlt_ThreshholdSettings == null)
            {
                return NotFound();
            }

            return View(wlt_ThreshholdSettings);
        }

        // GET: wlt_ThreshholdSettings/Create

        public List<SelectListItem> SelectListItems() {

            var  _list = new List<SelectListItem>();
            var _thresholdList = _context.wlt_Connections
                        .Distinct()
                        .OrderByDescending(i => i.Host)
                        .ToList();

            foreach (var item in _thresholdList)
            {
                _list.Add(new SelectListItem { Value = item.conn_ID.ToString(), Text = item.Host });
               

            }

            return _list;
        }
        public List<wlt_Connections> ServerList() {

            var _thresholdList = _context.wlt_Connections                        
                         .Distinct()
                         .OrderByDescending(i => i.Host)
                         .ToList();


            ViewBag._thresholdList = _thresholdList;


            return _thresholdList;
        }

        public IActionResult Create()
        {



            ViewData["selectionlist"] = SelectListItems();
            return View();
        }

        // POST: wlt_ThreshholdSettings/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ipkThreshholdID,Resource,ifkUserID,ifkServerID,ResouceValue")] wlt_ThreshholdSettings wlt_ThreshholdSettings)
        {
            if (ModelState.IsValid)
            {

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                wlt_ThreshholdSettings.ifkUserID = userId;
                _context.Add(wlt_ThreshholdSettings);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(wlt_ThreshholdSettings);
        }

        // GET: wlt_ThreshholdSettings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var wlt_ThreshholdSettings = await _context.wlt_ThreshholdSettings.SingleOrDefaultAsync(m => m.ipkThreshholdID == id);

            if (wlt_ThreshholdSettings == null)
            {
                return NotFound();
            }
            ViewData["selectionlist"] = SelectListItems();
            return View(wlt_ThreshholdSettings);
        }

        // POST: wlt_ThreshholdSettings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ipkThreshholdID,Resource,ifkUserID,ifkServerID,ResouceValue")] wlt_ThreshholdSettings wlt_ThreshholdSettings)
        {
            if (id != wlt_ThreshholdSettings.ipkThreshholdID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    wlt_ThreshholdSettings.ifkUserID = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    _context.Update(wlt_ThreshholdSettings);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!wlt_ThreshholdSettingsExists(wlt_ThreshholdSettings.ipkThreshholdID))
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
            return View(wlt_ThreshholdSettings);
        }

        // GET: wlt_ThreshholdSettings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var wlt_ThreshholdSettings = await _context.wlt_ThreshholdSettings
                .SingleOrDefaultAsync(m => m.ipkThreshholdID == id);
            if (wlt_ThreshholdSettings == null)
            {
                return NotFound();
            }

            return View(wlt_ThreshholdSettings);
        }

        // POST: wlt_ThreshholdSettings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var wlt_ThreshholdSettings = await _context.wlt_ThreshholdSettings.SingleOrDefaultAsync(m => m.ipkThreshholdID == id);
            _context.wlt_ThreshholdSettings.Remove(wlt_ThreshholdSettings);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool wlt_ThreshholdSettingsExists(int id)
        {
            return _context.wlt_ThreshholdSettings.Any(e => e.ipkThreshholdID == id);
        }
    }
}
