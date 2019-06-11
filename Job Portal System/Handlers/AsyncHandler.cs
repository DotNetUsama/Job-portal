using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Job_Portal_System.Data;
using Job_Portal_System.Enums;
using Job_Portal_System.Managers;
using Job_Portal_System.Models;
using Job_Portal_System.Utilities.RankingSystem;
using Job_Portal_System.Utilities.RankingSystem.Helpers;
using Job_Portal_System.SignalR;
using Job_Portal_System.Utilities.Semantic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Job_Portal_System.Handlers
{
    public class AsyncHandler
    {
        public static async Task RequestRecruiterAccountApproval(ApplicationDbContext context,
            IHubContext<SignalRHub> hubContext, UserManager<User> userManager, Recruiter recruiter)
        {
            var administrators = await userManager.GetUsersInRoleAsync("Administrator");

            await context.SendNotificationAsync(hubContext, new Notification
            {
                Type = (int)NotificationType.RecruiterApproval,
                EntityId = recruiter.Id,
                Peer1 = recruiter.User.FirstName
            }, administrators);
        }

        public static async Task ApproveRecruiterAsync(ApplicationDbContext context,
            IHubContext<SignalRHub> hubContext, UserManager<User> userManager, Recruiter recruiter)
        {
            var user = recruiter.User;
            var company = recruiter.Company;
            await userManager.RemoveFromRoleAsync(user, "PendingRecruiter");
            await userManager.AddToRoleAsync(user, "Recruiter");

            if (company.Approved) { 
                await context.SendNotificationAsync(hubContext, new Notification
                {
                    Type = (int) NotificationType.AccountApproved,
                }, user);
            }
            else
            {
                await context.SendNotificationAsync(hubContext, new Notification
                {
                    Type = (int)NotificationType.AccountApprovedEditCompanyInfo,
                    EntityId = company.Id,
                }, user);
            }
            await context.SaveChangesAsync();
        }

        public static async Task RejectRecruiterAsync(ApplicationDbContext context,
            IHubContext<SignalRHub> hubContext, UserManager<User> userManager, Recruiter recruiter)
        {
            var user = recruiter.User;
            await userManager.RemoveFromRoleAsync(user, "PendingRecruiter");
            await userManager.AddToRoleAsync(user, "RejectedRecruiter");

            await context.SendNotificationAsync(hubContext, new Notification
            {
                Type = (int)NotificationType.AccountRejected,
            }, user);

            await context.SaveChangesAsync();
        }

        public static async Task SubmitToJobVacancy(ApplicationDbContext context,
            IHubContext<SignalRHub> hubContext, JobVacancy jobVacancy, Resume resume)
        {
            var applicant = context.Applicants.Add(new Applicant
            {
                Recruiter = jobVacancy.User,
                JobVacancy = jobVacancy,
                JobSeeker = resume.User,
                Resume = resume,
            });
            jobVacancy.AwaitingApplicants++;

            await context.SendNotificationAsync(hubContext, new Notification
            {
                Type = (int)NotificationType.ReceivedSubmission,
                Peer1 = $"{resume.User.FirstName} {resume.User.LastName}",
                Peer2 = jobVacancy.Title,
                EntityId = applicant.Entity.Id,
            }, applicant.Entity.Recruiter);

            await context.SaveChangesAsync();
        }

        public static async Task FinalDecideOnApplicants(IHostingEnvironment env, 
            ApplicationDbContext context, IHubContext<SignalRHub> hubContext, JobVacancy jobVacancy)
        {
            var applicants = context.Applicants
                .Where(a => a.JobVacancyId == jobVacancy.Id)
                .ToList();
            applicants.ForEach(a => a.Status = (int)((ApplicantStatus)a.Status).GetFinal());
            
            var evaluatedResumes = applicants
                .Select(a => new EvaluatedResume
                {
                    Applicant = a,
                    Resume = context.Resumes
                        .Include(r => r.Educations)
                        .Include(r => r.WorkExperiences)
                        .Include(r => r.OwnedSkills)
                        .Include(r => r.User)
                        .SingleOrDefault(r => r.Id == a.ResumeId),
                    Accepted = ((ApplicantStatus)a.Status).IsAccepted(),
                })
                .ToList();

            Operator.CalculateAndNormalizeEvaluations(evaluatedResumes, jobVacancy);
            evaluatedResumes.ForEach(r => FilesManager.Store(env, "Evaluations", r.Applicant.Id, r.Evaluation));

            jobVacancy.Status = (int) JobVacancyStatus.InAction;

            evaluatedResumes.ForEach(async evaluatedResume =>
                await context.SendNotificationAsync(hubContext, new Notification
                {
                    Type = (int)(evaluatedResume.Accepted ?
                        NotificationType.ApplicantAccepted :
                        NotificationType.ApplicantRejected),
                    Peer1 = jobVacancy.Title,
                    EntityId = evaluatedResume.Applicant.Id,
                }, evaluatedResume.Applicant.JobSeeker));

            await context.SaveChangesAsync();
        }

        public static async Task Recommend(ApplicationDbContext context, IHostingEnvironment env,
            IHubContext<SignalRHub> hubContext, JobVacancy jobVacancy)
        {
            var fetchedResumes = FetchMatchingResumes(context, env, jobVacancy);
            var recommendedResumes = Operator.GetRecommendedResumes(fetchedResumes, jobVacancy);
            recommendedResumes.ForEach(async r =>
            {
                var applicant = context.Applicants.Add(new Applicant
                {
                    Resume = r,
                    JobSeeker = r.User,
                    JobVacancy = jobVacancy,
                    Recruiter = jobVacancy.User,
                    Status = (int) ApplicantStatus.PendingRecommendation,
                }).Entity;
                await context.SendNotificationAsync(hubContext, new Notification
                {
                    Type = (int)NotificationType.ResumeRecommendation,
                    EntityId = applicant.Id,
                    Peer1 = jobVacancy.Title,
                }, r.User);
            });
            await context.SendNotificationAsync(hubContext, new Notification
            {
                Type = (int)NotificationType.FinishedRecommendation,
                EntityId = jobVacancy.Id,
                Peer1 = jobVacancy.Title,
            }, jobVacancy.User);

            await context.SaveChangesAsync();
        }

        public static async Task DeleteJobVacancy(ApplicationDbContext context,
            IHubContext<SignalRHub> hubContext, JobVacancy jobVacancy)
        {
            if (jobVacancy.Status != (int)JobVacancyStatus.Finished)
            {
                var applicants = context.Applicants
                    .Include(a => a.JobSeeker)
                    .Where(a => a.JobVacancyId == jobVacancy.Id);

                var applicantsUsers = applicants.Select(a => a.JobSeeker).ToList();

                await context.SendNotificationAsync(hubContext, new Notification
                {
                    Type = (int) NotificationType.CancelledJobVacancy,
                    Peer1 = jobVacancy.Title,
                }, applicantsUsers);
                
                context.Applicants.RemoveRange(applicants);
            }
            context.JobVacancies.Remove(jobVacancy);
            await context.SaveChangesAsync();
        }

        public static async Task DeleteApplicant(ApplicationDbContext context,
            IHubContext<SignalRHub> hubContext, Applicant applicant)
        {
            var jobVacancy = context.JobVacancies
                .Include(j => j.User)
                .SingleOrDefault(j => j.Id == applicant.JobVacancyId);

            if (jobVacancy != null)
                await context.SendNotificationAsync(hubContext, new Notification
                {
                    Type = (int) NotificationType.CancelledApplicant,
                    Peer1 = $"{applicant.JobSeeker.FirstName} {applicant.JobSeeker.LastName}",
                    Peer2 = jobVacancy.Title,
                    EntityId = jobVacancy.Id,
                }, jobVacancy.User);

            context.Applicants.Remove(applicant);
            await context.SaveChangesAsync();
        }

        private static List<EvaluatedResume> FetchMatchingResumes(ApplicationDbContext context,
                        IHostingEnvironment env, JobVacancy jobVacancy)
        {
            var jobVacancyCityId = jobVacancy.CompanyDepartment.CityId;

            var similaritiesQueryPath = Path.Combine(env.ContentRootPath, "Queries", "GetSimilarities.txt");

            var jobTitlesSimilarities = GetSimilarJobTitles(context, jobVacancy, similaritiesQueryPath);
            
            var fieldsOfStudySimilarities = GetSimilarFieldsOfStudy(context, jobVacancy, similaritiesQueryPath);

            var skillsSimilarities = GetSimilarSkills(context, jobVacancy, similaritiesQueryPath);

            var resumes = context.Resumes
                .Include(r => r.Educations)
                .Include(r => r.WorkExperiences)
                .Include(r => r.OwnedSkills)
                .Include(r => r.User)
                .Where(r => r.IsSeeking &&
                            (
                                // Remote
                                jobVacancy.DistanceLimit == 0 ||
                                // Not remote (limited by geo distance)
                                (context.GeoDistances.SingleOrDefault(d => 
                                    (d.City1Id == r.User.CityId && d.City2Id == jobVacancyCityId) ||
                                    (d.City1Id == r.User.CityId && d.City2Id == jobVacancyCityId))
                                .Distance <= jobVacancy.DistanceLimit)
                            ) &&
                            r.JobTypes
                                .Select(type => type.JobType)
                                .Intersect(jobVacancy.JobTypes.Select(type => type.JobType))
                                .Any() &&
                            r.MinSalary <= jobVacancy.MaxSalary && r.MinSalary >= jobVacancy.MinSalary &&
                            jobVacancy.WorkExperienceQualifications
                                .Where(workExperienceQualification =>
                                    workExperienceQualification.Type == (int)QualificationType.Required)
                                .All(workExperienceQualification => r.WorkExperiences
                                    .Any(workExperience => 
                                        jobTitlesSimilarities.ContainsKey(workExperience.JobTitleId) &&
                                        workExperienceQualification.JobTitleId == jobTitlesSimilarities[workExperience.JobTitleId] &&
                                        HelperFunctions.GetYears(workExperience.StartDate, (workExperience.EndDate ?? DateTime.Now)) >= workExperienceQualification.MinimumYears)) &&
                            jobVacancy.EducationQualifications
                                .Where(educationQualification =>
                                    educationQualification.Type == (int)QualificationType.Required)
                                .All(educationQualification => r.Educations
                                    .Any(education =>
                                        fieldsOfStudySimilarities.ContainsKey(education.FieldOfStudyId) &&
                                        educationQualification.FieldOfStudyId == fieldsOfStudySimilarities[education.FieldOfStudyId] &&
                                        HelperFunctions.GetYears(education.StartDate, (education.EndDate ?? DateTime.Now)) >= educationQualification.MinimumYears)) &&
                            jobVacancy.DesiredSkills
                                .Where(desiredSkill =>
                                    desiredSkill.Type == (int)QualificationType.Required)
                                .All(desiredSkill => r.OwnedSkills
                                    .Any(ownedSkill =>
                                        skillsSimilarities.ContainsKey(ownedSkill.SkillId) &&
                                        desiredSkill.SkillId == skillsSimilarities[ownedSkill.SkillId] &&
                                        ownedSkill.Years >= desiredSkill.MinimumYears)));

            foreach (var resume in resumes)
            {
                foreach (var workExperience in resume.WorkExperiences)
                {
                    if (jobTitlesSimilarities.ContainsKey(workExperience.JobTitleId))
                    {
                        workExperience.JobTitleId = jobTitlesSimilarities[workExperience.JobTitleId];
                    }
                }
                foreach (var education in resume.Educations)
                {
                    if (jobTitlesSimilarities.ContainsKey(education.FieldOfStudyId))
                    {
                        education.FieldOfStudyId = jobTitlesSimilarities[education.FieldOfStudyId];
                    }
                }
                foreach (var ownedSkill in resume.OwnedSkills)
                {
                    if (jobTitlesSimilarities.ContainsKey(ownedSkill.SkillId))
                    {
                        ownedSkill.SkillId = jobTitlesSimilarities[ownedSkill.SkillId];
                    }
                }

                context.Entry(resume).State = EntityState.Unchanged;
            }

            return resumes.Select(r => new EvaluatedResume {Resume = r}).ToList();
        }

        private static Dictionary<long, long> GetSimilarJobTitles(ApplicationDbContext context, 
            JobVacancy jobVacancy, string similaritiesQueryPath)
        {
            var jobTitles = jobVacancy.WorkExperienceQualifications
                .Select(q => q.JobTitle);

            var jobTitlesSimilarities = new Dictionary<long, long>();

            foreach (var jobTitle in jobTitles)
            {
                var similarities = SimilaritiesOperator
                    .GetSimilarities(jobTitle.NormalizedTitle, similaritiesQueryPath);
                foreach (var similarity in similarities)
                {
                    var similarityInDb = context.JobTitles
                        .SingleOrDefault(j => j.NormalizedTitle == similarity);
                    if (similarityInDb != null &&
                        !jobTitlesSimilarities.ContainsKey(similarityInDb.Id))
                        jobTitlesSimilarities.Add(similarityInDb.Id, jobTitle.Id);
                }
            }

            return jobTitlesSimilarities;
        }

        private static Dictionary<long, long> GetSimilarFieldsOfStudy(ApplicationDbContext context,
            JobVacancy jobVacancy, string similaritiesQueryPath)
        {
            var fieldsOfStudy = jobVacancy.EducationQualifications
                .Select(q => q.FieldOfStudy);

            var fieldsOfStudySimilarities = new Dictionary<long, long>();

            foreach (var fieldOfStudy in fieldsOfStudy)
            {
                var similarities = SimilaritiesOperator
                    .GetSimilarities(fieldOfStudy.NormalizedTitle, similaritiesQueryPath);
                foreach (var similarity in similarities)
                {
                    var similarityInDb = context.FieldOfStudies
                        .SingleOrDefault(f => f.NormalizedTitle == similarity);
                    if (similarityInDb != null &&
                        !fieldsOfStudySimilarities.ContainsKey(similarityInDb.Id))
                        fieldsOfStudySimilarities.Add(similarityInDb.Id, fieldOfStudy.Id);
                }
            }

            return fieldsOfStudySimilarities;
        }

        private static Dictionary<long, long> GetSimilarSkills(ApplicationDbContext context,
            JobVacancy jobVacancy, string similaritiesQueryPath)
        {
            var skills = jobVacancy.DesiredSkills
                .Select(q => q.Skill);

            var skillsSimilarities = new Dictionary<long, long>();

            foreach (var skill in skills)
            {
                var similarities = SimilaritiesOperator
                    .GetSimilarities(skill.NormalizedTitle, similaritiesQueryPath);
                foreach (var similarity in similarities)
                {
                    var similarityInDb = context.Skills
                        .SingleOrDefault(j => j.NormalizedTitle == similarity);
                    if (similarityInDb != null &&
                        !skillsSimilarities.ContainsKey(similarityInDb.Id))
                        skillsSimilarities.Add(similarityInDb.Id, skill.Id);
                }
            }

            return skillsSimilarities;
        }
    }
}
