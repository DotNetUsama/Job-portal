using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Job_Portal_System.Enums
{
    public enum ApplicantStatus
    {
        PendingRecommendation,
        RejectedRecommendation,
        WaitingRecruiterDecision,
        DummyAccepted,
        DummyRejected,
        AcceptedByRecruiter,
        RejectedByRecruiter,
        AcceptMeeting,
        RejectMeeting,
    }
}
