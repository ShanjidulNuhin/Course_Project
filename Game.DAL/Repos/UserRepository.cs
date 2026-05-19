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

        /// <summary>
        /// Returns all non-admin users (for admin dashboard)
        /// </summary>
        public List<User> GetAllCustomers()
        {
            return db.Users.Where(u => u.Role == "Customer").ToList();
        }

        /// <summary>
        /// Returns ALL users including admins (for admin overview)
        /// </summary>
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
            // Always set IsActive = 1 on registration
            u.IsActive = 1;
            db.Users.Add(u);
            return db.SaveChanges() > 0;
        }

        public bool Update(User u)
        {
            db.Users.Update(u);
            return db.SaveChanges() > 0;
        }

        /// <summary>
        /// Block a user: set IsActive = 0
        /// </summary>
        public bool BlockUser(int userId)
        {
            var user = GetById(userId);
            if (user == null) return false;
            user.IsActive = 0;
            // Invalidate any active token so they're logged out immediately
            user.Token = null;
            return db.SaveChanges() > 0;
        }

        /// <summary>
        /// Unblock a user: set IsActive = 1
        /// </summary>
        public bool UnblockUser(int userId)
        {
            var user = GetById(userId);
            if (user == null) return false;
            user.IsActive = 1;
            return db.SaveChanges() > 0;
        }

        /// <summary>
        /// Promote a customer to Admin role
        /// </summary>
        public bool PromoteToAdmin(int userId)
        {
            var user = GetById(userId);
            if (user == null) return false;
            user.Role = "Admin";
            return db.SaveChanges() > 0;
        }

        public bool Delete(User u)
        {
            // Remove user's libraries to prevent foreign key errors
            var libraries = db.Libraries.Where(l => l.UserId == u.Id);
            db.Libraries.RemoveRange(libraries);

            // Remove user's orders to prevent foreign key errors
            var orders = db.Orders.Where(o => o.UserId == u.Id);
            db.Orders.RemoveRange(orders);

            // Remove the user itself
            db.Users.Remove(u);

            return db.SaveChanges() > 0;
        }
    }
}
