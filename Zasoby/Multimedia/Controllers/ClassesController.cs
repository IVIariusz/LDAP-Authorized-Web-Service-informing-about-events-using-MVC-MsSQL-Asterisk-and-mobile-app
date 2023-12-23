using Microsoft.AspNetCore.Mvc;

namespace Multimedia.Controllers
{
    public class ClassesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
