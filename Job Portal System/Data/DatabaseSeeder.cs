using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Job_Portal_System.Client;
using Job_Portal_System.Enums;
using Job_Portal_System.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;

namespace Job_Portal_System.Data
{
    public static class DatabaseSeeder
    {
        private static readonly Random Random = new Random();

        private static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[Random.Next(s.Length)]).ToArray());
        }

        private static string RandomEmail()
        {
            return $"{RandomString(4)}@{RandomString(2)}.{RandomString(2)}";
        }

        private static DateTime? RandomDate(DateTime start, DateTime end, bool isNullable = false)
        {
            var days = Random.Next((end - start).Days);
            return isNullable && days == 0 ? (DateTime?)null : start.AddDays(days);
        }

        private static List<Education> RandomEducations(ApplicationDbContext context, DateTime birthDate)
        {
            var fieldsOfStudiesIds = context.FieldOfStudies.Select(f => f.Id);
            var fieldsOfStudiesCount = fieldsOfStudiesIds.Count();
            var count = Random.Next(5);
            var educationDegrees = Enum.GetValues(typeof(EducationDegree));

            var schoolsIds = context.Schools.Select(s => s.Id);
            var schoolsCount = schoolsIds.Count();

            var res = new List<Education>();
            for (var i = 0; i < count; i++)
            {
                var startDate = RandomDate(birthDate.AddYears(15), DateTime.Now.AddMonths(-1));
                if (startDate == null) continue;
                var fieldOfStudyId = fieldsOfStudiesIds.Skip(Random.Next(0, fieldsOfStudiesCount)).Take(1).First();
                while (res.Any(j => j.FieldOfStudyId == fieldOfStudyId))
                {
                    fieldOfStudyId = fieldsOfStudiesIds.Skip(Random.Next(0, fieldsOfStudiesCount)).Take(1).First();
                }
                res.Add(new Education
                {
                    Degree =
                            (int)(EducationDegree)educationDegrees.GetValue(Random.Next(educationDegrees.Length)),
                    StartDate = startDate.Value,
                    EndDate = RandomDate(startDate.Value.AddMonths(1), DateTime.Now, true),
                    SchoolId = schoolsIds.Skip(Random.Next(0, schoolsCount)).Take(1).First(),
                    FieldOfStudyId = fieldsOfStudiesIds.Skip(Random.Next(0, fieldsOfStudiesCount)).Take(1).First(),
                });
            }

            return res;
        }

        private static List<WorkExperience> RandomWorkExperiences(ApplicationDbContext context, DateTime birthDate)
        {
            var jobTitlesIds = context.JobTitles.Select(j => j.Id);
            var jobTitlesCount = jobTitlesIds.Count();
            var count = Random.Next(5);

            var companiesIds = context.Companies.Select(s => s.Id);
            var companiesCount = companiesIds.Count();

            var res = new List<WorkExperience>();
            for (var i = 0; i < count; i++)
            {
                var startDate = RandomDate(birthDate.AddYears(15), DateTime.Now.AddMonths(-1));
                if (startDate == null) continue;
                var jobTitleId = jobTitlesIds.Skip(Random.Next(0, jobTitlesCount)).Take(1).First();
                while (res.Any(j => j.JobTitleId == jobTitleId))
                {
                    jobTitleId = jobTitlesIds.Skip(Random.Next(0, jobTitlesCount)).Take(1).First();
                }
                res.Add(new WorkExperience
                {
                    StartDate = startDate.Value,
                    EndDate = RandomDate(startDate.Value.AddMonths(1), DateTime.Now, true),
                    CompanyId = companiesIds.Skip(Random.Next(0, companiesCount)).Take(1).First(),
                    JobTitleId = jobTitleId,
                });
            }

            return res;
        }

        private static List<OwnedSkill> RandomSkills(ApplicationDbContext context)
        {
            var skillsIds = context.Skills.Select(s => s.Id);
            var skillsCount = skillsIds.Count();
            var count = Random.Next(7);

            var res = new List<OwnedSkill>();
            for (var i = 0; i < count; i++)
            {
                var skillId = skillsIds.Skip(Random.Next(0, skillsCount)).Take(1).First();
                while (res.Any(s => s.SkillId == skillId))
                {
                    skillId = skillsIds.Skip(Random.Next(0, skillsCount)).Take(1).First();
                }
                res.Add(new OwnedSkill
                {
                    SkillId = skillId,
                    Years = Random.Next(1, 10),
                });
            }

            return res;
        }

        private static List<ResumeJobType> RandomJobTypes()
        {
            var jobTypes = Enum.GetValues(typeof(JobType));
            var count = Random.Next(jobTypes.Length);

            var res = new List<ResumeJobType>();
            for (var i = 0; i < count; i++)
            {
                var jobType = (int)(JobType)jobTypes.GetValue(Random.Next(jobTypes.Length));
                while (res.Any(j => j.JobType == jobType))
                {
                    jobType = (int)(JobType)jobTypes.GetValue(Random.Next(jobTypes.Length));
                }
                res.Add(new ResumeJobType
                {
                    JobType = jobType,
                });
            }

            return res;
        }

        private static List<SeekedJobTitle> RandomJobTitles(ApplicationDbContext context)
        {
            var jobTitles = context.JobTitles;
            var jobTitlesCount = jobTitles.Count();
            var count = Random.Next(5);

            var res = new List<SeekedJobTitle>();
            for (var i = 0; i < count; i++)
            {
                var jobTitle = jobTitles.Skip(Random.Next(0, jobTitlesCount)).Take(1).First();
                while (res.Any(j => j.JobTitle.Id == jobTitle.Id))
                {
                    jobTitle = jobTitles.Skip(Random.Next(0, jobTitlesCount)).Take(1).First();
                }
                res.Add(new SeekedJobTitle()
                {
                    JobTitle = jobTitle,
                });
            }

            return res;
        }

        public static void SeedData(IHostingEnvironment env, ApplicationDbContext context,
            UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
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

        public static void SeedCompanies(ApplicationDbContext context)
        {
            var companies = new[] { "MTN", "Elixer", "LG" };
            foreach (var company in companies)
            {
                context.Companies.FindOrAdd(new Company { Name = company }, c => c.Name == company);
            }

            context.SaveChanges();
        }

        public static void SeedSkills(ApplicationDbContext context)
        {
            var skills = new[] { "SQL", "Oracle", "Laravel", "Photoshop", "Microsoft office", "Nodejs" };
            foreach (var skill in skills)
            {
                context.Skills.FindOrAdd(new Skill { Title = skill }, c => c.Title == skill);
            }

            context.SaveChanges();
        }

        public static void SeedSchools(ApplicationDbContext context)
        {
            var schools = new Dictionary<string, string>
            {
                { "Al-Basel", "Qutaifah" },
                { "Al-Awael", "Damascus" },
                { "Aleppo university", "Aleppo" },
                { "SVU", "Damascus" },
            };
            foreach (var school in schools)
            {
                context.Schools.FindOrAdd(new School
                {
                    Name = school.Key,
                    City = context.Cities.FindOrAdd(new City { Name = school.Value }, c => c.Name == school.Value),
                }, s => s.Name == school.Key);
            }

            context.SaveChanges();
        }

        public static void SeedJobSeekers(ApplicationDbContext context,
            UserManager<User> userManager, RoleManager<IdentityRole> roleManager, int count)
        {

            var startBirthDates = new DateTime(1970, 1, 1);
            var endBirthDates = new DateTime(2001, 1, 1);
            for (var i = 0; i < count; i++)
            {
                var email = RandomEmail();
                var birthDate = RandomDate(startBirthDates, endBirthDates);
                if (!birthDate.HasValue) continue;
                var user = new User
                {
                    UserName = email,
                    Email = email,
                    FirstName = RandomString(5),
                    LastName = RandomString(5),
                    Gender = (byte)Random.Next(0, 1),
                    BirthDate = birthDate.Value,
                };

                var result = userManager.CreateAsync(user, "Tom&Jerry123");

                if (result.Result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, "JobSeeker").Wait();
                }

                var jobSeeker = context.JobSeekers.Add(new JobSeeker
                {
                    User = user,
                }).Entity;

                context.Resumes.Add(new Resume
                {
                    JobSeeker = jobSeeker,
                    User = user,
                    IsPublic = true,
                    MinSalary = Random.Next(15, 100) * 1000,
                    Educations = RandomEducations(context, birthDate.Value),
                    WorkExperiences = RandomWorkExperiences(context, birthDate.Value),
                    OwnedSkills = RandomSkills(context),
                    JobTypes = RandomJobTypes(),
                    SeekedJobTitles = RandomJobTitles(context),
                });
                if (i % 100 == 0) context.SaveChanges();
            }
            context.SaveChanges();
        }

        private static void SeedUsers(UserManager<User> userManager)
        {
            if (userManager.FindByNameAsync("usamaghannam@gmail.com").Result != null) return;
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
            var roles = new[]
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

        private static void SeedJobTitles(IHostingEnvironment env, ApplicationDbContext context)
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

        private static void SeedFieldsOfStudy(IHostingEnvironment env, ApplicationDbContext context)
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
