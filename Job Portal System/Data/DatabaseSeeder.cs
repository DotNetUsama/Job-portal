﻿using System;
using System.IO;
using System.Linq;
using GeoCoordinatePortable;
using Job_Portal_System.Enums;
using Job_Portal_System.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;

namespace Job_Portal_System.Data
{
    public static class DatabaseSeeder
    {
        public static void SeedData(IHostingEnvironment env, ApplicationDbContext context,
            UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            SeedRoles(roleManager);
            SeedUsers(userManager);
            SeedJobTitles(env, context);
            SeedFieldsOfStudy(env, context);
            SeedSkills(env, context);
            SeedStates(env, context);
            SeedCities(env, context);
            SeedCompanies(env, context);
            SeedSchools(env, context);
        }

        public static void SeedJobSeekers(ApplicationDbContext context,
            UserManager<User> userManager, int count)
        {
            var genders = Enum.GetValues(typeof(GenderType));

            var startBirthDates = new DateTime(1970, 1, 1);
            var endBirthDates = new DateTime(2001, 1, 1);
            for (var i = 0; i < count; i++)
            {
                var email = RandomGenerator.RandomEmail();
                var birthDate = RandomGenerator.RandomDate(startBirthDates, endBirthDates, 17);
                if (!birthDate.HasValue) continue;
                var user = new User
                {
                    UserName = email,
                    Email = email,
                    FirstName = RandomGenerator.RandomString(5),
                    LastName = RandomGenerator.RandomString(5),
                    Gender = (byte)(GenderType)genders.GetValue(RandomGenerator.RandomNumber(genders.Length)),
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
                    IsSeeking = true,
                    MovingDistanceLimit = (uint)RandomGenerator.RandomNumber(5, 60),
                    MinSalary = RandomGenerator.RandomNumber(15, 100) * 1000,
                    Educations = RandomGenerator.RandomEducations(context, birthDate.Value),
                    WorkExperiences = RandomGenerator.RandomWorkExperiences(context, birthDate.Value),
                    OwnedSkills = RandomGenerator.RandomSkills(context),
                    JobTypes = RandomGenerator.RandomJobTypes(),
                    SeekedJobTitles = RandomGenerator.RandomSeekedJobTitles(context),
                });
                if (i % 100 == 0) context.SaveChanges();
            }
            context.SaveChanges();
        }

        public static void ClearDatabase(ApplicationDbContext context)
        {
            context.Educations.RemoveAll();
            context.WorkExperiences.RemoveAll();
            context.OwnedSkills.RemoveAll();
            context.ResumeJobTypes.RemoveAll();
            context.SeekedJobTitles.RemoveAll();
            context.EducationQualifications.RemoveAll();
            context.WorkExperienceQualifications.RemoveAll();
            context.DesiredSkills.RemoveAll();
            context.JobVacancyJobTypes.RemoveAll();
            context.Applicants.RemoveAll();
            context.UserNotifications.RemoveAll();
            if (context.HasUnsavedChanges()) context.SaveChanges();

            context.JobVacancies.RemoveAll();
            context.Resumes.RemoveAll();
            context.Notifications.RemoveAll();
            if (context.HasUnsavedChanges()) context.SaveChanges();

            context.JobSeekers.RemoveAll();
            context.Recruiters.RemoveAll();
            if (context.HasUnsavedChanges()) context.SaveChanges();

            context.Users.RemoveAll();
            if (context.HasUnsavedChanges()) context.SaveChanges();

            context.Roles.RemoveAll();
            if (context.HasUnsavedChanges()) context.SaveChanges();
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
            roleManager.CreateAsync(role).Wait();
        }

        private static void SeedJobTitles(IHostingEnvironment env, ApplicationDbContext context)
        {
            var path = Path.Combine(env.ContentRootPath, "Queries", "JobTitles.txt");
            string title;
            var file = new StreamReader(path);
            while ((title = file.ReadLine()) != null)
            {
                if (context.JobTitles.SingleOrDefault(s => s.Title == title) == null)
                {
                    context.JobTitles.Add(new JobTitle
                    {
                        Title = title,
                        NormalizedTitle = title.ToLower(),
                    });
                }
            }

            context.SaveChanges();
        }

        private static void SeedFieldsOfStudy(IHostingEnvironment env, ApplicationDbContext context)
        {
            var path = Path.Combine(env.ContentRootPath, "Queries", "FieldsOfStudy.txt");
            string fieldTitle;
            var file = new StreamReader(path);
            while ((fieldTitle = file.ReadLine()) != null)
            {
                if (context.FieldOfStudies.SingleOrDefault(s => s.Title == fieldTitle) == null)
                {
                    context.FieldOfStudies.Add(new FieldOfStudy
                    {
                        Title = fieldTitle,
                        NormalizedTitle = fieldTitle.ToLower(),
                    });
                }
            }

            context.SaveChanges();
        }

        private static void SeedSkills(IHostingEnvironment env, ApplicationDbContext context)
        {
            var path = Path.Combine(env.ContentRootPath, "Queries", "Skills.txt");
            string title;
            var file = new StreamReader(path);
            while ((title = file.ReadLine()) != null)
            {
                if (context.Skills.SingleOrDefault(s => s.Title == title) == null)
                {
                    context.Skills.Add(new Skill
                    {
                        Title = title,
                        NormalizedTitle = title.ToLower(),
                    });
                }
            }

            context.SaveChanges();
        }

        private static void SeedStates(IHostingEnvironment env, ApplicationDbContext context)
        {
            var path = Path.Combine(env.ContentRootPath, "Queries", "Syrian-States.txt");
            string line;
            var file = new StreamReader(path);
            while ((line = file.ReadLine()) != null)
            {
                if (context.States.SingleOrDefault(s => s.Name == line) == null)
                {
                    context.States.Add(new State { Name = line });
                }
            }

            context.SaveChanges();
        }

        private static void SeedCities(IHostingEnvironment env, ApplicationDbContext context)
        {
            var path = Path.Combine(env.ContentRootPath, "Queries", "Syrian-Cities.txt");
            string line;
            var file = new StreamReader(path);
            while ((line = file.ReadLine()) != null)
            {
                var cityName = line;
                var latitude = double.Parse(file.ReadLine());
                var longitude = double.Parse(file.ReadLine());
                var stateName = file.ReadLine();
                var state = context.States.SingleOrDefault(s => s.Name == stateName);
                var city = context.Cities.SingleOrDefault(c => c.Name == cityName && c.StateId == state.Id);

                if (city != null) continue;

                var otherCities = context.Cities.ToList();
                var cityId = context.Cities.Add(new City
                {
                    Name = cityName,
                    Latitude = latitude,
                    Longitude = longitude,
                    StateId = state?.Id,
                }).Entity.Id;

                var cityCoord = new GeoCoordinate(latitude, longitude);
                otherCities.ForEach(other =>
                {
                    var otherCoord = new GeoCoordinate(other.Latitude, other.Longitude);
                    var distance = (uint)cityCoord.GetDistanceTo(otherCoord) / 1000;
                    if (distance > 60) return;
                    context.GeoDistances.Add(new GeoDistance
                    {
                        City1Id = cityId,
                        City2Id = other.Id,
                        Distance = distance,
                    });
                });

                context.SaveChanges();
            }
        }

        private static void SeedCompanies(IHostingEnvironment env, ApplicationDbContext context)
        {
            var path = Path.Combine(env.ContentRootPath, "Queries", "Syrian-Companies.txt");

            string line;
            var file = new StreamReader(path);
            const string separator = ">$.$>";

            while ((line = file.ReadLine()) != null)
            {
                var companyInfo = line.Split(separator);
                var name = companyInfo[0];
                var website = companyInfo[1];
                var description = companyInfo[2];
                var logo = companyInfo[3];
                var type = companyInfo[4];
                var foundedYear = companyInfo[5];
                var employeesNum = companyInfo[6];
                var email = companyInfo[7];
                var phone = companyInfo[8];

                context.Companies.Add(new Company
                {
                    Name = name,
                    Website = website,
                    Description = description,
                    Logo = logo,
                    Type = type,
                    FoundedYear = string.IsNullOrEmpty(foundedYear) ? null : (int?)int.Parse(foundedYear),
                    EmployeesNum = string.IsNullOrEmpty(employeesNum) ? null : (int?)int.Parse(employeesNum),
                    Email = email,
                    PhoneNumber = phone,
                    Approved = true,
                });
            }

            context.SaveChanges();
        }

        private static void SeedSchools(IHostingEnvironment env, ApplicationDbContext context)
        {
            var path = Path.Combine(env.ContentRootPath, "Queries", "Universities.txt");
            string line;
            var file = new StreamReader(path);
            while ((line = file.ReadLine()) != null)
            {
                var schoolName = line;
                var country = file.ReadLine();

                context.Schools.Add(new School
                {
                    Name = schoolName,
                    Country = country,
                });
            }

            context.SaveChanges();
        }
    }
}
