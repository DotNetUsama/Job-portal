using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Job_Portal_System.Data;
using Job_Portal_System.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Job_Portal_System.Controllers
{
    [Route("Notifications")]
    public class NotificationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public NotificationsController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost]
        [Route("GetNotifications")]
        public async Task<IActionResult> GetNotifications()
        {
            var user = await _userManager.GetUserAsync(User);
            return Json(_context.GetNotifications(user));
        }

        [HttpPost]
        [Route("GetNotificationsCount")]
        public async Task<IActionResult> GetNotificationsCount()
        {
            return Json(_context.GetNotificationsCount(await _userManager.GetUserAsync(User)));
        }

        [HttpPost]
        [Route("ReadNotification")]
        public void ReadNotification(string id)
        {
            var userNotification =
                _context.UserNotifications.SingleOrDefault(notification => notification.Id == id);
            if (userNotification == null) return;
            userNotification.IsRead = true;
            _context.SaveChanges();
        }
    }
}