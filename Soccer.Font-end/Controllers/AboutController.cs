using Microsoft.AspNetCore.Mvc;

namespace Soccer.Font_end.Controllers
{
    public class AboutController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
