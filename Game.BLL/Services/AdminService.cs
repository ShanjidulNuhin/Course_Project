using System.Collections.Generic;
using System.Linq;
using Game.BLL.DTOs;
using Game.DAL.Repos;
using Game.DAL.EF;
using GameClass = Game.DAL.EF.Tables.Game;

namespace Game.BLL.Services
{
    public class AdminService
    {
        private readonly UserRepository _userRepo;
        private readonly GameRepository _gameRepo;
        private readonly GameSpdbContext _db;
        private readonly NotificationService _notificationService;
        public AdminService(UserRepository userRepo, GameRepository gameRepo, GameSpdbContext db, NotificationService notificationService)
        {
            _userRepo = userRepo;
            _gameRepo = gameRepo;
            _db = db;
            _notificationService = notificationService;
        }
        public List<UserDTO> GetAllUsers()
        {
            return _userRepo.GetAll()
                .Select(u => new UserDTO
                {
                    Id = u.Id,
                    Name = u.Name,
                    Email = u.Email,
                    Blance = u.Blance,
                    Role = u.Role,
                    IsActive = u.IsActive??1
                }).ToList();
        }
        public bool BlockUser(int userId) => _userRepo.BlockUser(userId);
        public bool UnblockUser(int userId) => _userRepo.UnblockUser(userId);
        public bool PromoteToAdmin(int userId)
        {
            var user = _userRepo.GetById(userId);
            if (user == null) return false;
            var userName = user.Name;
            var result = _userRepo.PromoteToAdmin(userId);
            if (result)
            {
                _notificationService.CreateNotification($"🚀 Customer '{userName}' has been promoted to Admin!", "Customer", null);
                _notificationService.CreateNotification("🎉 You have been promoted to Admin by the administrator!", null, userId);
            }
            return result;
        }
        public List<GameDTO> GetAllGames()
        {
            return _gameRepo.Get().Select(g => new GameDTO
            {
                Id = g.Id,
                Title = g.Title,
                Genre = g.Genre,
                Cover = g.Cover,
                Description = g.Description,
                Price = g.Price
            }).ToList();
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
        public bool AddGame(GameCreateDTO dto)
        {
            var game = new GameClass
            {
                Title = dto.Title,
                Genre = dto.Genre,
                Cover = dto.Cover,
                Description = dto.Description,
                Price = dto.Price
            };
            var result = _gameRepo.Create(game);
            if (result)
            {
                _notificationService.CreateNotification($"🎮 New game added: '{game.Title}'!", "Customer", null);
            }
            return result;
        }
        public bool UpdateGame(GameDTO dto)
        {
            var game = new GameClass
            {
                Id = dto.Id,
                Title = dto.Title,
                Genre = dto.Genre,
                Cover = dto.Cover,
                Description = dto.Description,
                Price = dto.Price
            };
            return _gameRepo.Update(game);
        }
        public bool DeleteGame(int id) => _gameRepo.Delete(id);
        public List<MostPurchasedGameDTO> GetMostPurchasedGames()
        {
            var stats = _db.Libraries
                .Where(l => l.Cart_Type == "Purchased" && l.GameId != null)
                .GroupBy(l => l.GameId)
                .Select(g => new
                {
                    GameId = g.Key,
                    PurchaseCount = g.Count()
                })
                .OrderByDescending(x => x.PurchaseCount)
                .ToList();

            var gameIds = stats.Select(s => s.GameId).ToList();
            var games = _db.Games.Where(g => gameIds.Contains(g.Id)).ToList();
            var gameMap = games.ToDictionary(g => g.Id);

            var list = new List<MostPurchasedGameDTO>();
            foreach (var item in stats)
            {
                if (item.GameId == null) continue;
                gameMap.TryGetValue(item.GameId.Value, out var game);
                list.Add(new MostPurchasedGameDTO
                {
                    Id = item.GameId.Value,
                    Title = game?.Title ?? "Unknown",
                    Genre = game?.Genre ?? "Unknown",
                    Cover = game?.Cover,
                    Description = game?.Description ?? "",
                    Price = game?.Price ?? 0,
                    PurchaseCount = item.PurchaseCount
                });
            }
            return list;
        }
    }
}
