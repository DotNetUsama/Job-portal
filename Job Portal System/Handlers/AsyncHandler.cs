using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Job_Portal_System.Data;
using Job_Portal_System.Enums;
using Job_Portal_System.Models;
using Job_Portal_System.SignalR;
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
                Peer1 = resume.User.FirstName,
                Peer2 = jobVacancy.Title,
                EntityId = applicant.Entity.Id,
            }, applicant.Entity.Recruiter);

            await context.SaveChangesAsync();
        }
    }
}
