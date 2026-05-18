using Microsoft.AspNetCore.Mvc;
using Game.BLL.Services;

namespace App.Controllers
{
    public class UserController : Controller
    {
        private readonly AuthService _authService;

        public UserController(AuthService authService)
        {
            _authService = authService;
        }

        public IActionResult Index()
        {
            var token = Request.Cookies["AuthToken"];
            if (token != null)
            {
                ViewBag.UserName = _authService.GetUserNameByToken(token);
            }
            return View("Users");
        }
        public IActionResult Profile()
        {
            return View();
        }
        public IActionResult Library()
        {
            return View();
        }
        public IActionResult Balance()
        {
            return View();
        }
    }
}
