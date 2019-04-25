using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Job_Portal_System.Client;
using Job_Portal_System.Handlers;
using Job_Portal_System.Models;
using Job_Portal_System.SignalR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Job_Portal_System.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions options)
            : base(options)
        {
        }
        
        public DbSet<Recruiter> Recruiters { get; set; }
        public DbSet<JobSeeker> JobSeekers { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Announcement> Announcements { get; set; }
        public DbSet<Applicant> Applicants { get; set; }
        public DbSet<Education> Educations { get; set; }
        public DbSet<CompanyDepartment> CompanyDepartments { get; set; }
        public DbSet<DesiredSkill> DesiredSkills { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<EducationQualification> EducationQualifications { get; set; }
        public DbSet<FieldOfStudy> FieldOfStudies { get; set; }
        public DbSet<FieldOfStudySimilarity> FieldOfStudySimilarities { get; set; }
        public DbSet<JobTitle> JobTitles { get; set; }
        public DbSet<JobTitleSimilarity> JobTitleSimilarities { get; set; }
        public DbSet<JobVacancy> JobVacancies { get; set; }
        public DbSet<JobVacancyJobType> JobVacancyJobTypes { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<OwnedSkill> OwnedSkills { get; set; }
        public DbSet<Resume> Resumes { get; set; }
        public DbSet<ResumeJobType> ResumeJobTypes { get; set; }
        public DbSet<School> Schools { get; set; }
        public DbSet<SeekedJobTitle> SeekedJobTitles { get; set; }
        public DbSet<SimilarFieldOfStudyTitle> SimilarFieldOfStudyTitles { get; set; }
        public DbSet<SimilarJobTitle> SimilarJobTitles { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<UserNotification> UserNotifications { get; set; }
        public DbSet<WorkExperience> WorkExperiences { get; set; }
        public DbSet<WorkExperienceQualification> WorkExperienceQualifications { get; set; }

        public async Task SendNotificationAsync(IHubContext<SignalRHub> hubContext, Notification notification, 
            IList<User> users)
        {
            var insertedNotification = Notifications.Add(notification);
            foreach (var user in users)
            {
                UserNotifications.Add(new UserNotification
                {
                    Notification = insertedNotification.Entity,
                    User = user,
                });
                await hubContext.Clients.User(user.Id).SendAsync("receiveNotification");
            }
        }

        public async Task SendNotificationAsync(IHubContext<SignalRHub> hubContext, Notification notification,
            User user)
        {
            var insertedNotification = Notifications.Add(notification);
            UserNotifications.Add(new UserNotification
            {
                Notification = insertedNotification.Entity,
                User = user,
            });
            await hubContext.Clients.User(user.Id).SendAsync("receiveNotification");
        }

        public List<ClientNotification> GetNotifications(User user)
        {
            return UserNotifications
                .Where(userNotification => userNotification.UserId == user.Id)
                .OrderByDescending(userNotification => userNotification.Notification.CreatedAt)
                .Take(20)
                .Include(userNotification => userNotification.Notification)
                .Select(userNotification => new ClientNotification(userNotification))
                .ToList();
        }

        public int GetNotificationsCount(User user)
        {
            return UserNotifications
                .Count(userNotification => userNotification.UserId == user.Id && !userNotification.IsRead);
        }
    }

}
