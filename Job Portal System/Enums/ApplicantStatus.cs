using System.Collections.Generic;
using System.ComponentModel;

namespace Job_Portal_System.Enums
{
    public enum ApplicantStatus
    {
        [Description("Pending recommendation")]
        PendingRecommendation,
        [Description("Rejected recommendation")]
        RejectedRecommendation,
        [Description("Waiting recruiter decision")]
        WaitingRecruiterDecision,
        [Description("Dummy accepted")]
        DummyAccepted,
        [Description("Dummy rejected")]
        DummyRejected,
        [Description("Accepted by recruiter")]
        AcceptedByRecruiter,
        [Description("Rejected by recruiter")]
        RejectedByRecruiter,
        [Description("Accept meeting")]
        AcceptMeeting,
        [Description("Reject meeting")]
        RejectMeeting,
        Other
    }

    public static class ApplicantStatusMethods
    {
        public static string GetCssClass(this ApplicantStatus status)
        {
            switch (status)
            {
                case ApplicantStatus.WaitingRecruiterDecision:
                case ApplicantStatus.PendingRecommendation:
                    return "badge badge-info text-white";
                case ApplicantStatus.DummyAccepted:
                case ApplicantStatus.AcceptMeeting:
                case ApplicantStatus.AcceptedByRecruiter:
                    return "badge badge-success text-white";
                case ApplicantStatus.DummyRejected:
                case ApplicantStatus.RejectMeeting:
                case ApplicantStatus.RejectedByRecruiter:
                case ApplicantStatus.RejectedRecommendation:
                    return "badge badge-danger text-white";
                default: return null;
            }
        }

        public static string GetBtnCssClass(this ApplicantStatus status)
        {
            switch (status)
            {
                case ApplicantStatus.WaitingRecruiterDecision:
                case ApplicantStatus.PendingRecommendation:
                    return "btn-info";
                case ApplicantStatus.DummyAccepted:
                case ApplicantStatus.AcceptMeeting:
                case ApplicantStatus.AcceptedByRecruiter:
                    return "btn-success";
                case ApplicantStatus.DummyRejected:
                case ApplicantStatus.RejectMeeting:
                case ApplicantStatus.RejectedByRecruiter:
                case ApplicantStatus.RejectedRecommendation:
                    return "btn-danger";
                default: return null;
            }
        }

        public static string GetActionName(this ApplicantStatus status)
        {
            switch (status)
            {
                case ApplicantStatus.DummyAccepted:
                case ApplicantStatus.AcceptMeeting:
                case ApplicantStatus.WaitingRecruiterDecision:
                    return "Accept";
                case ApplicantStatus.DummyRejected:
                case ApplicantStatus.RejectMeeting:
                case ApplicantStatus.RejectedRecommendation:
                    return "Reject";
                case ApplicantStatus.PendingRecommendation:
                case ApplicantStatus.AcceptedByRecruiter:
                case ApplicantStatus.RejectedByRecruiter:
                    return null;
                default: return null;
            }
        }

        public static List<ApplicantStatus> NextStates(this ApplicantStatus status, UserType userType)
        {
            switch (userType)
            {
                case UserType.Recruiter: return NextStatesForRecruiter(status);
                case UserType.JobSeeker: return NextStatesForJobSeeker(status);
                case UserType.Administrator:
                case UserType.Other:
                    return new List<ApplicantStatus>();
                default: return null;
            }
        }

        public static bool IsNextState(this ApplicantStatus status,
            ApplicantStatus nextStatus,
            UserType userType)
        {
            return status.NextStates(userType).Contains(nextStatus);
        }

        public static NotificationType GetNotificationType(this ApplicantStatus status)
        {
            switch (status)
            {
                case ApplicantStatus.WaitingRecruiterDecision: return NotificationType.RecommendationAccepted;
                case ApplicantStatus.AcceptMeeting: return NotificationType.MeetingAccepted;
                case ApplicantStatus.RejectMeeting: return NotificationType.MeetingRejected;
                case ApplicantStatus.AcceptedByRecruiter: return NotificationType.ApplicantAccepted;
                case ApplicantStatus.RejectedByRecruiter: return NotificationType.ApplicantRejected;

                case ApplicantStatus.PendingRecommendation:
                case ApplicantStatus.RejectedRecommendation:
                case ApplicantStatus.DummyAccepted:
                case ApplicantStatus.DummyRejected:
                case ApplicantStatus.Other:
                    return NotificationType.Other;
                default:
                    return NotificationType.Other;
            }
        }

        public static ApplicantStatus GetFinal(this ApplicantStatus status)
        {
            switch (status)
            {
                case ApplicantStatus.DummyAccepted: return ApplicantStatus.AcceptedByRecruiter;
                case ApplicantStatus.DummyRejected: 
                case ApplicantStatus.PendingRecommendation:
                    return ApplicantStatus.RejectedByRecruiter;
                case ApplicantStatus.RejectedRecommendation:
                    return status;
                case ApplicantStatus.WaitingRecruiterDecision:
                case ApplicantStatus.AcceptedByRecruiter:
                case ApplicantStatus.RejectedByRecruiter:
                case ApplicantStatus.AcceptMeeting:
                case ApplicantStatus.RejectMeeting:
                case ApplicantStatus.Other:
                    return ApplicantStatus.Other;
                default:
                    return ApplicantStatus.Other;
            }
        }

        public static bool IsAccepted(this ApplicantStatus status)
        {
            switch (status)
            {
                case ApplicantStatus.AcceptedByRecruiter:
                    return true;
                case ApplicantStatus.PendingRecommendation:
                case ApplicantStatus.RejectedRecommendation:
                case ApplicantStatus.WaitingRecruiterDecision:
                case ApplicantStatus.DummyAccepted:
                case ApplicantStatus.DummyRejected:
                case ApplicantStatus.RejectedByRecruiter:
                case ApplicantStatus.AcceptMeeting:
                case ApplicantStatus.RejectMeeting:
                case ApplicantStatus.Other:
                    return false;
                default:
                    return false;
            }
        }

        public static bool IsFinal(this ApplicantStatus status)
        {
            switch (status)
            {
                case ApplicantStatus.RejectedRecommendation:
                case ApplicantStatus.RejectedByRecruiter:
                case ApplicantStatus.RejectMeeting:
                case ApplicantStatus.AcceptMeeting:
                    return true;
                case ApplicantStatus.PendingRecommendation:
                case ApplicantStatus.WaitingRecruiterDecision:
                case ApplicantStatus.DummyAccepted:
                case ApplicantStatus.DummyRejected:
                case ApplicantStatus.AcceptedByRecruiter:
                case ApplicantStatus.Other:
                    return false;
                default:
                    return false;
            }
        }

        public static bool IsEvaluated(this ApplicantStatus status)
        {
            switch (status)
            {
                case ApplicantStatus.PendingRecommendation:
                case ApplicantStatus.RejectedRecommendation:
                case ApplicantStatus.WaitingRecruiterDecision:
                case ApplicantStatus.DummyAccepted:
                case ApplicantStatus.DummyRejected:
                case ApplicantStatus.Other:
                    return false;
                case ApplicantStatus.AcceptMeeting:
                case ApplicantStatus.RejectMeeting:
                case ApplicantStatus.RejectedByRecruiter:
                case ApplicantStatus.AcceptedByRecruiter:
                    return true;
                default:
                    return false;
            }
        }

        private static List<ApplicantStatus> NextStatesForJobSeeker(ApplicantStatus status)
        {
            switch (status)
            {
                case ApplicantStatus.PendingRecommendation:
                    return new List<ApplicantStatus>
                    {
                        ApplicantStatus.WaitingRecruiterDecision,
                        ApplicantStatus.RejectedRecommendation,
                    };
                case ApplicantStatus.AcceptedByRecruiter:
                    return new List<ApplicantStatus>
                    {
                        ApplicantStatus.AcceptMeeting,
                        ApplicantStatus.RejectMeeting,
                    };
                case ApplicantStatus.DummyRejected:
                case ApplicantStatus.RejectMeeting:
                case ApplicantStatus.RejectedByRecruiter:
                case ApplicantStatus.RejectedRecommendation:
                case ApplicantStatus.WaitingRecruiterDecision:
                case ApplicantStatus.DummyAccepted:
                case ApplicantStatus.AcceptMeeting:
                    return new List<ApplicantStatus>();
                default: return null;
            }
        }

        private static List<ApplicantStatus> NextStatesForRecruiter(ApplicantStatus status)
        {
            switch (status)
            {
                case ApplicantStatus.WaitingRecruiterDecision:
                    return new List<ApplicantStatus>
                    {
                        ApplicantStatus.DummyRejected,
                        ApplicantStatus.DummyAccepted,
                    };
                case ApplicantStatus.DummyRejected:
                    return new List<ApplicantStatus>
                    {
                        ApplicantStatus.DummyAccepted,
                    };
                case ApplicantStatus.DummyAccepted:
                    return new List<ApplicantStatus>
                    {
                        ApplicantStatus.DummyRejected,
                    };
                case ApplicantStatus.AcceptedByRecruiter:
                case ApplicantStatus.PendingRecommendation:
                case ApplicantStatus.RejectMeeting:
                case ApplicantStatus.RejectedByRecruiter:
                case ApplicantStatus.RejectedRecommendation:
                case ApplicantStatus.AcceptMeeting:
                    return new List<ApplicantStatus>();
                default: return null;
            }
        }
    }
}
