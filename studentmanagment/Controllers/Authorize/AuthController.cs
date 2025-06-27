using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentManagement.Services.Interfaces;
using StudentManagement.Web.ViewModels;
using System.Security.Claims;

namespace StudentManagement.Web.Controllers.Authorize
{
    [AllowAnonymous]
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var success = await _authService.RegisterAsync(model.Email, model.Password, model.Role, model.FullName);

            if (!success)
            {
                ViewBag.Message = "Email is already registered.";
                return View(model);
            }

            // ✅ تسجيل الدخول تلقائيًا بعد التسجيل
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, model.Email),
                new Claim(ClaimTypes.Role, model.Role),
                    new Claim(ClaimTypes.Email, model.Email) 

            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            // ✅ التوجيه إلى صفحة Welcome بعد التسجيل
            return RedirectToAction("Welcome", "Auth");
        }

        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var user = await _authService.LoginAsync(email, password);

            if (user == null)
            {
                ViewBag.Message = "Invalid email or password.";
                return View();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.Role, user.Role),
               new Claim(ClaimTypes.Email, user.Email) 

            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            // ✅ التوجيه إلى صفحة Welcome بعد تسجيل الدخول
            return RedirectToAction("Welcome", "Auth");
        }

        [Authorize]
        public IActionResult Welcome()
        {
            ViewBag.Username = User.Identity?.Name;
            ViewBag.Role = User.FindFirst(ClaimTypes.Role)?.Value;
            return View();
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            TempData["ErrorMessage"] = "يجب أن تكون Admin للوصول إلى هذه الصفحة.";
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Auth");
        }
    }
}
