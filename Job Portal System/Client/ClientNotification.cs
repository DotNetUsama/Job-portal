using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Job_Portal_System.Enums;
using Job_Portal_System.Models;

namespace Job_Portal_System.Client
{
    public class ClientNotification
    {
        public string Id { get; set; }
        public string Url { get; set; }
        public string Message { get; set; }
        public bool IsRead { get; set; }
        public string CreatedAt { get; set; }
        public string Type { get; set; }

        public ClientNotification(UserNotification userNotification)
        {
            Id = userNotification.Id;
            Url = userNotification.Notification.GetUrl();
            Message = userNotification.Notification.GetMessage();
            IsRead = userNotification.IsRead;
            CreatedAt = userNotification.Notification.CreatedAt.ToString("MM/dd/yyyy H:mm");
            Type = ((NotificationType) userNotification.Notification.Type).GetDescription();
        }
    }
}
