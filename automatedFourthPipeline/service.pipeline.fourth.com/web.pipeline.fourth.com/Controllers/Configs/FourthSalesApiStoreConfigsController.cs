using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using data.pipeline.fourth.com.Models.Configs.Store;
using domain.pipeline.fourth.com.Models;
using Microsoft.AspNetCore.Authorization;

namespace web.pipeline.fourth.com.Controllers.Configs
{

    [Authorize]
    public class FourthSalesApiStoreConfigsController : Controller
    {
        private readonly FourthPipelineContext _context;

        public FourthSalesApiStoreConfigsController(FourthPipelineContext context)
        {
            _context = context;
        }

        // GET: FourthSalesApiStoreConfigs
        public async Task<IActionResult> Index()
        {
            var fourthPipelineContext = _context.FourthSalesApiStoreConfig.Include(f => f.StoreIntegration);
            return View(await fourthPipelineContext.ToListAsync());
        }

        // GET: FourthSalesApiStoreConfigs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fourthSalesApiStoreConfig = await _context.FourthSalesApiStoreConfig
                .Include(f => f.StoreIntegration)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (fourthSalesApiStoreConfig == null)
            {
                return NotFound();
            }

            return View(fourthSalesApiStoreConfig);
        }

        // GET: FourthSalesApiStoreConfigs/Create
        public IActionResult Create()
        {
            ViewData["StoreIntegrationId"] = new SelectList(_context.StoreIntegrations, "Id", "Id");
            return View();
        }

        // POST: FourthSalesApiStoreConfigs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UnitId,SiteLocationCode,RevenueCenter,RevenueCenterMappingType,StoreIntegrationId,Active,WhenCreatedUTC,WhenUpdatedUTC")] FourthSalesApiStoreConfig fourthSalesApiStoreConfig)
        {
            if (ModelState.IsValid)
            {
                _context.Add(fourthSalesApiStoreConfig);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["StoreIntegrationId"] = new SelectList(_context.StoreIntegrations, "Id", "Id", fourthSalesApiStoreConfig.StoreIntegrationId);
            return View(fourthSalesApiStoreConfig);
        }

        // GET: FourthSalesApiStoreConfigs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fourthSalesApiStoreConfig = _context
                .FourthSalesApiStoreConfig
                .Include(x => x.StoreIntegration.Store.Brand)
                .FirstOrDefault(x => x.Id == id);
            if (fourthSalesApiStoreConfig == null)
            {
                return NotFound();
            }
            ViewData["StoreIntegrationId"] = new SelectList(_context.StoreIntegrations, "Id", "Id", fourthSalesApiStoreConfig.StoreIntegrationId);
            return View(fourthSalesApiStoreConfig);
        }

        // POST: FourthSalesApiStoreConfigs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UnitId,SiteLocationCode,RevenueCenter,RevenueCenterMappingType,StoreIntegrationId,Active,WhenCreatedUTC,WhenUpdatedUTC")] FourthSalesApiStoreConfig fourthSalesApiStoreConfig)
        {
            if (id != fourthSalesApiStoreConfig.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(fourthSalesApiStoreConfig);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FourthSalesApiStoreConfigExists(fourthSalesApiStoreConfig.Id))
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
            ViewData["StoreIntegrationId"] = new SelectList(_context.StoreIntegrations, "Id", "Id", fourthSalesApiStoreConfig.StoreIntegrationId);
            return View(fourthSalesApiStoreConfig);
        }

        // GET: FourthSalesApiStoreConfigs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fourthSalesApiStoreConfig = await _context.FourthSalesApiStoreConfig
                .Include(f => f.StoreIntegration)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (fourthSalesApiStoreConfig == null)
            {
                return NotFound();
            }

            return View(fourthSalesApiStoreConfig);
        }

        // POST: FourthSalesApiStoreConfigs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var fourthSalesApiStoreConfig = await _context.FourthSalesApiStoreConfig.FindAsync(id);
            _context.FourthSalesApiStoreConfig.Remove(fourthSalesApiStoreConfig);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FourthSalesApiStoreConfigExists(int id)
        {
            return _context.FourthSalesApiStoreConfig.Any(e => e.Id == id);
        }
    }
}
