using Microsoft.AspNetCore.Mvc;
using Game.BLL.Services;
using Game.BLL.DTOs;

namespace App.Controllers
{
    public class AdminController : Controller
    {
        private readonly AdminService _adminService;
        private readonly AuthService _authService;
        public AdminController(AdminService adminService, AuthService authService)
        {
            _adminService = adminService;
            _authService = authService;
        }
        private bool IsAdmin()
        {
            var token = Request.Cookies["AuthToken"];
            if (token == null) return false;
            return _authService.GetRoleByToken(token) == "Admin";
        }
        [HttpGet]
        public IActionResult Index()
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "Auth");
            }
            var games = _adminService.GetAllGames();
            var users = _adminService.GetAllUsers();
            ViewBag.GamesCount = games.Count;
            ViewBag.UsersCount = users.Count(u => u.Role == "Customer");
            ViewBag.BlockedCount = users.Count(u => u.IsActive == 0);
            ViewBag.TotalBalance = users.Sum(u => u.Blance ?? 0);
            
            ViewBag.Games = games;
            ViewBag.Users = users;
            ViewBag.BlockedUsers = users.Where(u => u.IsActive == 0).ToList();
            ViewBag.MostPurchasedGames = _adminService.GetMostPurchasedGames();
            
            return View();
        }
        [HttpGet]
        public IActionResult Games()
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "Auth");
            }
            var games = _adminService.GetAllGames();
            return View(games);
        }
        [HttpGet]
        public IActionResult AddGame()
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "Auth");
            }

            return View();
        }
        [HttpPost]
        public IActionResult AddGame(GameCreateDTO dto, IFormFile? CoverFile)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "Auth");
            }
            if (CoverFile != null && CoverFile.Length > 0)
            {
                using var stream = CoverFile.OpenReadStream();
                using var br = new BinaryReader(stream);
                dto.Cover = br.ReadBytes((int)CoverFile.Length);
            }
            if (string.IsNullOrWhiteSpace(dto.Title) || string.IsNullOrWhiteSpace(dto.Genre))
            {
                ViewBag.Error = "Title and Genre are required.";
                return View(dto);
            }
            var result = _adminService.AddGame(dto);
            if (!result)
            {
                ViewBag.Error = "Failed to add game.";
                return View(dto);
            }
            TempData["Success"] = $"Game '{dto.Title}' added successfully!";
            return RedirectToAction("Games");
        }
        [HttpGet]
        public IActionResult EditGame(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Auth");
            var game = _adminService.GetGameById(id);
            if (game == null)
            {
                TempData["Error"] = "Game not found.";
                return RedirectToAction("Games");
            }
            return View(game);
        }
        [HttpPost]
        public IActionResult EditGame(GameDTO dto, IFormFile CoverFile)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Auth");
            if (CoverFile != null && CoverFile.Length > 0)
            {
                using var stream = CoverFile.OpenReadStream();
                using var br = new BinaryReader(stream);

                dto.Cover = br.ReadBytes((int)CoverFile.Length);
            }
            else
            {
                var existing = _adminService.GetGameById(dto.Id);
                dto.Cover = existing?.Cover;
            }
            if (string.IsNullOrWhiteSpace(dto.Title) || string.IsNullOrWhiteSpace(dto.Genre))
            {
                ViewBag.Error = "Title and Genre are required.";
                return View(dto);
            }
            var result = _adminService.UpdateGame(dto);
            if (!result)
            {
                ViewBag.Error = "Update failed.";
                return View(dto);
            }
            TempData["Success"] = $"Game '{dto.Title}' updated successfully!";
            return RedirectToAction("Games");
        }
        [HttpPost]
        public IActionResult DeleteGame(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Auth");

            _adminService.DeleteGame(id);
            TempData["Success"] = "Game deleted.";
            return RedirectToAction("Games");
        }
        [HttpGet]
        public IActionResult Users()
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Auth");

            var users = _adminService.GetAllUsers();
            return View(users);
        }
        [HttpPost]
        public IActionResult BlockUser(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Auth");
            _adminService.BlockUser(id);
            TempData["Success"] = "User blocked successfully.";
            return RedirectToAction("Users");
        }
        [HttpPost]
        public IActionResult UnblockUser(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Auth");
            _adminService.UnblockUser(id);
            TempData["Success"] = "User unblocked successfully.";
            return RedirectToAction("Users");
        }
        [HttpPost]
        public IActionResult PromoteUser(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Auth");
            _adminService.PromoteToAdmin(id);
            TempData["Success"] = "User promoted to Admin.";
            return RedirectToAction("Users");
        }
    }
}
