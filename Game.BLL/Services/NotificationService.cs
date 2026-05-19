using Game.BLL.DTOs;
using Game.DAL.Repos;
using Game.DAL.EF;
using Game.DAL.EF.Tables;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.BLL.Services
{
    public class NotificationService
    {
        private readonly GameSpdbContext _db;
        private readonly UserRepository _userRepo;

        public NotificationService(GameSpdbContext db, UserRepository userRepo)
        {
            _db = db;
            _userRepo = userRepo;
        }

        public List<NotificationDTO> GetNotificationsForUser(string token)
        {
            var user = _userRepo.GetByToken(token);
            if (user == null) return new List<NotificationDTO>();

            var role = user.Role ?? "Customer";
            var userId = user.Id;

            var notifications = _db.Notifications
                .Where(n => n.TargetRole == role || n.TargetUserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToList();

            var readNotificationIds = _db.NotificationReadStates
                .Where(r => r.UserId == userId && r.IsRead)
                .Select(r => r.NotificationId)
                .ToHashSet();

            return notifications.Select(n => new NotificationDTO
            {
                Id = n.Id,
                Message = n.Message,
                CreatedAt = n.CreatedAt,
                IsRead = readNotificationIds.Contains(n.Id)
            }).ToList();
        }

        public int GetUnreadCount(string token)
        {
            var user = _userRepo.GetByToken(token);
            if (user == null) return 0;

            var role = user.Role ?? "Customer";
            var userId = user.Id;

            var totalNotificationIds = _db.Notifications
                .Where(n => n.TargetRole == role || n.TargetUserId == userId)
                .Select(n => n.Id)
                .ToList();

            var readNotificationIds = _db.NotificationReadStates
                .Where(r => r.UserId == userId && r.IsRead && totalNotificationIds.Contains(r.NotificationId))
                .Select(r => r.NotificationId)
                .ToList();

            return totalNotificationIds.Count - readNotificationIds.Count;
        }

        public bool MarkAllAsRead(string token)
        {
            var user = _userRepo.GetByToken(token);
            if (user == null) return false;

            var role = user.Role ?? "Customer";
            var userId = user.Id;

            var eligibleNotificationIds = _db.Notifications
                .Where(n => n.TargetRole == role || n.TargetUserId == userId)
                .Select(n => n.Id)
                .ToList();

            var alreadyReadIds = _db.NotificationReadStates
                .Where(r => r.UserId == userId && r.IsRead)
                .Select(r => r.NotificationId)
                .ToList();

            var unreadIds = eligibleNotificationIds.Except(alreadyReadIds).ToList();

            if (!unreadIds.Any()) return true;

            foreach (var notificationId in unreadIds)
            {
                var readState = _db.NotificationReadStates
                    .FirstOrDefault(r => r.UserId == userId && r.NotificationId == notificationId);

                if (readState == null)
                {
                    _db.NotificationReadStates.Add(new NotificationReadState
                    {
                        UserId = userId,
                        NotificationId = notificationId,
                        IsRead = true
                    });
                }
                else
                {
                    readState.IsRead = true;
                }
            }

            return _db.SaveChanges() > 0;
        }

        public bool CreateNotification(string message, string? targetRole, int? targetUserId)
        {
            var notification = new Notification
            {
                Message = message,
                CreatedAt = DateTime.UtcNow,
                TargetRole = targetRole,
                TargetUserId = targetUserId
            };
            _db.Notifications.Add(notification);
            return _db.SaveChanges() > 0;
        }
    }
}
