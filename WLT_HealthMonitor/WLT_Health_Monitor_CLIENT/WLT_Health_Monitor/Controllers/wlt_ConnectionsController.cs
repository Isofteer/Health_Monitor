using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WLT_HealthMonitor.Models;
using WLT_Health_Monitor.Data;
using Newtonsoft;
using Newtonsoft.Json;

using WLT_Health_Monitor.Models.ViewModel;

namespace WLT_Health_Monitor.Controllers
{
    public class wlt_ConnectionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public wlt_ConnectionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: wlt_Connections
        public async Task<IActionResult> Index()
        {


            return View(await _context.wlt_Connections.ToListAsync());
        }


        public  string getServerConnections()
        {

            var _NotificationViewModel = (from m in _context.wlt_Connections
                                               join r in _context.wlt_Configurations on m.conn_ID equals r.configuration_ID

                                               select new ConnetionViewModel()
                                               {
                                                   conn_ID = m.conn_ID,
                                                   RefreshRate = r.RefreshRate,
                                                   Host = m.Host,
                                                   Connection = m.Connection


                                               }).ToList();



            //_context.wlt_Connections.ToListAsync()
            var connections = JsonConvert.SerializeObject(_NotificationViewModel);

            return connections; 
        }

        // GET: wlt_Connections/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var wlt_Connections = await _context.wlt_Connections
                .SingleOrDefaultAsync(m => m.conn_ID == id);
            if (wlt_Connections == null)
            {
                return NotFound();
            }

            return View(wlt_Connections);
        }

        // GET: wlt_Connections/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: wlt_Connections/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("conn_ID,Connection,Host")] wlt_Connections wlt_Connections)
        {
            if (ModelState.IsValid)
            {
                _context.Add(wlt_Connections);
                await _context.SaveChangesAsync();
                _context.Add( new wlt_Configurations { ServerID = wlt_Connections.conn_ID, RefreshRate=2 });
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(wlt_Connections);
        }

        // GET: wlt_Connections/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var wlt_Connections = await _context.wlt_Connections.SingleOrDefaultAsync(m => m.conn_ID == id);
            if (wlt_Connections == null)
            {
                return NotFound();
            }
            return View(wlt_Connections);
        }

        // POST: wlt_Connections/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("conn_ID,Connection,Host")] wlt_Connections wlt_Connections)
        {
            if (id != wlt_Connections.conn_ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(wlt_Connections);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!wlt_ConnectionsExists(wlt_Connections.conn_ID))
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
            return View(wlt_Connections);
        }

        // GET: wlt_Connections/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var wlt_Connections = await _context.wlt_Connections
                .SingleOrDefaultAsync(m => m.conn_ID == id);
            if (wlt_Connections == null)
            {
                return NotFound();
            }

            return View(wlt_Connections);
        }

        // POST: wlt_Connections/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var wlt_Connections = await _context.wlt_Connections.SingleOrDefaultAsync(m => m.conn_ID == id);
            _context.wlt_Connections.Remove(wlt_Connections);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool wlt_ConnectionsExists(int id)
        {
            return _context.wlt_Connections.Any(e => e.conn_ID == id);
        }
    }
}
