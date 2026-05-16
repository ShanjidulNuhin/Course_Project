using Game.DAL.EF;
using Game.DAL.EF.Tables;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.DAL.Repos
{
    public class GameRepository
    {
        private readonly GameSpdbContext db;

        public GameRepository(GameSpdbContext db)
        {
            
            this.db = db;
        }

        public bool Create(Game g)
        {
            db.Games.Add(g);
            return db.SaveChanges() > 0;
        }

        public List<Game> Get()
        {
            return db.Games
                     .Include(g => g.Category)
                     .Where(g => g.IsActive)
                     .ToList();
        }

        public Game? Get(int id)
        {
            return db.Games
                     .Include(g => g.Category)
                     .FirstOrDefault(g => g.Id == id);
        }

        public bool Update(Game g)
        {
            var exobj = Get(g.Id);

            if (exobj == null)
            {
                return false;
            }

            db.Entry(exobj).CurrentValues.SetValues(g);
            return db.SaveChanges() > 0;
        }

        public bool Delete(int id)
        {
            var exobj = Get(id);
            if (exobj == null)
            {
                return false;
            }
            exobj.IsActive = false;
            return db.SaveChanges() > 0;
        }
    }
}