using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Job_Portal_System.Data;
using Job_Portal_System.Enums;
using Job_Portal_System.Managers;
using Job_Portal_System.Models;
using Job_Portal_System.Utilities.RankingSystem;
using Job_Portal_System.SignalR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

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

            if (company.Approved)
            {
                await context.SendNotificationAsync(hubContext, new Notification
                {
                    Type = (int)NotificationType.AccountApproved,
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

            jobVacancy.Status = (int)JobVacancyStatus.InAction;
            jobVacancy.AwaitingApplicants =
                applicants.Count(a => a.Status == (int)ApplicantStatus.AcceptedByRecruiter);

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

        public static async Task Recommend(ApplicationDbContext context,
            IHubContext<SignalRHub> hubContext, JobVacancy jobVacancy)
        {
            var fetchedResumes = FetchMatchingResumes(context, jobVacancy);
            var recommendedResumes = Operator.GetRecommendedResumes(fetchedResumes, jobVacancy);
            recommendedResumes.ForEach(async r =>
            {
                var applicant = context.Applicants.Add(new Applicant
                {
                    Resume = r,
                    JobSeeker = r.User,
                    JobVacancy = jobVacancy,
                    Recruiter = jobVacancy.User,
                    Status = (int)ApplicantStatus.PendingRecommendation,
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
                    Type = (int)NotificationType.CancelledJobVacancy,
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
                    Type = (int)NotificationType.CancelledApplicant,
                    Peer1 = $"{applicant.JobSeeker.FirstName} {applicant.JobSeeker.LastName}",
                    Peer2 = jobVacancy.Title,
                    EntityId = jobVacancy.Id,
                }, jobVacancy.User);

            context.Applicants.Remove(applicant);
            await context.SaveChangesAsync();
        }

        private static List<EvaluatedResume> FetchMatchingResumes(ApplicationDbContext context,
            JobVacancy jobVacancy)
        {
            var jobVacancyCityId = context.CompanyDepartments
                .SingleOrDefault(d => d.Id == jobVacancy.CompanyDepartmentId)?
                .CityId;

            var requiredEducations = jobVacancy.EducationQualifications
                .Where(q => q.Type == (int)QualificationType.Required)
                .Select(q => q.FieldOfStudy.FieldOfStudySynsetId)
                .ToList();
            var requiredSkills = jobVacancy.DesiredSkills
                .Where(q => q.Type == (int)QualificationType.Required)
                .Select(q => q.Skill.SkillSynsetId)
                .ToList();
            var requiredWorks = jobVacancy.WorkExperienceQualifications
                .Where(q => q.Type == (int)QualificationType.Required)
                .Select(q => q.JobTitle.JobTitleSynsetId)
                .ToList();

            var resumesIds = context.Resumes
                .Where(r =>
                    r.IsSeeking &&
                    r.MinSalary <= jobVacancy.MaxSalary && r.MinSalary >= jobVacancy.MinSalary)
                .Select(r => r.Id)
                .ToList();
            resumesIds = requiredEducations
                .Aggregate(resumesIds, (current, education) => current.Intersect(context.Educations
                    .Where(e => e.FieldOfStudy.FieldOfStudySynsetId == education)
                    .Select(e => e.ResumeId)
                    .ToList())
                .ToList());

            resumesIds = requiredSkills
                .Aggregate(resumesIds, (current, skill) => current.Intersect(context.OwnedSkills
                    .Where(e => e.Skill.SkillSynsetId == skill)
                    .Select(e => e.ResumeId)
                    .ToList())
                .ToList());

            resumesIds = requiredWorks
                .Aggregate(resumesIds, (current, work) => current.Intersect(context.WorkExperiences
                    .Where(e => e.JobTitleId == work)
                    .Select(e => e.ResumeId)
                    .ToList())
                .ToList());

            var jobVacancyJobTypes = jobVacancy.JobTypes.Select(t => t.JobType);
            return resumesIds
                .Select(id => context.Resumes
                    .Include(r => r.WorkExperiences).ThenInclude(w => w.JobTitle)
                    .Include(r => r.Educations).ThenInclude(e => e.FieldOfStudy)
                    .Include(r => r.OwnedSkills).ThenInclude(s => s.Skill)
                    .Include(r => r.JobTypes)
                    .Include(r => r.User)
                    .FirstOrDefault(r => r.Id == id))
                .Where(r =>
                    r != null &&
                    (
                        jobVacancy.DistanceLimit == 0 ||
                        context.GeoDistances.First(d =>
                                d.City1Id == r.User.CityId && d.City2Id == jobVacancyCityId ||
                                d.City1Id == r.User.CityId && d.City2Id == jobVacancyCityId)
                            .Distance <= jobVacancy.DistanceLimit
                    ) &&
                    r.JobTypes
                        .Select(type => type.JobType)
                        .Intersect(jobVacancyJobTypes)
                        .Any())
                .Select(r => new EvaluatedResume { Resume = r }).ToList();
        }
    }
}
