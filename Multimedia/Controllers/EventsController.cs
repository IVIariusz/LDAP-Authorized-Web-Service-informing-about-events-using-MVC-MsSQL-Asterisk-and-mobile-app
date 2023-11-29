using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Multimedia.Models;
using System.Linq;
using System.Threading.Tasks;

public class EventsController : Controller
{
    private readonly SchoolEventsContext _context;

    private readonly ILogger<EventsController> _logger;

    public EventsController(SchoolEventsContext context, ILogger<EventsController> logger)
    {
        _context = context;
        _logger = logger;
    }


    // GET: Events
    public async Task<IActionResult> Index()
    {
        return View(await _context.Events.ToListAsync());
    }

    // GET: Events/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var @event = await _context.Events
            .FirstOrDefaultAsync(m => m.EventID == id);
        if (@event == null)
        {
            return NotFound();
        }

        return View(@event);
    }

    // GET: Events/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Events/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Title,Description,StartDateTime,EndDateTime,Location")] Event @event)
    {
        _logger.LogInformation("Create method called");
        if (ModelState.IsValid)
        {
            _logger.LogInformation("Model state is valid");
            _context.Add(@event);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Event created and saved");
            return RedirectToAction(nameof(Index));
        }
        _logger.LogWarning("Model state is invalid");
        return View(@event);
    }



    // GET: Events/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var @event = await _context.Events.FindAsync(id);
        if (@event == null)
        {
            return NotFound();
        }
        return View(@event);
    }

    // POST: Events/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("EventID,Title,Description,StartDateTime,EndDateTime,Location")] Event @event)
    {
        if (id != @event.EventID)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(@event);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EventExists(@event.EventID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        return View(@event);
    }

    // GET: Events/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var @event = await _context.Events
            .FirstOrDefaultAsync(m => m.EventID == id);
        if (@event == null)
        {
            return NotFound();
        }

        return View(@event);
    }

    // POST: Events/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var @event = await _context.Events.FindAsync(id);
        if (@event != null)
        {
            _context.Events.Remove(@event);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    private bool EventExists(int id)
    {
        return _context.Events.Any(e => e.EventID == id);
    }
}
