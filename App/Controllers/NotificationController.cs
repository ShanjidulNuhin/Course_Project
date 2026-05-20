using Microsoft.AspNetCore.Mvc;
using Game.BLL.Services;
using System;
using System.Linq;

namespace App.Controllers
{
    public class NotificationController : Controller
    {
        private readonly NotificationService _notificationService;

        public NotificationController(NotificationService notificationService)
        {
            _notificationService = notificationService;
        }
        [HttpGet]
        public IActionResult GetUnreadCount()
        {
            var token = Request.Cookies["AuthToken"];
            if (token == null) return Json(new { count = 0 });

            var count = _notificationService.GetUnreadCount(token);
            return Json(new { count = count });
        }
        [HttpGet]
        public IActionResult Get()
        {
            var token = Request.Cookies["AuthToken"];
            if (token == null) return Json(new { notifications = Array.Empty<object>() });

            var list = _notificationService.GetNotificationsForUser(token);
            var formatted = list.Select(n => new
            {
                n.Id,
                n.Message,
                CreatedAt = n.CreatedAt.ToString("o"),
                n.IsRead
            });
            return Json(new { notifications = formatted });
        }
        [HttpPost]
        public IActionResult MarkAllAsRead()
        {
            var token = Request.Cookies["AuthToken"];
            if (token == null) return Json(new { success = false });

            var result = _notificationService.MarkAllAsRead(token);
            return Json(new { success = result });
        }
    }
}
