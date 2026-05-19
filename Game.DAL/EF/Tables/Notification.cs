using System;

namespace Game.DAL.EF.Tables
{
    public class Notification
    {
        public int Id { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? TargetRole { get; set; }
        public int? TargetUserId { get; set; }
    }
}
