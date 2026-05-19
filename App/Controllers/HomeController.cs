using App.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Game.BLL.Services;
using Game.DAL.Repos;
using System;

namespace App.Controllers
{
    public class HomeController : Controller
    {
        private readonly GameService _gameService;
        private readonly UserRepository _userRepo;

        public HomeController(GameService gameService, UserRepository userRepo)
        {
            _gameService = gameService;
            _userRepo = userRepo;
        }

        public IActionResult Index()
        {
            var token = Request.Cookies["AuthToken"];
            if (token != null)
            {
                var user = _userRepo.GetByToken(token);
                if (user != null)
                {
                    return RedirectToAction("Index", "User");
                }
            }

            var games = _gameService.GetGamesForLandingPage();
            return View(games);
        }

        public IActionResult Details(int id)
        {
            var game = _gameService.GetGameById(id);
            if (game == null)
            {
                return NotFound();
            }
            return View(game);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
