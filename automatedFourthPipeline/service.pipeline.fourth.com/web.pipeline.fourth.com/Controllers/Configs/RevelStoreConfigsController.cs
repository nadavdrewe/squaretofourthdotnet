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
    public class RevelStoreConfigsController : Controller
    {
        private readonly FourthPipelineContext _context;

        public RevelStoreConfigsController(FourthPipelineContext context)
        {
            _context = context;
        }

        // GET: RevelStoreConfigs
        public async Task<IActionResult> Index()
        {
            var fourthPipelineContext = _context.RevelStoreConfigs.Include(r => r.StoreIntegration);
            return View(await fourthPipelineContext.ToListAsync());
        }

        // GET: RevelStoreConfigs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var revelStoreConfig = await _context.RevelStoreConfigs
                .Include(r => r.StoreIntegration)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (revelStoreConfig == null)
            {
                return NotFound();
            }

            return View(revelStoreConfig);
        }

        // GET: RevelStoreConfigs/Create
        public IActionResult Create()
        {
            ViewData["StoreIntegrationId"] = new SelectList(_context.StoreIntegrations, "Id", "Id");
            return View();
        }

        // POST: RevelStoreConfigs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,EstablishmentID,EstablishmentResourceUri,StoreIntegrationId,Active,WhenCreatedUTC,WhenUpdatedUTC")] RevelStoreConfig revelStoreConfig)
        {
            if (ModelState.IsValid)
            {
                _context.Add(revelStoreConfig);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["StoreIntegrationId"] = new SelectList(_context.StoreIntegrations, "Id", "Id", revelStoreConfig.StoreIntegrationId);
            return View(revelStoreConfig);
        }

        // GET: RevelStoreConfigs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var revelStoreConfig = await _context.RevelStoreConfigs.FindAsync(id);
            if (revelStoreConfig == null)
            {
                return NotFound();
            }
            ViewData["StoreIntegrationId"] = new SelectList(_context.StoreIntegrations, "Id", "Id", revelStoreConfig.StoreIntegrationId);
            return View(revelStoreConfig);
        }

        // POST: RevelStoreConfigs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,EstablishmentID,EstablishmentResourceUri,StoreIntegrationId,Active,WhenCreatedUTC,WhenUpdatedUTC")] RevelStoreConfig revelStoreConfig)
        {
            if (id != revelStoreConfig.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(revelStoreConfig);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RevelStoreConfigExists(revelStoreConfig.Id))
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
            ViewData["StoreIntegrationId"] = new SelectList(_context.StoreIntegrations, "Id", "Id", revelStoreConfig.StoreIntegrationId);
            return View(revelStoreConfig);
        }

        // GET: RevelStoreConfigs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var revelStoreConfig = await _context.RevelStoreConfigs
                .Include(r => r.StoreIntegration)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (revelStoreConfig == null)
            {
                return NotFound();
            }

            return View(revelStoreConfig);
        }

        // POST: RevelStoreConfigs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var revelStoreConfig = await _context.RevelStoreConfigs.FindAsync(id);
            _context.RevelStoreConfigs.Remove(revelStoreConfig);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RevelStoreConfigExists(int id)
        {
            return _context.RevelStoreConfigs.Any(e => e.Id == id);
        }
    }
}
