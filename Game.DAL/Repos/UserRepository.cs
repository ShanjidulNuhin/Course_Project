using System;
using System.Collections.Generic;
using System.Text;
using Game.DAL.EF;
using Game.DAL.EF.Tables;

namespace Game.DAL.Repos
{
    public class UserRepository
    {
        private readonly GameSpdbContext db;

        public UserRepository(GameSpdbContext db)
        {
            this.db = db;
        }
        public User? GetByEmail(string email)
        {
            return db.Users.FirstOrDefault(u => u.Email == email);
        }
        public User? GetByToken(string token)
        {
            return db.Users.FirstOrDefault(u => u.token == token);
        }
        public bool Create(User u)
        {
            db.Users.Add(u);
            return db.SaveChanges() > 0;
        }
        public bool Update(User u) {
            db.Users.Update(u);
           return db.SaveChanges()>0;
        }
    }
}
