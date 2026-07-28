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
    public class GlobalsController : Controller
    {
        private readonly FourthPipelineContext _context;

        public GlobalsController(FourthPipelineContext context)
        {
            _context = context;
        }

        // GET: Globals
        public async Task<IActionResult> Index()
        {
            return View(await _context.Globals.ToListAsync());
        }

        // GET: Globals/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @global = await _context.Globals
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@global == null)
            {
                return NotFound();
            }

            return View(@global);
        }

        // GET: Globals/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Globals/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id")] Global @global)
        {
            if (ModelState.IsValid)
            {
                _context.Add(@global);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(@global);
        }

        // GET: Globals/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @global = await _context.Globals.FindAsync(id);
            if (@global == null)
            {
                return NotFound();
            }
            return View(@global);
        }

        // POST: Globals/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id")] Global @global)
        {
            if (id != @global.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(@global);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GlobalExists(@global.Id))
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
            return View(@global);
        }

        // GET: Globals/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @global = await _context.Globals
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@global == null)
            {
                return NotFound();
            }

            return View(@global);
        }

        // POST: Globals/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var @global = await _context.Globals.FindAsync(id);
            _context.Globals.Remove(@global);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GlobalExists(int id)
        {
            return _context.Globals.Any(e => e.Id == id);
        }
    }
}
