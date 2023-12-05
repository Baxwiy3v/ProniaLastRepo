using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AB460Proniya.Areas.ProniaAdmin.Controllers
{
    [Area("admin")]


    public class HomeController : Controller
    {
        [Authorize(Roles = "Admin,Moderator")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
