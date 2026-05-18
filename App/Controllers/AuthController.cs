using Microsoft.AspNetCore.Mvc;
using Game.BLL.Services;
using Game.BLL.DTOs;
namespace App.Controllers
{
    public class AuthController : Controller
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(LoginDTO dto)
        {
            var token = _authService.Login(dto);
            if (token==null)
            {
                ViewBag.Error = "Invalid email or password";
                return View();
            }
            Response.Cookies.Append("AuthToken", token);

            return RedirectToAction("Index", "User");
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Register(UserRegisterDTO dto)
        {
            var result = _authService.Register(dto);
            if (!result)
            {
                ViewBag.Error = "Registration failed. Email might already be in use.";
                return View();
            }
            return RedirectToAction("Login");
        }
        public IActionResult Logout()
        {
            var token=Request.Cookies["AuthToken"];
            if (token != null)
            {
                _authService.Logout(token);
                Response.Cookies.Delete("AuthToken");
            }
            return RedirectToAction("Index", "Home");
        }
        //public IActionResult Index()
        //{
        //    return View();
        //}
    }
}
