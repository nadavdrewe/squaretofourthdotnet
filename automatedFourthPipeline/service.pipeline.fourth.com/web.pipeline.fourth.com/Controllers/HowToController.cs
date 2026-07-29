using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace web.pipeline.fourth.com.Controllers
{
    [Authorize]
    public class HowToController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
