using Microsoft.AspNetCore.Mvc;

namespace LolTeamTracker.Web.Controllers
{
    public class ComponentController : Controller
    {
        public IActionResult Component()
        {
            return View();
        }
        public IActionResult Simple()
        {
            return View();
        }
        public IActionResult Sim()
        {
            return View();
        }
    }
}
