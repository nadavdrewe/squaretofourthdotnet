using System.Threading.Tasks;
using domain.pipeline.fourth.com.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace web.pipeline.fourth.com.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("health")]
    public class HealthController : ControllerBase
    {
        private readonly FourthPipelineContext _context;

        public HealthController(FourthPipelineContext context)
        {
            _context = context;
        }

        [HttpGet("live")]
        public IActionResult Live()
        {
            return Ok(new { status = "live" });
        }

        [HttpGet("ready")]
        public async Task<IActionResult> Ready()
        {
            try
            {
                if (!await _context.Database.CanConnectAsync())
                {
                    return StatusCode(503, new { status = "not-ready" });
                }

                return Ok(new { status = "ready" });
            }
            catch
            {
                return StatusCode(503, new { status = "not-ready" });
            }
        }
    }
}