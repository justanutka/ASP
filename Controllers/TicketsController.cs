using Microsoft.AspNetCore.Mvc;

namespace UniDesk.Web.Controllers
{
    public class TicketsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
