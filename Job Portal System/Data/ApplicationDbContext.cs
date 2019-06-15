using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Job_Portal_System.Client;
using Job_Portal_System.Models;
using Job_Portal_System.SignalR;
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
        public DbSet<State> States { get; set; }
        public DbSet<GeoDistance> GeoDistances { get; set; }
        public DbSet<EducationQualification> EducationQualifications { get; set; }
        public DbSet<FieldOfStudy> FieldOfStudies { get; set; }
        public DbSet<FieldOfStudySynset> FieldOfStudySynsets { get; set; }
        public DbSet<JobTitle> JobTitles { get; set; }
        public DbSet<JobTitleSynset> JobTitleSynsets { get; set; }
        public DbSet<JobVacancy> JobVacancies { get; set; }
        public DbSet<JobVacancyJobType> JobVacancyJobTypes { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<OwnedSkill> OwnedSkills { get; set; }
        public DbSet<Resume> Resumes { get; set; }
        public DbSet<ResumeJobType> ResumeJobTypes { get; set; }
        public DbSet<School> Schools { get; set; }
        public DbSet<SeekedJobTitle> SeekedJobTitles { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<SkillSynset> SkillSynsets { get; set; }
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

        public bool HasUnsavedChanges()
        {
            return ChangeTracker.Entries()
                .Any(e => e.State == EntityState.Added
                     || e.State == EntityState.Modified
                     || e.State == EntityState.Deleted);
        }
    }
}
