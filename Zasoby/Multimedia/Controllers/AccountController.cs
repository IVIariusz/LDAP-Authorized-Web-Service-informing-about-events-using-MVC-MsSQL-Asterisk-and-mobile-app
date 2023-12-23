using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Multimedia.Models;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.Options;


namespace Multimedia.Controllers
{
    public class AccountController : Controller
    {
        private readonly SchoolEventsContext _eventsContext;
        private readonly LdapService _ldapService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(
            SchoolEventsContext eventsContext,
            ILogger<AccountController> logger,
            IOptions<LdapConfig> ldapConfig,
            ILoggerFactory loggerFactory) // Add this parameter
        {
            _eventsContext = eventsContext;
            _logger = logger;

            // Use the ILoggerFactory to create an ILogger for LdapService
            var ldapLogger = loggerFactory.CreateLogger<LdapService>();

            _ldapService = new LdapService(ldapConfig.Value, ldapLogger);
        }



        public ActionResult Index()
        {
            return View();
        }
        public IActionResult Login()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _eventsContext.Users
                    .Include(u => u.Role)
                    .FirstOrDefault(u => u.Email == model.Email && u.PasswordHash == model.Password);

                if (user != null)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.FirstName + " " + user.LastName),
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim(ClaimTypes.Role, user.Role.RoleName),
                        new Claim("UserID", user.UserID.ToString())
                    };

                    var identity = new ClaimsIdentity(claims, "CookieAuthentication");
                    var principal = new ClaimsPrincipal(identity);

                    await HttpContext.SignInAsync("CookieAuthentication", principal);

                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError(string.Empty, "Nieudane logowanie. Spróbuj ponownie.");
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                _logger.LogInformation("Attempting to register a new user.");

                       var existingUserLocal = _eventsContext.Users.FirstOrDefault(u => u.Email == model.Email);

            if (existingUserLocal != null)
            {
                _logger.LogWarning($"Registration attempt failed: User {model.Email} already exists in local database.");
                ModelState.AddModelError(string.Empty, "Użytkownik o podanym adresie email już istnieje.");
                return View(model);
            }


                var existingUser = _eventsContext.Users.FirstOrDefault(u => u.Email == model.Email);


                if (existingUser == null)
                {
                    _logger.LogInformation($"User {model.Email} not found in local database. Checking LDAP.");

                    var ldapRole = _ldapService.GetUserRole(model.Email, model.Password);

                    int roleID;
                    if (ldapRole == "admin")
                    {
                        roleID = _eventsContext.Roles.FirstOrDefault(r => r.RoleName == "admin")?.RoleID ?? default;
                        _logger.LogInformation($"LDAP identified {model.Email} as an admin.");
                    }
                    else
                    {
                        roleID = _eventsContext.Roles.FirstOrDefault(r => r.RoleName == "User")?.RoleID ?? default;
                        _logger.LogInformation($"LDAP identified {model.Email} as a standard user.");
                    }

                    var user = new User
                    {
                        Username = model.UserName,
                        FirstName = model.Name,
                        LastName = model.Surname,
                        Email = model.Email,
                        PhoneNumber = model.PhoneNumber,
                        PasswordHash = model.Password, // Ensure this is hashed appropriately
                        RoleID = roleID
                    };

                    _eventsContext.Users.Add(user);
                    await _eventsContext.SaveChangesAsync();

                    var userPreferences = new UserPreferences
                    {
                        UserID = user.UserID, // Ustawienie UserID dla preferencji
                        ReceiveMessages = false, // Domyślna wartość
                        PreferredDeliveryTime = TimeSpan.Parse("12:00:00"), // Domyślna wartość
                        DeliveryMethod = "email", // Domyślna wartość
                        BlockedHoursStart = TimeSpan.Parse("22:00:00"), // Domyślna wartość
                        BlockedHoursEnd = TimeSpan.Parse("06:00:00") // Domyślna wartość
                    };

                    _eventsContext.UserPreferences.Add(userPreferences);
                    await _eventsContext.SaveChangesAsync();
                    _logger.LogInformation($"User {model.Email} successfully registered.");

                    return RedirectToAction("Index", "Home");
                }

                _logger.LogWarning($"Registration attempt failed: User {model.Email} already exists.");
                ModelState.AddModelError(string.Empty, "Użytkownik o podanym adresie email już istnieje.");
            }

            _logger.LogWarning("Invalid model state during user registration.");
            return View(model);
        }


        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("CookieAuthentication");
            return RedirectToAction("Index", "Home");
        }
    }
}
