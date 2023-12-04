using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Multimedia.Models;
using System;
using System.Linq;
using System.Threading.Tasks;  // Dodaj using System.Threading.Tasks;

namespace Multimedia.Controllers
{
    [Authorize]
    public class PreferencesController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SchoolEventsContext _context;

        public PreferencesController(UserManager<ApplicationUser> userManager, SchoolEventsContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> ManagePreferences(string userId)
        {
            // Pobierz użytkownika na podstawie UserId z bazy danych
            var user = await _userManager.FindByIdAsync(userId);

            var existingPreferences = _context.UserPreferences.FirstOrDefault(up => up.UserID == userId);

            if (existingPreferences != null)
            {
                return View(existingPreferences);
            }
            else
            {
                var newPreferences = new UserPreferences
                {
                    UserID = userId
                };

                return View(newPreferences);
            }
        }
        [HttpPost]
        public IActionResult ManagePreferences(UserPreferences preferences)
        {
            if (ModelState.IsValid)
            {
                preferences.UserID = _userManager.GetUserId(User);

                var existingPreferences = _context.UserPreferences.FirstOrDefault(up => up.UserID == preferences.UserID);

                if (existingPreferences != null)
                {
                    // ... reszta kodu ...

                    existingPreferences.UserID = preferences.UserID; // Dodaj tę linijkę
                }
                else
                {
                    _context.UserPreferences.Add(preferences);
                }

                _context.SaveChanges();

                return RedirectToAction("Index", "Home");
            }

            return View(preferences);
        }
    }
}
