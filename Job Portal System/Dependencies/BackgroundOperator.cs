using System.Collections.Generic;
using System.IO;
using System.Linq;
using Job_Portal_System.BackgroundTasking.Interfaces;
using Job_Portal_System.Data;
using Job_Portal_System.Enums;
using Job_Portal_System.Managers;
using Job_Portal_System.Models;
using Job_Portal_System.SignalR;
using Job_Portal_System.Utilities.RankingSystem;
using Job_Portal_System.Utilities.Semantic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Job_Portal_System.Dependencies
{
    public class BackgroundOperator : IBackgroundOperator
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public BackgroundOperator(IServiceScopeFactory serviceScopeFactory, IBackgroundTaskQueue queue)
        {
            _serviceScopeFactory = serviceScopeFactory;
            Queue = queue;
        }

        public IBackgroundTaskQueue Queue { get; }

        public void RequestRecruiterAccountApproval(Recruiter recruiter)
        {
            Queue.QueueBackgroundWorkItem(async token =>
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var context = scopedServices.GetRequiredService<ApplicationDbContext>();
                    var userManager = scopedServices.GetRequiredService<UserManager<User>>();
                    var hubContext = scopedServices.GetRequiredService<IHubContext<SignalRHub>>();

                    var administrators = await userManager.GetUsersInRoleAsync("Administrator");

                    await context.SendNotificationAsync(hubContext, new Notification
                    {
                        Type = (int)NotificationType.RecruiterApproval,
                        EntityId = recruiter.Id,
                        Peer1 = recruiter.User.FirstName
                    }, administrators);

                    await context.SaveChangesAsync(token);
                }
            });
        }

        public void ApproveRecruiter(Recruiter recruiter)
        {
            Queue.QueueBackgroundWorkItem(async token =>
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var context = scopedServices.GetRequiredService<ApplicationDbContext>();
                    var userManager = scopedServices.GetRequiredService<UserManager<User>>();
                    var hubContext = scopedServices.GetRequiredService<IHubContext<SignalRHub>>();

                    var user = recruiter.User;
                    var company = recruiter.Company;
                    await userManager.RemoveFromRoleAsync(user, "PendingRecruiter");
                    await userManager.AddToRoleAsync(user, "Recruiter");

                    if (company.Approved)
                    {
                        await context.SendNotificationAsync(hubContext, new Notification
                        {
                            Type = (int) NotificationType.AccountApproved,
                        }, user);
                    }
                    else
                    {
                        await context.SendNotificationAsync(hubContext, new Notification
                        {
                            Type = (int) NotificationType.AccountApprovedEditCompanyInfo,
                            EntityId = company.Id,
                        }, user);
                    }

                    await context.SaveChangesAsync(token);
                }
            });
        }

        public void RejectRecruiter(Recruiter recruiter)
        {
            Queue.QueueBackgroundWorkItem(async token =>
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var context = scopedServices.GetRequiredService<ApplicationDbContext>();
                    var userManager = scopedServices.GetRequiredService<UserManager<User>>();
                    var hubContext = scopedServices.GetRequiredService<IHubContext<SignalRHub>>();

                    var user = recruiter.User;
                    await userManager.RemoveFromRoleAsync(user, "PendingRecruiter");
                    await userManager.AddToRoleAsync(user, "RejectedRecruiter");

                    await context.SendNotificationAsync(hubContext, new Notification
                    {
                        Type = (int) NotificationType.AccountRejected,
                    }, user);

                    await context.SaveChangesAsync(token);
                }
            });
        }

        public void SubmitToJobVacancy(JobVacancy jobVacancy, Resume resume)
        {
            Queue.QueueBackgroundWorkItem(async token =>
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var context = scopedServices.GetRequiredService<ApplicationDbContext>();
                    var hubContext = scopedServices.GetRequiredService<IHubContext<SignalRHub>>();

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
                        Type = (int) NotificationType.ReceivedSubmission,
                        Peer1 = $"{resume.User.FirstName} {resume.User.LastName}",
                        Peer2 = jobVacancy.Title,
                        EntityId = applicant.Entity.Id,
                    }, applicant.Entity.Recruiter);

                    await context.SaveChangesAsync(token);
                }
            });
        }

        public void FinalDecideOnApplicants(JobVacancy jobVacancy)
        {
            Queue.QueueBackgroundWorkItem(async token =>
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var context = scopedServices.GetRequiredService<ApplicationDbContext>();
                    var hubContext = scopedServices.GetRequiredService<IHubContext<SignalRHub>>();
                    var env = scopedServices.GetRequiredService<IHostingEnvironment>();

                    var applicants = context.Applicants
                        .Where(a => a.JobVacancyId == jobVacancy.Id)
                        .ToList();
                    applicants.ForEach(a => a.Status = (int) ((ApplicantStatus) a.Status).GetFinal());

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
                            Accepted = ((ApplicantStatus) a.Status).IsAccepted(),
                        })
                        .ToList();

                    Operator.CalculateAndNormalizeEvaluations(evaluatedResumes, jobVacancy);
                    evaluatedResumes.ForEach(r =>
                        FilesManager.Store(env, "Evaluations", r.Applicant.Id, r.Evaluation));

                    jobVacancy.Status = (int) JobVacancyStatus.InAction;
                    jobVacancy.AwaitingApplicants =
                        applicants.Count(a => a.Status == (int) ApplicantStatus.AcceptedByRecruiter);

                    evaluatedResumes.ForEach(async evaluatedResume =>
                        await context.SendNotificationAsync(hubContext, new Notification
                        {
                            Type = (int) (evaluatedResume.Accepted
                                ? NotificationType.ApplicantAccepted
                                : NotificationType.ApplicantRejected),
                            Peer1 = jobVacancy.Title,
                            EntityId = evaluatedResume.Applicant.Id,
                        }, evaluatedResume.Applicant.JobSeeker));

                    await context.SaveChangesAsync(token);
                }
            });
        }

        public void Recommend(JobVacancy jobVacancy)
        {
            Queue.QueueBackgroundWorkItem(async token =>
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var context = scopedServices.GetRequiredService<ApplicationDbContext>();
                    var hubContext = scopedServices.GetRequiredService<IHubContext<SignalRHub>>();
                    var env = scopedServices.GetRequiredService<IHostingEnvironment>();

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
                            Type = (int) NotificationType.ResumeRecommendation,
                            EntityId = applicant.Id,
                            Peer1 = jobVacancy.Title,
                        }, r.User);
                    });
                    await context.SendNotificationAsync(hubContext, new Notification
                    {
                        Type = (int) NotificationType.FinishedRecommendation,
                        EntityId = jobVacancy.Id,
                        Peer1 = jobVacancy.Title,
                    }, jobVacancy.User);

                    await context.SaveChangesAsync(token);
                }
            });
        }

        public void DeleteJobVacancy(JobVacancy jobVacancy)
        {

            Queue.QueueBackgroundWorkItem(async token =>
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var context = scopedServices.GetRequiredService<ApplicationDbContext>();
                    var hubContext = scopedServices.GetRequiredService<IHubContext<SignalRHub>>();

                    if (jobVacancy.Status != (int) JobVacancyStatus.Finished)
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

                    await context.SaveChangesAsync(token);
                }
            });
        }

        public void DeleteApplicant(Applicant applicant)
        {
            Queue.QueueBackgroundWorkItem(async token =>
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var context = scopedServices.GetRequiredService<ApplicationDbContext>();
                    var hubContext = scopedServices.GetRequiredService<IHubContext<SignalRHub>>();

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
                    await context.SaveChangesAsync(token);
                }
            });
        }

        private static List<EvaluatedResume> FetchMatchingResumes(ApplicationDbContext context,
            IHostingEnvironment env, JobVacancy jobVacancy)
        {
            var similaritiesQueryPath = Path.Combine(env.ContentRootPath, "Queries", "GetSimilarities.txt");

            var jobTitlesSimilarities = GetSimilarJobTitles(context, jobVacancy, similaritiesQueryPath);
            var fieldsOfStudySimilarities = GetSimilarFieldsOfStudy(context, jobVacancy, similaritiesQueryPath);
            var skillsSimilarities = GetSimilarSkills(context, jobVacancy, similaritiesQueryPath);

            var jobVacancyCityId = context.CompanyDepartments
                .SingleOrDefault(d => d.Id == jobVacancy.CompanyDepartmentId)?
                .CityId;

            var requiredJobTitles = jobTitlesSimilarities
                .Where(keyValuePair => keyValuePair.Key.Type == (int) QualificationType.Required)
                .Select(keyValuePair => keyValuePair.Value)
                .ToList();
            var requiredFieldsOfStudy = fieldsOfStudySimilarities
                .Where(keyValuePair => keyValuePair.Key.Type == (int)QualificationType.Required)
                .Select(keyValuePair => keyValuePair.Value)
                .ToList();
            var requiredSkills = skillsSimilarities
                .Where(keyValuePair => keyValuePair.Key.Type == (int)QualificationType.Required)
                .Select(keyValuePair => keyValuePair.Value)
                .ToList();

            var maxCount = new[]
            {
                requiredJobTitles.Count,
                requiredFieldsOfStudy.Count,
                requiredSkills.Count
            }.Max();

            var maxJobTitles = requiredJobTitles.Count > 0;
            var maxFieldsOfStudy = requiredFieldsOfStudy.Count > 0;
            var maxSkills = requiredSkills.Count > 0;

            var resumes = context.Resumes
                .Include(r => r.WorkExperiences)
                .Include(r => r.Educations)
                .Include(r => r.OwnedSkills)
                .Include(r => r.User)
                .Where(r =>
                    r.IsSeeking &&
                    (
                        jobVacancy.DistanceLimit == 0 ||
                        context.GeoDistances.SingleOrDefault(d =>
                                d.City1Id == r.User.CityId && d.City2Id == jobVacancyCityId ||
                                d.City1Id == r.User.CityId && d.City2Id == jobVacancyCityId)
                            .Distance <= jobVacancy.DistanceLimit
                    ) &&
                    r.JobTypes
                        .Select(type => type.JobType)
                        .Any(resumeType => jobVacancy.JobTypes
                            .Select(vacancyType => vacancyType.JobType)
                            .Contains(resumeType)) &&
                    r.MinSalary <= jobVacancy.MaxSalary && r.MinSalary >= jobVacancy.MinSalary &&
                    r.WorkExperiences.Any(w => !maxJobTitles || requiredJobTitles[0].Contains(w.JobTitleId)) &&
                    r.Educations.Any(e => !maxFieldsOfStudy || requiredFieldsOfStudy[0].Contains(e.FieldOfStudyId)) &&
                    r.OwnedSkills.Any(s => !maxSkills || requiredSkills[0].Contains(s.SkillId)))
                .ToList();

            for (var i = 1; i < maxCount; i++)
            {
                resumes = Filter(resumes, requiredJobTitles, requiredFieldsOfStudy, requiredSkills, i);
            }

            foreach (var resume in resumes)
            {
                foreach (var workExperience in resume.WorkExperiences)
                {
                    if (jobTitlesSimilarities.Any(js => js.Value.Contains(workExperience.JobTitleId)))
                    {
                        workExperience.JobTitleId = jobTitlesSimilarities
                            .FirstOrDefault(js => js.Value.Contains(workExperience.JobTitleId)).Key.JobTitleId;
                    }
                }
                foreach (var education in resume.Educations)
                {
                    if (fieldsOfStudySimilarities.Any(js => js.Value.Contains(education.FieldOfStudyId)))
                    {
                        education.FieldOfStudyId = fieldsOfStudySimilarities
                            .FirstOrDefault(js => js.Value.Contains(education.FieldOfStudyId)).Key.FieldOfStudyId;
                    }
                }
                foreach (var ownedSkill in resume.OwnedSkills)
                {
                    if (skillsSimilarities.Any(js => js.Value.Contains(ownedSkill.SkillId)))
                    {
                        ownedSkill.SkillId = skillsSimilarities
                            .FirstOrDefault(js => js.Value.Contains(ownedSkill.SkillId)).Key.SkillId;
                    }
                }

                context.Entry(resume).State = EntityState.Unchanged;
            }

            return resumes.Select(r => new EvaluatedResume { Resume = r }).ToList();
        }

        private static Dictionary<WorkExperienceQualification, IEnumerable<long?>> GetSimilarJobTitles(
            ApplicationDbContext context, JobVacancy jobVacancy, string similaritiesQueryPath)
        {
            return jobVacancy.WorkExperienceQualifications
                .ToDictionary(
                    q => q, 
                    q => SimilaritiesOperator
                        .GetSimilarities(q.JobTitle.NormalizedTitle, similaritiesQueryPath)
                        .Select(s => context.JobTitles.FirstOrDefault(jdb => jdb.NormalizedTitle == s)?.Id));
        }

        private static Dictionary<EducationQualification, IEnumerable<long?>> GetSimilarFieldsOfStudy(
            ApplicationDbContext context, JobVacancy jobVacancy, string similaritiesQueryPath)
        {
            return jobVacancy.EducationQualifications
                .ToDictionary(
                    q => q,
                    q => SimilaritiesOperator
                        .GetSimilarities(q.FieldOfStudy.NormalizedTitle, similaritiesQueryPath)
                        .Select(s => context.FieldOfStudies.FirstOrDefault(fdb => fdb.NormalizedTitle == s)?.Id));
        }

        private static Dictionary<DesiredSkill, IEnumerable<long?>> GetSimilarSkills(
            ApplicationDbContext context, JobVacancy jobVacancy, string similaritiesQueryPath)
        {
            return jobVacancy.DesiredSkills
                .ToDictionary(
                    sk => sk,
                    sk => SimilaritiesOperator
                        .GetSimilarities(sk.Skill.NormalizedTitle, similaritiesQueryPath)
                        .Select(s => context.Skills.FirstOrDefault(skdb => skdb.NormalizedTitle == s)?.Id));
        }

        private static List<Resume> Filter(IEnumerable<Resume> resumes, 
            IReadOnlyList<IEnumerable<long?>> requiredJobTitles,
            IReadOnlyList<IEnumerable<long?>> requiredFieldsOfStudy, 
            IReadOnlyList<IEnumerable<long?>> requiredSkills, int i)
        {
            var maxJobTitles = requiredJobTitles.Count > i;
            var maxFieldsOfStudy = requiredFieldsOfStudy.Count > i;
            var maxSkills = requiredSkills.Count > i;
            return resumes
                .Where(r =>
                    r.WorkExperiences.Any(w => !maxJobTitles || requiredJobTitles[i].Contains(w.JobTitleId)) &&
                    r.Educations.Any(e => !maxFieldsOfStudy || requiredFieldsOfStudy[i].Contains(e.FieldOfStudyId)) &&
                    r.OwnedSkills.Any(s => !maxSkills || requiredSkills[i].Contains(s.SkillId)))
                .ToList();
        }
    }
}
