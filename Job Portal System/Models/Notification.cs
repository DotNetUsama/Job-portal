using System;
using System.Collections.Generic;
using Job_Portal_System.Data;
using Job_Portal_System.Enums;

namespace Job_Portal_System.Models
{
    public class Notification
    {
        public string Id { get; set; }
        
        public int Type { get; set; }

        public string EntityId { get; set; }
        public string Peer1 { get; set; }
        public string Peer2 { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        public ICollection<UserNotification> UserNotifications { get; set; }

        public string GetUrl()
        {
            return ((NotificationType) Type).GetUrl(this);
        }

        public string GetMessage()
        {
            return ((NotificationType)Type).GetMessage(this);
        }
    }
}
