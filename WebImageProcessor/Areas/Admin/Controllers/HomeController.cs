using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebImageProcessor.Models;
using WebImageProcessor.ViewModel;
using WebImageProcessor.ViewModel.Models;

namespace WebImageProcessor.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        private readonly ImageProcessorDbContext db;

        public HomeController(ImageProcessorDbContext context) 
        {
            db = context;
        }

        [Authorize(Roles = "admin")]
        public IActionResult Index()
        {
            AdminPageViewModel usersInf = new AdminPageViewModel();

            usersInf.FullUsersInf = db.AppUsers
                .Select(user => new FullUserInfModel
                {
                    Nickname = user.Nickname,
                    Name = user.Name,
                    Surname = user.Surname,
                    Password = user.Password,
                    RoleName = user.Role.RoleName ?? "user"
                })
                .ToList();

            return View(usersInf);
        }

		[Authorize(Roles = "admin")]
		[HttpPost]
		public async Task<IActionResult> ChangeRole(string? nickname)
		{
			return await ProcessUserAction(nickname, async (user) =>
			{
				string? userRoleName = db.UserRoles
					.Where(r => r.RoleId == user.RoleId)
					.Select(n => n.RoleName)
					.FirstOrDefault();

				if (userRoleName == null)
				{
					RegProblem(401, "Не знайдена роль користувача.");
					return RedirectToAction("Index", "Home", new { area = "Admin" });
				}

				int userRoleId = db.UserRoles
					.Where(r => r.RoleName == "user")
					.Select(n => n.RoleId)
					.FirstOrDefault();

				int adminRoleId = db.UserRoles
					.Where(r => r.RoleName == "admin")
					.Select(n => n.RoleId)
					.FirstOrDefault();

				user.RoleId = CheckAdminRole(userRoleName) ? userRoleId : adminRoleId;

				db.AppUsers.Update(user);
				await db.SaveChangesAsync();

				return RedirectToAction("Index", "Home", new { area = "Admin" });
			});
		}

		[Authorize(Roles = "admin")]
		[HttpPost]
		public async Task<IActionResult> DeleteUser(string? nickname)
		{
			return await ProcessUserAction(nickname, async (user) =>
			{
				db.AppUsers.Remove(user);
				await db.SaveChangesAsync();

				return RedirectToAction("Index", "Home", new { area = "Admin" });
			});
		}

		private async Task<IActionResult> ProcessUserAction(string? nickname, Func<AppUser, Task<IActionResult>> action)
		{
			if (string.IsNullOrWhiteSpace(nickname))
			{
				RegProblem(401, "Некоректний нікнейм.");
				return RedirectToAction("Index", "Home", new { area = "Admin" });
			}

			AppUser? user = db.AppUsers.FirstOrDefault(x => x.Nickname == nickname);

			if (user == null)
			{
				RegProblem(401, "Користувач не знайдений.");
				return RedirectToAction("Index", "Home", new { area = "Admin" });
			}

			return await action(user);
		}

		private bool CheckAdminRole(string roleName)
        {
            return roleName.ToLower() == "admin";
		}

		public void RegProblem(int statucCode,  string problem)
        {
			HttpContext.Response.StatusCode = statucCode;
			ViewData["AdminMistake"] = problem;
		}
    }
}
