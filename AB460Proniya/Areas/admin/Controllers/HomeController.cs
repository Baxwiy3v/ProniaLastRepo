using Microsoft.AspNetCore.Mvc;

namespace AB460Proniya.Areas.ProniaAdmin.Controllers
{
    [Area("admin")]


    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
