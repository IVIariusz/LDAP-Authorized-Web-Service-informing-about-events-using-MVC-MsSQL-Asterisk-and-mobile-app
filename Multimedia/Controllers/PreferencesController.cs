using Microsoft.AspNetCore.Mvc;
using Multimedia.Models;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;

public class PreferencesController : Controller
{
    private readonly SchoolEventsContext _context;
    private readonly ILogger<PreferencesController> _logger;

    public PreferencesController(SchoolEventsContext context, ILogger<PreferencesController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // GET: Preferences
    public IActionResult Index()
    {
        int userId = GetLoggedInUserId();
        if (userId == 0)
        {
            _logger.LogWarning("No logged in user found for preferences.");
            return Unauthorized();
        }

        var userPreferences = GetUserPreferences(userId);
        return View(userPreferences);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SavePreferences(UserPreferences model)
    {
        int userId = GetLoggedInUserId();
        if (userId == 0)
        {
            _logger.LogWarning("Attempted to save preferences for an invalid user.");
            return Unauthorized();
        }

        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid model state for saving preferences.");
            return View("Index", model);
        }

        var userPreferences = GetUserPreferences(userId);
        UpdateUserPreferences(userPreferences, model);

        try
        {
            _context.Update(userPreferences);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Preferences for user {userId} were successfully saved.");
            TempData["Saved"] = true;
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error saving preferences for user {userId}: {ex.Message}");
            ModelState.AddModelError(string.Empty, "An error occurred saving preferences.");
            return View("Index", model);
        }
    }

    private int GetLoggedInUserId()
    {
        var userIdClaim = User.FindFirst("UserID");
        return userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId) ? userId : 0;
    }

    private UserPreferences GetUserPreferences(int userId)
    {
        return _context.UserPreferences
                       .FirstOrDefault(u => u.UserID == userId)
                       ?? new UserPreferences { UserID = userId };
    }

    private void UpdateUserPreferences(UserPreferences existing, UserPreferences updated)
    {
        existing.ReceiveMessages = updated.ReceiveMessages;
        existing.PreferredDeliveryTime = updated.PreferredDeliveryTime;
        existing.DeliveryMethod = updated.DeliveryMethod;
        existing.BlockedHoursStart = updated.BlockedHoursStart;
        existing.BlockedHoursEnd = updated.BlockedHoursEnd;
    }
}
