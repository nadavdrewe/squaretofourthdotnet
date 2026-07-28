using data.pipeline.fourth.com.Models.Mappings;
using domain.pipeline.fourth.com.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace web.pipeline.fourth.com.Controllers.Configs
{
    [Authorize]
    public class SquareEmployeeMappingsController : Controller
    {
        private readonly FourthPipelineContext _context;

        public SquareEmployeeMappingsController(FourthPipelineContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var mappings = _context.SquareEmployeeMappings
                .Include(x => x.StoreIntegration)
                .ThenInclude(x => x.Store)
                .ThenInclude(x => x.Brand)
                .OrderBy(x => x.StoreIntegration.Store.Brand.Name)
                .ThenBy(x => x.StoreIntegration.Store.Name)
                .ThenBy(x => x.SquareDisplayName)
                .ThenBy(x => x.SquareTeamMemberId);

            return View(await mappings.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mapping = await _context.SquareEmployeeMappings
                .Include(x => x.StoreIntegration)
                .ThenInclude(x => x.Store)
                .ThenInclude(x => x.Brand)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (mapping == null)
            {
                return NotFound();
            }

            return View(mapping);
        }

        public IActionResult Create(int? storeIntegrationId)
        {
            PopulateStoreIntegrationList(storeIntegrationId);
            return View(new SquareEmployeeMapping
            {
                StoreIntegrationId = storeIntegrationId ?? 0,
                Active = true
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("StoreIntegrationId,SquareTeamMemberId,SquareDisplayName,FourthEmployeeNumber,Active")] SquareEmployeeMapping mapping)
        {
            if (ModelState.IsValid)
            {
                mapping.WhenCreatedUTC = DateTime.UtcNow;
                mapping.WhenUpdatedUTC = mapping.WhenCreatedUTC;
                _context.Add(mapping);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            PopulateStoreIntegrationList(mapping.StoreIntegrationId);
            return View(mapping);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mapping = await _context.SquareEmployeeMappings.FindAsync(id);
            if (mapping == null)
            {
                return NotFound();
            }

            PopulateStoreIntegrationList(mapping.StoreIntegrationId);
            return View(mapping);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("Id,StoreIntegrationId,SquareTeamMemberId,SquareDisplayName,FourthEmployeeNumber,Active,WhenCreatedUTC")] SquareEmployeeMapping mapping)
        {
            if (id != mapping.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    mapping.WhenUpdatedUTC = DateTime.UtcNow;
                    _context.Update(mapping);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SquareEmployeeMappingExists(mapping.Id))
                    {
                        return NotFound();
                    }

                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            PopulateStoreIntegrationList(mapping.StoreIntegrationId);
            return View(mapping);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mapping = await _context.SquareEmployeeMappings
                .Include(x => x.StoreIntegration)
                .ThenInclude(x => x.Store)
                .ThenInclude(x => x.Brand)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (mapping == null)
            {
                return NotFound();
            }

            return View(mapping);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var mapping = await _context.SquareEmployeeMappings.FindAsync(id);
            if (mapping != null)
            {
                _context.SquareEmployeeMappings.Remove(mapping);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private void PopulateStoreIntegrationList(int? selectedId = null)
        {
            var storeIntegrations = _context.StoreIntegrations
                .Include(x => x.Store)
                .ThenInclude(x => x.Brand)
                .OrderBy(x => x.Store.Brand.Name)
                .ThenBy(x => x.Store.Name)
                .Select(x => new
                {
                    x.Id,
                    Label = $"{x.Store.Brand.Name} / {x.Store.Name} / Integration {x.Id}"
                })
                .ToList();

            ViewData["StoreIntegrationId"] = new SelectList(storeIntegrations, "Id", "Label", selectedId);
        }

        private bool SquareEmployeeMappingExists(int id)
        {
            return _context.SquareEmployeeMappings.Any(x => x.Id == id);
        }
    }
}
