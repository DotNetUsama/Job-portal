using System;
using System.ComponentModel;
using Job_Portal_System.Models;

namespace Job_Portal_System.Enums
{
    public enum NotificationType
    {
        // Sent to: Recruiter | When: A job seeker submit to job vacancy
        [Description("Received submission")]
        ReceivedSubmission,
        [Description("Finished recommendation")]
        FinishedRecommendation,
        [Description("Recommendation accepted")]
        RecommendationAccepted,
        [Description("Meeting accepted")]
        MeetingAccepted,
        [Description("Meeting rejected")]
        MeetingRejected,
        [Description("Applicant cancelled")]
        CancelledApplicant,
        [Description("Job vacancy cancelled")]
        CancelledJobVacancy,
        // Sent to: JobSeeker | When: reqruiter accept job seeker's applicant on his job vacancy
        [Description("Applicant accepted")]
        ApplicantAccepted,
        // Sent to: JobSeeker | When: reqruiter reject job seeker's applicant on his job vacancy
        [Description("Applicant rejected")]
        ApplicantRejected,
        [Description("Resume recommended")]
        ResumeRecommendation,
        // Sent to: Recruiter | When: An administrator approve recruiter account and his company info doesn't need to be edited
        [Description("Account approved")]
        AccountApproved,
        // Sent to: Recruiter | When: An administrator approve recruiter account and his company info should be edited
        [Description("Account approved")]
        AccountApprovedEditCompanyInfo,
        // Sent to: Recruiter | When: An administrator reject recruiter account
        [Description("Account rejected")]
        AccountRejected,
        // Sent to: All administrators | When: A recruiter account is registered
        [Description("Recruiter approval")]
        RecruiterApproval,

        Other,
    }

    public static class NotificationTypeMethods
    {
        public static string GetUrl(this NotificationType notificationType, Notification notification)
        {
            switch (notificationType)
            {
                case NotificationType.AccountApproved:
                case NotificationType.AccountRejected:
                    return "/";
                case NotificationType.AccountApprovedEditCompanyInfo:
                    return $"/Companies/Edit?id={notification.EntityId}";
                case NotificationType.RecruiterApproval:
                    return $"/Dashboard/Recruiters/Approve?recruiterId={notification.EntityId}";
                case NotificationType.ReceivedSubmission:
                case NotificationType.ApplicantAccepted:
                case NotificationType.ApplicantRejected:
                case NotificationType.ResumeRecommendation:
                case NotificationType.RecommendationAccepted:
                case NotificationType.MeetingAccepted:
                case NotificationType.MeetingRejected:
                    return $"/Applicants/Details?id={notification.EntityId}";
                case NotificationType.FinishedRecommendation:
                    return $"/Applicants?jobVacancyId={notification.EntityId}";
                case NotificationType.CancelledApplicant:
                    return $"/JobVacancies/Details?id={notification.EntityId}";
                case NotificationType.CancelledJobVacancy:
                    return "#";
                case NotificationType.Other:
                    return null;
                default: 
                    return null;
            }
        }

        public static string GetMessage(this NotificationType notificationType, Notification notification)
        {
            switch (notificationType)
            {
                case NotificationType.AccountApproved:
                case NotificationType.AccountApprovedEditCompanyInfo:
                    return "Your account has been approved.";
                case NotificationType.AccountRejected:
                    return "Your account has been rejected.";
                case NotificationType.RecruiterApproval:
                    return $"({notification.Peer1}) account needs to be approved.";
                case NotificationType.ReceivedSubmission:
                    return $"{notification.Peer1} submitted to your job vacancy ({notification.Peer2})";
                case NotificationType.ApplicantAccepted:
                    return $"Your applicant on job vacancy ({notification.Peer1}) has been accepted";
                case NotificationType.ApplicantRejected:
                    return $"Your applicant on job vacancy ({notification.Peer1}) has been rejected";
                case NotificationType.ResumeRecommendation:
                    return $"Your resume has been recommended on job vacancy ({notification.Peer1})";
                case NotificationType.FinishedRecommendation:
                    return $"Recommending operation on your job vacancy ({notification.Peer1}) has been finished";
                case NotificationType.CancelledApplicant:
                    return
                        $"{notification.Peer1} cancelled his/her applicant on your job vacancy ({notification.Peer2})";
                case NotificationType.CancelledJobVacancy:
                    return $"job vacancy ({notification.Peer1}) has been cancelled";
                case NotificationType.RecommendationAccepted:
                    return $"{notification.Peer1} accepted recommendation on your job vacancy ({notification.Peer2})";
                case NotificationType.MeetingAccepted:
                    return $"{notification.Peer1} accepted meeting on your job vacancy ({notification.Peer2})";
                case NotificationType.MeetingRejected:
                    return $"{notification.Peer1} rejected meeting on your job vacancy ({notification.Peer2})";
                default:
                    return null;
            }
        }
    }
}
