using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using data.pipeline.fourth.com.Models.Configs.Store;
using domain.pipeline.fourth.com.Models;

namespace web.pipeline.fourth.com.Controllers.Configs
{
    public class LightspeedRestoStoreConfigsController : Controller
    {
        private readonly FourthPipelineContext _context;

        public LightspeedRestoStoreConfigsController(FourthPipelineContext context)
        {
            _context = context;
        }

        // GET: LightspeedRestoStoreConfigs
        public async Task<IActionResult> Index()
        {
            var fourthPipelineContext = _context.LightspeedRestoStoreConfig.Include(l => l.StoreIntegration);
            return View(await fourthPipelineContext.ToListAsync());
        }

        // GET: LightspeedRestoStoreConfigs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lightspeedRestoStoreConfig = await _context.LightspeedRestoStoreConfig
                .Include(l => l.StoreIntegration)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (lightspeedRestoStoreConfig == null)
            {
                return NotFound();
            }

            return View(lightspeedRestoStoreConfig);
        }

        // GET: LightspeedRestoStoreConfigs/Create
        public IActionResult Create()
        {
            ViewData["StoreIntegrationId"] = new SelectList(_context.StoreIntegrations, "Id", "Id");
            return View();
        }

        // POST: LightspeedRestoStoreConfigs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CompanyId,StoreIntegrationId,Active,WhenCreatedUTC,WhenUpdatedUTC")] LightspeedRestoStoreConfig lightspeedRestoStoreConfig)
        {
            if (ModelState.IsValid)
            {
                _context.Add(lightspeedRestoStoreConfig);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["StoreIntegrationId"] = new SelectList(_context.StoreIntegrations, "Id", "Id", lightspeedRestoStoreConfig.StoreIntegrationId);
            return View(lightspeedRestoStoreConfig);
        }

        // GET: LightspeedRestoStoreConfigs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lightspeedRestoStoreConfig = await _context.LightspeedRestoStoreConfig.FindAsync(id);
            if (lightspeedRestoStoreConfig == null)
            {
                return NotFound();
            }
            ViewData["StoreIntegrationId"] = new SelectList(_context.StoreIntegrations, "Id", "Id", lightspeedRestoStoreConfig.StoreIntegrationId);
            return View(lightspeedRestoStoreConfig);
        }

        // POST: LightspeedRestoStoreConfigs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CompanyId,StoreIntegrationId,Active,WhenCreatedUTC,WhenUpdatedUTC")] LightspeedRestoStoreConfig lightspeedRestoStoreConfig)
        {
            if (id != lightspeedRestoStoreConfig.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(lightspeedRestoStoreConfig);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LightspeedRestoStoreConfigExists(lightspeedRestoStoreConfig.Id))
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
            ViewData["StoreIntegrationId"] = new SelectList(_context.StoreIntegrations, "Id", "Id", lightspeedRestoStoreConfig.StoreIntegrationId);
            return View(lightspeedRestoStoreConfig);
        }

        // GET: LightspeedRestoStoreConfigs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lightspeedRestoStoreConfig = await _context.LightspeedRestoStoreConfig
                .Include(l => l.StoreIntegration)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (lightspeedRestoStoreConfig == null)
            {
                return NotFound();
            }

            return View(lightspeedRestoStoreConfig);
        }

        // POST: LightspeedRestoStoreConfigs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var lightspeedRestoStoreConfig = await _context.LightspeedRestoStoreConfig.FindAsync(id);
            _context.LightspeedRestoStoreConfig.Remove(lightspeedRestoStoreConfig);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LightspeedRestoStoreConfigExists(int id)
        {
            return _context.LightspeedRestoStoreConfig.Any(e => e.Id == id);
        }
    }
}
