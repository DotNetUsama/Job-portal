﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Job_Portal_System.Models;

namespace Job_Portal_System.Enums
{
    public enum NotificationType
    {
        ReceivedSubmission,
        FinishedRecommendation,
        ApprovedRecommendation,
        CancelledApplicant,
        ApplicantDecide,
        ResumeRecommendation,
        [Description("Account approved")]
        AccountApproved,
        [Description("Account approved")]
        AccountApprovedEditCompanyInfo,
        [Description("Account rejected")]
        AccountRejected,
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
                    return "/Companies/Edit?id=" + notification.Peer1;
                case NotificationType.RecruiterApproval:
                    return "/Dashboard/Recruiters/Approve?recruiterId=" + notification.Peer1;
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
                    return "A recruiter account needs to be approved.";
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
