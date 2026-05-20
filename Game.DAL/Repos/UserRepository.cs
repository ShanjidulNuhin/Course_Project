using System;
using System.Collections.Generic;
using System.Linq;
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
        public User? GetByEmail(string? email)
        {
            return db.Users.FirstOrDefault(u => u.Email == email);
        }
        public User? GetByToken(string token)
        {
            return db.Users.FirstOrDefault(u => u.Token == token);
        }
        public User? GetById(int id)
        {
            return db.Users.FirstOrDefault(u => u.Id == id);
        }
        public List<User> GetAllCustomers()
        {
            return db.Users.Where(u => u.Role == "Customer").ToList();
        }
        public List<User> GetAll()
        {
            return db.Users.ToList();
        }
        public bool AdminExists()
        {
            return db.Users.Any(u => u.Role == "Admin");
        }
        public bool Create(User u)
        {
            u.IsActive = 1;
            db.Users.Add(u);
            return db.SaveChanges() > 0;
        }
        public bool Update(User u)
        {
            db.Users.Update(u);
            return db.SaveChanges() > 0;
        }
        public bool BlockUser(int userId)
        {
            var user = GetById(userId);
            if (user == null) return false;
            user.IsActive = 0;
            user.Token = null;
            return db.SaveChanges() > 0;
        }
        public bool UnblockUser(int userId)
        {
            var user = GetById(userId);
            if (user == null) return false;
            user.IsActive = 1;
            return db.SaveChanges() > 0;
        }
        public bool PromoteToAdmin(int userId)
        {
            var user = GetById(userId);
            if (user == null) return false;
            user.Role = "Admin";
            return db.SaveChanges() > 0;
        }
        public bool Delete(User u)
        {
            var libraries = db.Libraries.Where(l => l.UserId == u.Id);
            db.Libraries.RemoveRange(libraries);

            var orders = db.Orders.Where(o => o.UserId == u.Id);
            db.Orders.RemoveRange(orders);

            db.Users.Remove(u);

            return db.SaveChanges() > 0;
        }
    }
}
