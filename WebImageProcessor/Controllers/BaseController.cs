using Microsoft.AspNetCore.Mvc;

namespace WebImageProcessor.Controllers
{
    public class BaseController : Controller
    {
        // Метод для фіксації помилки
        protected void RegProblem(int statucCode, string title, string problem)
        {
            HttpContext.Response.StatusCode = statucCode;
            ViewData[title] = problem;
        }

        // Метод для переадресації
        protected IActionResult RedirectUser(string? link)
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
    }
}
