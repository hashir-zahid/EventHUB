using System.Diagnostics;
using APProj.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using static EventHUB.Models.ApplicationDbContext;

namespace EventHUB.Controllers
{
    public class LoginController : Controller
    {
        private readonly ApplicationDbcontext db;

        public LoginController(ApplicationDbcontext context)
        {
            db = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            try
            {
                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                {
                    ViewBag.Error = "Email and password are required";
                    return View();
                }

                // Hardcoded Admin Check
                if ((email == "Hashir@gmail.com" || email == "Ammad@gmail.com") && password == "123")
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, email),
                        new Claim(ClaimTypes.Role, "Admin")
                    };

                    var identity = new ClaimsIdentity(claims, "MyCookieAuth");
                    var principal = new ClaimsPrincipal(identity);

                    await HttpContext.SignInAsync("MyCookieAuth", principal);

                    return RedirectToAction("AdminDashboard", "Admin");
                }

                // Check regular user
                var user = db.Users.FirstOrDefault(u => u.Email == email && u.Password == password);

                if (user != null)
                {
                    if (!user.IsApproved)
                        return RedirectToAction("PendingApproval","Login");

                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.Email),
                        new Claim(ClaimTypes.Role, "User")
                    };

                    var identity = new ClaimsIdentity(claims, "MyCookieAuth");
                    var principal = new ClaimsPrincipal(identity);

                    await HttpContext.SignInAsync("MyCookieAuth", principal);

                    return RedirectToAction("MemberDashboard", "Member");
                }

                ViewBag.Error = "Invalid Email or Password";
                return View();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Login error: {ex.Message}");
                ViewBag.Error = "An error occurred during login.";
                return View();
            }
        }
        public IActionResult PendingApproval()
        {
            return View();
        }
    }
}
