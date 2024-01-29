using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using WebImageProcessor.Models;

namespace WebImageProcessor.Controllers
{
    //Подумати про реєстрацію та аунтефікацію за допомогою jwt токенів

    public class LoginLogoutRegController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        ImageProcessorDbContext db;

        public LoginLogoutRegController(ILogger<HomeController> logger, ImageProcessorDbContext context)
        {
            _logger = logger;
            db = context;
        }

        [HttpGet]
        public IActionResult Login() 
        {
            return View();
        }

        [HttpPost]
        public IActionResult LoginAsync(string password, string nickname)
        {
            if (db.AppUsers.Any(b => b.Password == password && b.Nickname == nickname))
            {
                AppUser user = db.AppUsers.FirstOrDefault(b => b.Password == password && b.Nickname == nickname);

                if (user.Password == password)
                {
                    HttpContext.Response.Cookies.Append("nickname", user.Nickname.ToString());

                    UserRole userRole = db.UserRoles.Where(r => r.RoleId == user.RoleId).FirstOrDefault();

                    // Занесення даних про користувача в систему, для подальшого використання у праві доступа до деяких функцій
                    SetUserInSystem(nickname, userRole.RoleName);

                    return RedirectUser(HttpContext.Request.Cookies["lastVisitedURL"]);
                }
                else
                {
                    HttpContext.Response.StatusCode = 401;
                    ViewData["LoginMistake"] = "Невірно введений пароль.";

                    return View();
                }
            }
            else
            {
                HttpContext.Response.StatusCode = 401;
                ViewData["LoginMistake"] = "Користвача з таким нікнеймом не існує.";

                return View();
            }
        }

        [HttpGet]
        public IActionResult Registration() 
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Registration(AppUser user)
        {
            //При реєстрації задається роль по дефолту, а саме користувач, можна передбачити можливість
            //надання прав адміна іншим адміном

            if (db.AppUsers.Any(b => b.Nickname == user.Nickname))
            {
                HttpContext.Response.StatusCode = 409;

                ViewData["RegistrationMistake"] = "Користувач з таким ніком вже існує.";

                return View();
            }
            else
            {
                user.RoleId = 1;
                db.AppUsers.Add(user);
                await db.SaveChangesAsync();

                SetUserInSystem(user.Nickname, "user");

                HttpContext.Response.Cookies.Append("nickname", user.Nickname.ToString());
            }

            return RedirectUser(HttpContext.Request.Cookies["lastVisitedURL"]);
        }

        public async Task<IActionResult> LogOutAsync()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (string.IsNullOrEmpty(HttpContext.Request.Cookies["nickname"]))
            {
                return RedirectUser(HttpContext.Request.Cookies["lastVisitedURL"]);
            }
            else
            {
                HttpContext.Response.Cookies.Delete("nickname");

                return RedirectUser("");
            }
        }

        private IActionResult RedirectUser(string link)
        {
            // Переадресація користувача но сторінку з якої він перейшов на сторінку реєстрації
            if (string.IsNullOrEmpty(link))
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return Redirect(link);
            }
        }

        // Метод для занесення даних про користувача у систему
        private async void SetUserInSystem(string nickname, string userRole)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, nickname),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, userRole)
            };

            var claimsIdentity = new ClaimsIdentity(claims, "Cookies");
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            await HttpContext.SignInAsync(claimsPrincipal);
        }
    }
}
