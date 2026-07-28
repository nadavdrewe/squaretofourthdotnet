using domain.pipeline.fourth.com.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace web.pipeline.fourth.com.Controllers
{
    [Authorize]
    public class PipelineEventLogsController : Controller
    {
        private readonly FourthPipelineContext _context;

        public PipelineEventLogsController(FourthPipelineContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var records = await _context.PipelineEventLogs
                .OrderByDescending(x => x.WhenCreatedUTC)
                .Take(1000)
                .ToListAsync();

            return View(records);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var record = await _context.PipelineEventLogs
                .FirstOrDefaultAsync(x => x.Id == id);

            if (record == null)
            {
                return NotFound();
            }

            return View(record);
        }
    }
}
