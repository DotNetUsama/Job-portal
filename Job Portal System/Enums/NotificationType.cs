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
        FinishedRecommendation,
        RecommendationAccepted,
        MeetingAccepted,
        MeetingRejected,
        CancelledApplicant,
        CancelledJobVacancy,
        ApplicantAccepted,
        ApplicantRejected,
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
                    return $"/Applicants/Details?id={notification.EntityId}";
                case NotificationType.FinishedRecommendation:
                case NotificationType.RecommendationAccepted:
                case NotificationType.MeetingAccepted:
                case NotificationType.MeetingRejected:
                case NotificationType.CancelledApplicant:
                case NotificationType.CancelledJobVacancy:
                case NotificationType.ResumeRecommendation:
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
                    return $"Your applicant on job vacancy ({notification.Peer1} has been accepted)";
                case NotificationType.ApplicantRejected:
                    return $"Your applicant on job vacancy ({notification.Peer1} has been rejected)";
                default:
                    return null;
            }
        }

        public static string GetDescription(this Enum value)
        {
            var type = value.GetType();
            var name = Enum.GetName(type, value);
            if (name == null) return null;
            var field = type.GetField(name);
            if (field == null) return null;
            if (Attribute.GetCustomAttribute(field,
                typeof(DescriptionAttribute)) is DescriptionAttribute attr)
            {
                return attr.Description;
            }
            return null;
        }
    }
}
