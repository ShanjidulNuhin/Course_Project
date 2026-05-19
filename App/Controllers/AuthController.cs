using Game.BLL.DTOs;
using Game.BLL.Services;
using Game.DAL.EF.Tables;
using Microsoft.AspNetCore.Mvc;
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
            if (token == "BLOCKED")
            {
                ViewBag.Error = "Your account has been blocked. Please contact the administrator.";
                return View();
            }
            Response.Cookies.Append("AuthToken", token);
            var role = _authService.GetRoleByToken(token);
            if (role == "Admin")
                return RedirectToAction("Index", "Admin");
            return RedirectToAction("Index", "User");
        }
        [HttpGet]
        public IActionResult Register()
        {
            ViewBag.AdminExists = _authService.AdminExists();
            return View();
        }
        [HttpPost]
        public IActionResult Register(UserRegisterDTO dto)
        {
            var result = _authService.Register(dto);
            if (!result)
            {
                ViewBag.Error = "Registration failed. Email might already be in use.";
                ViewBag.AdminExists = _authService.AdminExists();
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
