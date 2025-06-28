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

            var existingUser = await _authService.GetUserByEmailAsync(model.Email);
            if (existingUser != null)
            {
                ViewBag.Message = $"هذا الإيميل مسجل بالفعل كـ {existingUser.Role}.";
                return View(model);
            }

            // ✅ قم بتسجيل الطالب فقط (الرول ثابت)
            var success = await _authService.RegisterAsync(model.Email, model.Password, "Student", model.FullName);

            if (!success)
            {
                ViewBag.Message = "حدث خطأ أثناء التسجيل.";
                return View(model);
            }

            // تسجيل الدخول التلقائي
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, model.Email),
        new Claim(ClaimTypes.Role, "Student"),
        new Claim(ClaimTypes.Email, model.Email)
    };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return RedirectToAction("Dashboard", "StudentDashboard");
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

            // ⬅️ ناخد الدور الحقيقي من قاعدة البيانات
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, user.Email),
        new Claim(ClaimTypes.Role, user.Role),
        new Claim(ClaimTypes.Email, user.Email)
    };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return user.Role switch
            {
                "Student" => RedirectToAction("Dashboard", "StudentDashboard"),
                "Teacher" => RedirectToAction("Dashboard", "TeacherDashboard"),
                "Admin" => RedirectToAction("Index", "Dashboard"),
                _ => RedirectToAction("Welcome", "Auth")
            };
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
