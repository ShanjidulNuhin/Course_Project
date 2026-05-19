using System;

namespace Game.DAL.EF.Tables
{
    public class NotificationReadState
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int NotificationId { get; set; }
        public bool IsRead { get; set; }

        public virtual User? User { get; set; }
        public virtual Notification? Notification { get; set; }
    }
}
