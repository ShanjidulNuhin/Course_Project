using Game.BLL.DTOs;
using Game.DAL.Repos;
using Game.DAL.EF;
using System;
using System.Collections.Generic;
using System.Linq;

using GameClass = Game.DAL.EF.Tables.Game;

namespace Game.BLL.Services
{
    public class GameService 
    {
        private readonly GameRepository _gameRepo;
        private readonly UserRepository _userRepo;
        private readonly GameSpdbContext _db;
        private readonly NotificationService _notificationService;

        public GameService(GameRepository gameRepo, UserRepository userRepo, GameSpdbContext db, NotificationService notificationService)
        {
            _gameRepo = gameRepo;
            _userRepo = userRepo;
            _db = db;
            _notificationService = notificationService;
        }
        public List<GameDTO> GetGamesForLandingPage()
        {
            var gamesFromDb = _gameRepo.Get();

            var gameList = gamesFromDb.Select(g => new GameDTO
            {
                Id = g.Id,
                Title = g.Title,
                Genre = g.Genre,
                Cover = g.Cover,
                Description = g.Description,
                Price = g.Price
            }).ToList();

            return gameList;
        }
        public GameDTO? GetGameById(int id)
        {
            var g = _gameRepo.Get(id);
            if (g == null) return null;
            return new GameDTO
            {
                Id = g.Id,
                Title = g.Title,
                Genre = g.Genre,
                Cover = g.Cover,
                Description = g.Description,
                Price = g.Price
            };
        }
        public List<LibraryViewDTO> GetUserLibrary(string token)
        {
            var user = _userRepo.GetByToken(token);
            if (user == null)
            {
                return new List<LibraryViewDTO>();
            }
            var libraryItems = _db.Libraries
                .Where(l => l.UserId == user.Id)
                .Select(l => new LibraryViewDTO
                {
                    Id = l.Id,
                    UserId = l.UserId,
                    GameId = l.GameId,
                    GameTitle = l.Game != null ? l.Game.Title : "",
                    Genre = l.Game != null ? l.Game.Genre : "",
                    Cover = l.Game != null ? l.Game.Cover : null,
                    Description = l.Game != null ? l.Game.Description : "",
                    Price = l.Game != null ? l.Game.Price : 0,
                    Cart_Type = l.Cart_Type,
                    OrderDate = l.OrderDate
                })
                .ToList();

            return libraryItems;
        }
        public bool PurchaseGame(string token, int gameId, out string errorMessage)
        {
            errorMessage = "";
            var user = _userRepo.GetByToken(token);
            if (user == null)
            {
                errorMessage = "User not found or session expired. Please log in.";
                return false;
            }
            var game = _gameRepo.Get(gameId);
            if (game == null)
            {
                errorMessage = "Game not found.";
                return false;
            }
            var alreadyOwned = _db.Libraries.Any(l => l.UserId == user.Id && l.GameId == game.Id && l.Cart_Type == "Purchased");
            if (alreadyOwned)
            {
                errorMessage = "You already own this game!";
                return false;
            }
            var price = game.Price;
            var balance = user.Blance ?? 0;
            if (balance < price)
            {
                errorMessage = $"Insufficient balance. The game costs ${price:N2}, but your balance is ${balance:N2}. Please add balance.";
                return false;
            }
            user.Blance = balance - price;
            _userRepo.Update(user);
            var libraryRecord = new Game.DAL.EF.Tables.Library
            {
                UserId = user.Id,
                GameId = game.Id,
                Cart_Type = "Purchased",
                OrderDate = DateOnly.FromDateTime(DateTime.Now)
            };
            _db.Libraries.Add(libraryRecord);
            var orderRecord = new Game.DAL.EF.Tables.Order
            {
                UserId = user.Id,
                OrderDate = DateOnly.FromDateTime(DateTime.Now),
                TotalPrice = price
            };
            _db.Orders.Add(orderRecord);
            _db.SaveChanges();
            _notificationService.CreateNotification($"💰 Customer '{user.Name}' purchased game '{game.Title}'!", "Admin", null);

            return true;
        }
        public bool WishlistGame(string token, int gameId, out string errorMessage)
        {
            errorMessage = "";
            var user = _userRepo.GetByToken(token);
            if (user == null)
            {
                errorMessage = "User not found or session expired. Please log in.";
                return false;
            }

            var game = _gameRepo.Get(gameId);
            if (game == null)
            {
                errorMessage = "Game not found.";
                return false;
            }
            var alreadyWishlisted = _db.Libraries.Any(l => l.UserId == user.Id && l.GameId == game.Id && l.Cart_Type == "Wishlist");
            if (alreadyWishlisted)
            {
                errorMessage = "This game is already in your wishlist!";
                return false;
            }
            var libraryRecord = new Game.DAL.EF.Tables.Library
            {
                UserId = user.Id,
                GameId = game.Id,
                Cart_Type = "Wishlist",
                OrderDate = DateOnly.FromDateTime(DateTime.Now)
            };
            _db.Libraries.Add(libraryRecord);
            _db.SaveChanges();
            return true;
        }
        public bool RemoveFromWishlist(string token, int gameId, out string errorMessage)
        {
            errorMessage = "";
            var user = _userRepo.GetByToken(token);
            if (user == null)
            {
                errorMessage = "User not found or session expired. Please log in.";
                return false;
            }
            var wishlistRecord = _db.Libraries.FirstOrDefault(l => l.UserId == user.Id && l.GameId == gameId && l.Cart_Type == "Wishlist");
            if (wishlistRecord == null)
            {
                errorMessage = "Game not found in your wishlist.";
                return false;
            }
            _db.Libraries.Remove(wishlistRecord);
            _db.SaveChanges();
            return true;
        }
    }
}
