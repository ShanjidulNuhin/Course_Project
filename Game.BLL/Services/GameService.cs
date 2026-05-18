using Game.BLL.DTOs;
using Game.DAL.Repos;
using System.Collections.Generic;
using System.Linq;

using GameClass = Game.DAL.EF.Tables.Game;

namespace Game.BLL.Services
{
    public class GameService 
    {
        private readonly GameRepository _gameRepo;

        public GameService(GameRepository gameRepo)
        {
            _gameRepo = gameRepo;
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
    }
}