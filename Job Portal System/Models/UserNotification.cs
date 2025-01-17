﻿namespace Job_Portal_System.Models
{
    public class UserNotification
    {
        public string Id { get; set; }

        public bool IsRead { get; set; } = false;

        public User User { get; set; }
        public Notification Notification { get; set; }

        public string UserId { get; set; }
        public string NotificationId { get; set; }
    }
}
