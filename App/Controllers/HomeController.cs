using App.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Game.BLL.Services;
using System;

namespace App.Controllers
{
    public class HomeController : Controller
    {
        private readonly GameService _gameService;
        public HomeController(GameService gameService)
        {
            _gameService = gameService;
        }
        public IActionResult Index()
        {
            var games = _gameService.GetGamesForLandingPage();
            return View(games);
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
