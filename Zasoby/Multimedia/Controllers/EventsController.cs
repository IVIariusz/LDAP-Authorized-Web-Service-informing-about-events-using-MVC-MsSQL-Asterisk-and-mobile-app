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



    public async Task<IActionResult> Index()
    {
        var eventsQuery = _context.Events.AsQueryable();

        if (!User.IsInRole("admin"))
        {
            // Pobierz ID użytkownika z ciasteczka
            var userIdClaim = HttpContext.User.FindFirst("UserID");
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out var userId))
            {
                // Pobierz ID wydarzeń przypisanych do użytkownika
                var userEventIds = await _context.EventUser
                    .Where(eu => eu.UserID == userId)
                    .Select(eu => eu.EventID)
                    .ToListAsync();

                // Ogranicz zapytanie do wydarzeń, do których użytkownik jest przypisany
                eventsQuery = eventsQuery.Where(e => userEventIds.Contains(e.EventID));
            }
        }

        var events = await eventsQuery.ToListAsync();
        return View(events);
    }



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


    public async Task<IActionResult> Create()
    {
        ViewBag.Users = await _context.Users.ToListAsync(); // Populate users for dropdown
        return View();
    }

    // POST: Event/Create
    // POST: Event/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Event @event, int[] selectedUserIds)
    {
        if (ModelState.IsValid)
        {
            _context.Add(@event);
            await _context.SaveChangesAsync();

            if (@event.IsForAllUsers)
            {
                // Assign the event to all users
                var allUsers = await _context.Users.ToListAsync();
                var allUserAssignments = allUsers.Select(user => new EventUser
                {
                    UserID = user.UserID,
                    EventID = @event.EventID
                });
                _context.AddRange(allUserAssignments);
            }
            else if (selectedUserIds != null && selectedUserIds.Any())
            {
                // Assign the event to selected users
                var selectedUserAssignments = selectedUserIds.Select(userId => new EventUser
                {
                    UserID = userId,
                    EventID = @event.EventID
                });
                _context.AddRange(selectedUserAssignments);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        ViewBag.Users = await _context.Users.ToListAsync();
        return View(@event);
    }


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

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        using (var transaction = _context.Database.BeginTransaction())
        {
            try
            {
                var @event = await _context.Events.FindAsync(id);
                if (@event == null)
                {
                    _logger.LogInformation("Event not found");
                    return RedirectToAction(nameof(Index));
                }

                _context.Entry(@event).OriginalValues.SetValues(await _context.Events.AsNoTracking().FirstOrDefaultAsync(e => e.EventID == id));

                var relatedMessages = _context.SentMessagesHistory.Where(m => m.EventID == id);

                _context.SentMessagesHistory.RemoveRange(relatedMessages);

                // Oznacz zdarzenie jako usunięte
                @event.IsDeleted = true;

                _logger.LogInformation("Marking event as deleted");


                await _context.SaveChangesAsync();

                _logger.LogInformation("Event marked as deleted successfully");

                transaction.Commit();

                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException ex)
            {
                transaction.Rollback();
                _logger.LogError(ex, "Concurrency error during delete operation");
                return RedirectToAction("ConcurrencyError");
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                _logger.LogError(ex, "Error during delete operation");
                throw;
            }
        }
    }


    private bool EventExists(int id)
    {
        return _context.Events.Any(e => e.EventID == id);
    }
}
