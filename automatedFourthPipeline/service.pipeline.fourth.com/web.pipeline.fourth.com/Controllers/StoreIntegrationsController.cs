
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using data.pipeline.fourth.com.Models;
using domain.pipeline.fourth.com.Models;
using Microsoft.AspNetCore.Authorization;

namespace web.pipeline.fourth.com.Controllers
{
    [Authorize]
    public class StoreIntegrationsController : Controller
    {
        private readonly FourthPipelineContext _context;

        public StoreIntegrationsController(FourthPipelineContext context)
        {
            _context = context;
        }

        // GET: StoreIntegrations
        public async Task<IActionResult> Index()
        {
            var fourthPipelineContext = _context
                .StoreIntegrations
                .Include(s => s.Store).Include(x => x.Store.Brand);
            return View(await fourthPipelineContext.ToListAsync());
        }

        // GET: StoreIntegrations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var storeIntegration = await _context.StoreIntegrations
                .Include(s => s.Store)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (storeIntegration == null)
            {
                return NotFound();
            }

            return View(storeIntegration);
        }

        // GET: StoreIntegrations/Create
        public IActionResult Create()
        {
            ViewData["StoreId"] = new SelectList(_context.Stores, "Id", "Name");
            return View();
        }

        // POST: StoreIntegrations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,StartBatchTimeUTC,EndBatchTimeUTC,FireTimeUTC,Active,IntegrationType,IntegrationSubType,WhenCreatedUTC,WhenUpdatedUTC,StoreId")] StoreIntegration storeIntegration)
        {
            if (ModelState.IsValid)
            {
                _context.Add(storeIntegration);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["StoreId"] = new SelectList(_context.Stores, "Id", "Name", storeIntegration.StoreId);
            return View(storeIntegration);
        }

        // GET: StoreIntegrations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var storeIntegration = _context
                .StoreIntegrations
                .Include(x=>x.Store)
                .Include(X => X.FourthSalesApiStoreConfigs)
                .Include(X => X.LightspeedRestoStoreConfig)
                .Include(X => X.RevelStoreConfigs)
                .Include(X => X.SquareStoreConfigs)
                .Include(X => X.SquareEmployeeMappings)
                .FirstOrDefault(x => x.Id == id);
            if (storeIntegration == null)
            {
                return NotFound();
            }
            ViewData["StoreId"] = new SelectList(_context.Stores, "Id", "Name", storeIntegration.StoreId);
            return View(storeIntegration);
        }

        // POST: StoreIntegrations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,StartBatchTimeUTC,EndBatchTimeUTC,FireTimeUTC,Active,IntegrationType,IntegrationSubType,WhenCreatedUTC,WhenUpdatedUTC,StoreId")] StoreIntegration storeIntegration)
        {
            if (id != storeIntegration.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(storeIntegration);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StoreIntegrationExists(storeIntegration.Id))
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
            ViewData["StoreId"] = new SelectList(_context.Stores, "Id", "Name", storeIntegration.StoreId);
            return View(storeIntegration);
        }

        // GET: StoreIntegrations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var storeIntegration = await _context.StoreIntegrations
                .Include(s => s.Store)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (storeIntegration == null)
            {
                return NotFound();
            }

            return View(storeIntegration);
        }

        // POST: StoreIntegrations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var storeIntegration = await _context.StoreIntegrations.FindAsync(id);
            _context.StoreIntegrations.Remove(storeIntegration);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StoreIntegrationExists(int id)
        {
            return _context.StoreIntegrations.Any(e => e.Id == id);
        }
    }
}
