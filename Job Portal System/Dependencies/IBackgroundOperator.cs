using Job_Portal_System.Models;

namespace Job_Portal_System.Dependencies
{
    public interface IBackgroundOperator
    {
        void RequestRecruiterAccountApproval(Recruiter recruiter);
        void ApproveRecruiter(Recruiter recruiter);
        void RejectRecruiter(Recruiter recruiter);
        void SubmitToJobVacancy(JobVacancy jobVacancy, Resume resume);
        void FinalDecideOnApplicants(JobVacancy jobVacancy);
        void Recommend(JobVacancy jobVacancy);
        void DeleteJobVacancy(JobVacancy jobVacancy);
        void DeleteApplicant(Applicant applicant);
    }
}
