using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Job_Portal_System.Client;
using Job_Portal_System.Enums;
using Job_Portal_System.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using VDS.RDF.Query;

namespace Job_Portal_System.Data
{
    public static class DatabaseSeeder
    {

        public static void SeedData (IHostingEnvironment env,
            ApplicationDbContext context, 
            UserManager<User> userManager, 
            RoleManager<IdentityRole> roleManager)
        {
            SeedRoles(roleManager);
            SeedUsers(userManager);
            SeedJobTitles(env, context);
            SeedFieldsOfStudy(env, context);
        }

        public static void ClearDatabase(ApplicationDbContext context)
        {
            foreach (var id in context.Educations.Select(e => e.Id))
            {
                var entity = new Education { Id = id };
                context.Educations.Attach(entity);
                context.Educations.Remove(entity);
            }
            foreach (var id in context.WorkExperiences.Select(e => e.Id))
            {
                var entity = new WorkExperience { Id = id };
                context.WorkExperiences.Attach(entity);
                context.WorkExperiences.Remove(entity);
            }
            foreach (var id in context.OwnedSkills.Select(e => e.Id))
            {
                var entity = new OwnedSkill { Id = id };
                context.OwnedSkills.Attach(entity);
                context.OwnedSkills.Remove(entity);
            }
            foreach (var id in context.ResumeJobTypes.Select(e => e.Id))
            {
                var entity = new ResumeJobType { Id = id };
                context.ResumeJobTypes.Attach(entity);
                context.ResumeJobTypes.Remove(entity);
            }
            foreach (var id in context.SeekedJobTitles.Select(e => e.Id))
            {
                var entity = new SeekedJobTitle { Id = id };
                context.SeekedJobTitles.Attach(entity);
                context.SeekedJobTitles.Remove(entity);
            }

            foreach (var id in context.EducationQualifications.Select(e => e.Id))
            {
                var entity = new EducationQualification { Id = id };
                context.EducationQualifications.Attach(entity);
                context.EducationQualifications.Remove(entity);
            }
            foreach (var id in context.WorkExperienceQualifications.Select(e => e.Id))
            {
                var entity = new WorkExperienceQualification { Id = id };
                context.WorkExperienceQualifications.Attach(entity);
                context.WorkExperienceQualifications.Remove(entity);
            }
            foreach (var id in context.DesiredSkills.Select(e => e.Id))
            {
                var entity = new DesiredSkill { Id = id };
                context.DesiredSkills.Attach(entity);
                context.DesiredSkills.Remove(entity);
            }
            foreach (var id in context.JobVacancyJobTypes.Select(e => e.Id))
            {
                var entity = new JobVacancyJobType { Id = id };
                context.JobVacancyJobTypes.Attach(entity);
                context.JobVacancyJobTypes.Remove(entity);
            }
            foreach (var id in context.Applicants.Select(a => a.Id))
            {
                var entity = new Applicant { Id = id };
                context.Applicants.Attach(entity);
                context.Applicants.Remove(entity);
            }
            foreach (var id in context.UserNotifications.Select(a => a.Id))
            {
                var entity = new UserNotification { Id = id };
                context.UserNotifications.Attach(entity);
                context.UserNotifications.Remove(entity);
            }
            context.SaveChanges();
            foreach (var id in context.JobVacancies.Select(e => e.Id))
            {
                var entity = new JobVacancy { Id = id };
                context.JobVacancies.Attach(entity);
                context.JobVacancies.Remove(entity);
            }
            foreach (var id in context.Resumes.Select(e => e.Id))
            {
                var entity = new Resume { Id = id };
                context.Resumes.Attach(entity);
                context.Resumes.Remove(entity);
            }
            foreach (var id in context.Notifications.Select(a => a.Id))
            {
                var entity = new Notification { Id = id };
                context.Notifications.Attach(entity);
                context.Notifications.Remove(entity);
            }
            context.SaveChanges();
        }

        private static void SeedUsers (UserManager<User> userManager)
        {
            if (userManager.FindByNameAsync("Usama").Result != null) return;
            var user = new User
            {
                UserName = "usamaghannam@gmail.com",
                Email = "usamaghannam@gmail.com",
                FirstName = "Usama",
                LastName = "Ghannam",
                Gender = (int)GenderType.Male,
                BirthDate = new DateTime(1995, 2, 19),
            };

            var result = userManager.CreateAsync(user, "Tom&Jerry123");

            if (result.Result.Succeeded)
            {
                userManager.AddToRoleAsync(user, "Administrator").Wait();
            }
        }

        private static void SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            var roles = new []
            {
                "Administrator",
                "JobSeeker",
                "Recruiter",
                "PendingRecruiter",
                "RejectedRecruiter",
            };
            foreach (var role in roles)
            {
                SeedRole(roleManager, role);
            }
        }

        private static void SeedRole(RoleManager<IdentityRole> roleManager, string roleName)
        {
            if (roleManager.RoleExistsAsync(roleName).Result) return;
            var role = new IdentityRole
            {
                Name = roleName
            };
            var roleResult = roleManager.CreateAsync(role).Result;
        }

        private static void SeedJobTitles(IHostingEnvironment env,
            ApplicationDbContext context)
        {
            var path = Path.Combine(env.ContentRootPath, "Queries", "JobTitlesQuery.txt");
            var jobTitles = NamedEntity.GetNamedEntities(path)
                .Select(entity => new JobTitle
                {
                    Id = entity.Id,
                    Title = entity.Label,
                })
                .Where(entity => !context.JobTitles.Any(jobTitle => jobTitle.Title == entity.Title))
                .ToList();
            context.JobTitles.AddRange(jobTitles);
            context.SaveChanges();
        }

        private static void SeedFieldsOfStudy(IHostingEnvironment env, 
            ApplicationDbContext context)
        {
            var path = Path.Combine(env.ContentRootPath, "Queries", "FieldsOfStudyQuery.txt");
            var fieldsOfStudy = NamedEntity.GetNamedEntities(path)
                .Select(entity => new FieldOfStudy
                {
                    Id = entity.Id,
                    Title = entity.Label,
                })
                .Where(entity => !context.FieldOfStudies.Any(fieldOfStudy => fieldOfStudy.Title == entity.Title))
                .ToList();
            context.FieldOfStudies.AddRange(fieldsOfStudy);
            context.SaveChanges();
        }
    }
}
