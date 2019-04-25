using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Job_Portal_System.Data;
using Job_Portal_System.Models;
using Microsoft.EntityFrameworkCore;

namespace Job_Portal_System.Enums
{
    public enum NotificationType
    {
        // Sent to: Recruiter | When: A job seeker submit to job vacancy
        [Description("Received submission")]
        ReceivedSubmission,
        FinishedRecommendation,
        ApprovedRecommendation,
        CancelledApplicant,
        ApplicantDecide,
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
                    return $"/Applicants/Details?id={notification.EntityId}";
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
                default:
                    return null;
            }
        }

        public static string GetDescription(this Enum value)
        {
            Type type = value.GetType();
            string name = Enum.GetName(type, value);
            if (name != null)
            {
                FieldInfo field = type.GetField(name);
                if (field != null)
                {
                    DescriptionAttribute attr =
                        Attribute.GetCustomAttribute(field,
                            typeof(DescriptionAttribute)) as DescriptionAttribute;
                    if (attr != null)
                    {
                        return attr.Description;
                    }
                }
            }
            return null;
        }
    }
}
