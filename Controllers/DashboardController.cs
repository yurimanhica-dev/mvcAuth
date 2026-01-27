using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Autentication.Controllers
{
    public class DashboardController : Controller
    {
        [Authorize]
        public IActionResult Overview()
        {
            return View();
        }

        public IActionResult Projects()
        {
            return View();
        }

        public IActionResult Tasks()
        {
            return View();
        }
    }
}
