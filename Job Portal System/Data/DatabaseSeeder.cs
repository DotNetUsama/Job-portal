using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using GeoCoordinatePortable;
using Job_Portal_System.Enums;
using Job_Portal_System.Models;
using Job_Portal_System.Utilities.Semantic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;

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
            UserManager<User> userManager, int count, 
            int maxRandomUsersNum = 5000)
        {
            var randomUsers = new List<User>();
            var j = 0;

            for (var i = 0; i < count; i++)
            {
                if (j == randomUsers.Count)
                {
                    randomUsers = RandomUsers(context, Math.Min(maxRandomUsersNum, count - i));
                    j = 0;
                }
                var user = randomUsers[j++];
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
                    Educations = RandomGenerator.RandomEducations(context, user.BirthDate),
                    WorkExperiences = RandomGenerator.RandomWorkExperiences(context, user.BirthDate),
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

        public static void FixDatabase(ApplicationDbContext context, IHostingEnvironment env)
        {
            //FixJobTitles(context, env);
            //FixFieldsOfStudy(context, env);
            FixSkills(context, env);
        }

        private static void FixJobTitles(ApplicationDbContext context, IHostingEnvironment env)
        {
            var count = context.JobTitles.Count();
            for (var i = 0; i < count; i++)
            {
                var jobTitle = context.JobTitles.Skip(i).First();
                if (jobTitle.JobTitleSynset != null) continue;

                var synset = new JobTitleSynset();
                jobTitle.JobTitleSynset = synset;
                var normalizedTitle = jobTitle.NormalizedTitle;
                var similarities = SimilaritiesOperator.GetSimilarities(normalizedTitle, env);
                foreach (var similarity in similarities)
                {
                    var similarityInDb = context.JobTitles
                        .FirstOrDefault(j => j.NormalizedTitle == similarity);
                    if (similarityInDb != null)
                    {
                        similarityInDb.JobTitleSynset = synset;
                    }
                }

                context.SaveChanges();
            }
        }

        private static void FixFieldsOfStudy(ApplicationDbContext context, IHostingEnvironment env)
        {
            var count = context.FieldOfStudies.Count();
            for (var i = 0; i < count; i++)
            {
                var fieldOfStudy = context.FieldOfStudies.Skip(i).First();
                if (fieldOfStudy.FieldOfStudySynset != null) continue;

                var synset = new FieldOfStudySynset();
                fieldOfStudy.FieldOfStudySynset = synset;
                var normalizedTitle = fieldOfStudy.NormalizedTitle;
                var similarities = SimilaritiesOperator.GetSimilarities(normalizedTitle, env);
                foreach (var similarity in similarities)
                {
                    var similarityInDb = context.FieldOfStudies
                        .FirstOrDefault(j => j.NormalizedTitle == similarity);
                    if (similarityInDb != null)
                    {
                        similarityInDb.FieldOfStudySynset = synset;
                    }
                }

                context.SaveChanges();
            }
        }

        private static void FixSkills(ApplicationDbContext context, IHostingEnvironment env)
        {
            var count = context.Skills.Count();
            for (var i = 333; i < count; i++)
            {
                var skill = context.Skills.Skip(i).First();
                if (skill.SkillSynset != null) continue;

                var synset = new SkillSynset();
                skill.SkillSynset = synset;
                var normalizedTitle = skill.NormalizedTitle.Split(" - ").Last();
                skill.NormalizedTitle = normalizedTitle;
                var similarities = SimilaritiesOperator.GetSimilarities(normalizedTitle, env);
                foreach (var similarity in similarities)
                {
                    var similarityInDb = context.Skills
                        .FirstOrDefault(j => j.NormalizedTitle == similarity);
                    if (similarityInDb != null)
                    {
                        similarityInDb.SkillSynset = synset;
                    }
                }

                context.SaveChanges();
            }
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

        private static List<User> RandomUsers(ApplicationDbContext context, int count)
        {
            var citiesIds = context.Cities.Select(c => c.Id);
            var citiesCount = citiesIds.Count();

            using (var client = new WebClient())
            {
                var json = client.DownloadString($"https://randomuser.me/api/?results={count}&inc=gender,name,email,dob,picture,phone");
                var obj = JsonConvert.DeserializeAnonymousType(json, new
                {
                    results = new[]
                    {
                        new
                        {
                            gender = string.Empty,
                            name = new
                            {
                                first = string.Empty,
                                last = string.Empty,
                            },
                            email = string.Empty,
                            dob = new
                            {
                                age = 0,
                            },
                            phone = string.Empty,
                            picture = new
                            {
                                large = string.Empty,
                            }
                        }
                    },
                });

                return obj.results.Select(user => new User
                {
                    FirstName = user.name.first,
                    LastName = user.name.last,
                    Email = user.email,
                    UserName = user.email,
                    PhoneNumber = user.phone,
                    Gender = user.gender == "male" ? (byte) GenderType.Male : (byte) GenderType.Female,
                    BirthDate = DateTime.Now.AddYears(-user.dob.age),
                    Image = user.picture.large,
                    CityId = citiesIds.Random(citiesCount),
                }).ToList();
            }
        }
    }
}
