using System;
using System.Collections.Generic;
using System.Text;
using Game.BLL.DTOs;
using Game.DAL.Repos;
using Game.DAL.EF.Tables;

namespace Game.BLL.Services
{
    public class AuthService
    {
        private readonly UserRepository _userRepo;
        public AuthService(UserRepository userRepo)
        {
            _userRepo = userRepo;
        }
        public bool Register(UserRegisterDTO dto)
        {
            var exists = _userRepo.GetByEmail(dto.Email);
            if (exists != null)
            {
                return false;
            }

            string role = "Customer";
            if (!_userRepo.AdminExists())
            {
                role = string.Equals(dto.Role, "Admin", StringComparison.OrdinalIgnoreCase) ? "Admin" : "Customer";
            }

            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                Password = dto.Password,
                Role = role,
                IsActive = 1
            };
            return _userRepo.Create(user);
        }
        public string? Login(LoginDTO dTO)
        {
            var user = _userRepo.GetByEmail(dTO.Email);
            if (user == null || user.Password != dTO.Password)
            {
                return null;
            }
            if (user.IsActive == 0)
            {
                return "BLOCKED";
            }
            var token = Guid.NewGuid().ToString();
            user.Token = token;
            _userRepo.Update(user);
            return token;
        }
        public bool ValidateToken(string token)
        {
            var u = _userRepo.GetByToken(token);
            return u != null;
        }
        public string? GetUserNameByToken(string token)
        {
            var u = _userRepo.GetByToken(token);
            return u?.Name;
        }
        public string? GetRoleByToken(string token)
        {
            var u = _userRepo.GetByToken(token);
            return u?.Role;
        }
        public void Logout(string token)
        {
            var u = _userRepo.GetByToken(token);
            if (u != null)
            {
                u.Token = null;
                _userRepo.Update(u);
            }
        }
        public bool AdminExists()
        {
            return _userRepo.AdminExists();
        }
    }
}
