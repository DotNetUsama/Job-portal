using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Job_Portal_System.Enums
{
    public enum ApplicantStatus
    {
        PendingRecommendation,
        RejectedRecommendation,
        [Description("Waiting recruiter decision")]
        WaitingRecruiterDecision,
        DummyAccepted,
        DummyRejected,
        AcceptedByRecruiter,
        RejectedByRecruiter,
        AcceptMeeting,
        RejectMeeting,
    }
}
