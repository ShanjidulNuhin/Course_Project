using Game.BLL.DTOs;
using Game.DAL.Repos;
using System.Collections.Generic;
using System.Linq;

// নেমস্পেস কনф্লিক্ট এড়াতে অ্যালিয়াস ব্যবহার করা হলো
using GameClass = Game.DAL.EF.Tables.Game;

namespace Game.BLL.Services
{
    public class GameService // এখানে কোনো ইন্টারফেস (: IGameService) থাকবে না
    {
        private readonly GameRepository _gameRepo;

        // কনস্ট্রাক্টরের মাধ্যমে সরাসরি রিপোজিটরি ইনজেক্ট করা হচ্ছে
        public GameService(GameRepository gameRepo)
        {
            _gameRepo = gameRepo;
        }

        public List<GameDTO> GetGamesForLandingPage()
        {
            // ১. রিপোজিটরি থেকে ডাটাবেজের সব গেম তুলে আনা হলো
            var gamesFromDb = _gameRepo.Get();

            // ২. উদাহরণ কোডের মতো করে ম্যানুয়ালি ম্যাপিং করা হচ্ছে (যেহেতু AutoMapper এখনো কনফিগার করেননি)
            var gameList = gamesFromDb.Select(g => new GameDTO
            {
                Id = g.Id,
                Title = g.Title,
                Genre = g.Genre,
                Cover = g.Cover,
                Description = g.Description,
                Price = g.Price
                // আপনার Game টেবিলে Cover না থাকায় 'Cover = g.Cover' লাইনটি বাদ দেওয়া হয়েছে
            }).ToList();

            return gameList;
        }
    }
}