using Microsoft.AspNetCore.Mvc;
using Multimedia.Models;
using System.Linq;

namespace Multimedia.Controllers
{
    public class EventsHomeController : Controller
    {
        private readonly SchoolEventsContext _context;

        public EventsHomeController(SchoolEventsContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var eventsHome = _context.EventsHome.ToList();
            return View(eventsHome);
        }
    }
}