using Microsoft.AspNetCore.Mvc;
using Soccer.Font_end.ViewModels;

namespace Soccer.Font_end.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Profile()
        {
            return View(new UserProfileViewModel());
        }
    }
}