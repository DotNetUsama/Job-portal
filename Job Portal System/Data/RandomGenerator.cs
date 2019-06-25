using Job_Portal_System.Enums;
using Job_Portal_System.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Job_Portal_System.Data
{
    public class RandomGenerator
    {
        private static readonly Random Random = new Random();

        public static List<Education> RandomEducations(ApplicationDbContext context, 
            DateTime birthDate)
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
                var startDate = RandomDate(birthDate.AddYears(15), DateTime.Now.AddMonths(-1), 17);
                var fieldOfStudyId = fieldsOfStudiesIds.Random(fieldsOfStudiesCount);
                while (res.Any(j => j.FieldOfStudyId == fieldOfStudyId))
                {
                    fieldOfStudyId = fieldsOfStudiesIds.Random(fieldsOfStudiesCount);
                }
                res.Add(new Education
                {
                    Degree =
                            (int)(EducationDegree)educationDegrees.GetValue(Random.Next(educationDegrees.Length)),
                    StartDate = startDate,
                    EndDate = RandomDateNullable(startDate.AddMonths(1), DateTime.Now, 28),
                    SchoolId = schoolsIds.Random(schoolsCount),
                    FieldOfStudyId = fieldsOfStudiesIds.Skip(Random.Next(0, fieldsOfStudiesCount)).Take(1).First(),
                });
            }

            return res;
        }

        public static List<WorkExperience> RandomWorkExperiences(ApplicationDbContext context, 
            DateTime birthDate)
        {
            var jobTitlesIds = context.JobTitles.Select(j => j.Id);
            var jobTitlesCount = jobTitlesIds.Count();
            var count = Random.Next(5);

            var companiesIds = context.Companies.Select(s => s.Id);
            var companiesCount = companiesIds.Count();

            var res = new List<WorkExperience>();
            for (var i = 0; i < count; i++)
            {
                var startDate = RandomDate(birthDate.AddYears(15), DateTime.Now.AddMonths(-1), 17);
                var jobTitleId = jobTitlesIds.Random(jobTitlesCount);
                while (res.Any(j => j.JobTitleId == jobTitleId))
                {
                    jobTitleId = jobTitlesIds.Random(jobTitlesCount);
                }
                res.Add(new WorkExperience
                {
                    StartDate = startDate,
                    EndDate = RandomDateNullable(startDate.AddMonths(1), DateTime.Now, 28),
                    CompanyId = companiesIds.Random(companiesCount),
                    JobTitleId = jobTitleId,
                });
            }

            return res;
        }

        public static List<OwnedSkill> RandomSkills(ApplicationDbContext context)
        {
            var skillsIds = context.Skills.Select(s => s.Id);
            var skillsCount = skillsIds.Count();
            var count = Random.Next(7);

            var res = new List<OwnedSkill>();
            for (var i = 0; i < count; i++)
            {
                var skillId = skillsIds.Random(skillsCount);
                while (res.Any(s => s.SkillId == skillId))
                {
                    skillId = skillsIds.Random(skillsCount);
                }
                res.Add(new OwnedSkill
                {
                    SkillId = skillId,
                    Years = Random.Next(1, 10),
                });
            }

            return res;
        }

        public static List<ResumeJobType> RandomJobTypes()
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

        public static List<SeekedJobTitle> RandomSeekedJobTitles(ApplicationDbContext context)
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

        public static int RandomNumber(int max)
        {
            return Random.Next(max);
        }

        public static int RandomNumber(int min, int max)
        {
            return Random.Next(min, max);
        }

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[Random.Next(s.Length)]).ToArray());
        }

        public static string RandomEmail()
        {
            return $"{RandomString(4)}@{RandomString(2)}.{RandomString(2)}";
        }

        public static DateTime RandomDate(DateTime start, DateTime end, int daysRange)
        {
            var days = Random.Next((end - start).Days / daysRange);
            return start.AddDays(days * daysRange);
        }

        public static DateTime? RandomDateNullable(DateTime start, DateTime end, int daysRange)
        {
            var days = Random.Next((end - start).Days / daysRange);
            return days == 0 ? (DateTime?)null : start.AddDays(days * daysRange);
        }
    }
}
