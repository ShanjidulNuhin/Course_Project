using Microsoft.AspNetCore.Mvc;
using Game.BLL.Services;
using Game.DAL.Repos;
using Game.DAL.EF.Tables;
using System;

namespace App.Controllers
{
    public class UserController : Controller
    {
        private readonly AuthService _authService;
        private readonly UserRepository _userRepo;
        private readonly GameService _gameService;

        public UserController(AuthService authService, UserRepository userRepo, GameService gameService)
        {
            _authService = authService;
            _userRepo = userRepo;
            _gameService = gameService;
        }

        public IActionResult Index()
        {
            var token = Request.Cookies["AuthToken"];
            if (token == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            var user = _userRepo.GetByToken(token);
            if (user == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            ViewBag.UserName = user.Name;
            ViewBag.Balance = user.Blance ?? 0;

            var games = _gameService.GetGamesForLandingPage();
            return View("Users", games);
        }

        [HttpGet]
        public IActionResult Profile()
        {
            var token = Request.Cookies["AuthToken"];
            if (token == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            var user = _userRepo.GetByToken(token);
            if (user == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            return View(user);
        }

        [HttpPost]
        public IActionResult UpdateProfile(string name, string email, string newPassword, string currentPassword)
        {
            var token = Request.Cookies["AuthToken"];
            if (token == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            var user = _userRepo.GetByToken(token);
            if (user == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            if (user.Password != currentPassword)
            {
                TempData["Error"] = "Verification failed: Incorrect password.";
                return RedirectToAction("Profile");
            }

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email))
            {
                TempData["Error"] = "Name and Email are required.";
                return RedirectToAction("Profile");
            }

            // Check if another user has this email
            var emailUser = _userRepo.GetByEmail(email);
            if (emailUser != null && emailUser.Id != user.Id)
            {
                TempData["Error"] = "Email is already in use by another user.";
                return RedirectToAction("Profile");
            }

            user.Name = name;
            user.Email = email;
            if (!string.IsNullOrEmpty(newPassword))
            {
                user.Password = newPassword;
            }

            _userRepo.Update(user);
            TempData["Success"] = "Profile updated successfully!";
            return RedirectToAction("Profile");
        }

        [HttpPost]
        public IActionResult DeleteAccount(string currentPassword)
        {
            var token = Request.Cookies["AuthToken"];
            if (token == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            var user = _userRepo.GetByToken(token);
            if (user == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            if (user.Password != currentPassword)
            {
                TempData["Error"] = "Verification failed: Incorrect password.";
                return RedirectToAction("Profile");
            }

            _userRepo.Delete(user);

            Response.Cookies.Delete("AuthToken");
            TempData["Success"] = "Account deleted successfully.";
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Library()
        {
            var token = Request.Cookies["AuthToken"];
            if (token == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            var games = _gameService.GetUserLibrary(token);
            return View(games);
        }

        [HttpGet]
        public IActionResult Balance()
        {
            var token = Request.Cookies["AuthToken"];
            if (token == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            return View();
        }

        [HttpPost]
        public IActionResult AddBalance(decimal amount, string[] paymentMethods)
        {
            var token = Request.Cookies["AuthToken"];
            if (token == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            var user = _userRepo.GetByToken(token);
            if (user == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            if (paymentMethods == null || paymentMethods.Length == 0)
            {
                ViewBag.Error = "Please select at least one mobile payment method (bKash, Nagad, or Rocket).";
                return View("Balance");
            }

            if (amount <= 0)
            {
                ViewBag.Error = "Please enter a valid amount greater than 0.";
                return View("Balance");
            }

            user.Blance = (user.Blance ?? 0) + amount;
            _userRepo.Update(user);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult PurchaseGame(int gameId)
        {
            var token = Request.Cookies["AuthToken"];
            if (token == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var success = _gameService.PurchaseGame(token, gameId, out string errorMessage);
            if (success)
            {
                TempData["Success"] = "Game purchased successfully!";
            }
            else
            {
                TempData["Error"] = errorMessage;
            }

            return RedirectToAction("Details", "Home", new { id = gameId });
        }

        [HttpPost]
        public IActionResult WishlistGame(int gameId)
        {
            var token = Request.Cookies["AuthToken"];
            if (token == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var success = _gameService.WishlistGame(token, gameId, out string errorMessage);
            if (success)
            {
                TempData["Success"] = "Game added to wishlist successfully!";
            }
            else
            {
                TempData["Error"] = errorMessage;
            }

            return RedirectToAction("Details", "Home", new { id = gameId });
        }

        [HttpPost]
        public IActionResult RemoveFromWishlist(int gameId)
        {
            var token = Request.Cookies["AuthToken"];
            if (token == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var success = _gameService.RemoveFromWishlist(token, gameId, out string errorMessage);
            if (success)
            {
                TempData["Success"] = "Game removed from wishlist successfully!";
            }
            else
            {
                TempData["Error"] = errorMessage;
            }

            return RedirectToAction("Library");
        }
    }
}
