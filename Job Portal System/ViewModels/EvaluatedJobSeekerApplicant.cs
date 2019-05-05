using System.Collections.Generic;
using Job_Portal_System.Models;
using Job_Portal_System.RankingSystem;

namespace Job_Portal_System.ViewModels
{
    public class EvaluatedJobSeekerApplicant
    {
        public List<EvaluatedEducationQualification> Educations;
        public List<EvaluatedWorkExperienceQualification> WorkExperiences;
        public List<EvaluatedDesiredSkill> DesiredSkills;
        public Rank SalaryRank;

        public bool IsEvaluated;

        public Applicant Applicant;
    }

    public class EvaluatedEducationQualification : EducationQualification
    {
        public Rank Evaluation { get; set; }

        public EvaluatedEducationQualification(EducationQualification education)
        {
            Id = education.Id;
            Type = education.Type;
            FieldOfStudy = education.FieldOfStudy;
            MinimumYears = education.MinimumYears;
            Degree = education.Degree;
        }

        public EvaluatedEducationQualification(EducationQualification education, Rank rank)
        {
            Id = education.Id;
            Type = education.Type;
            FieldOfStudy = education.FieldOfStudy;
            MinimumYears = education.MinimumYears;
            Degree = education.Degree;
            Evaluation = rank;
        }
    }
    public class EvaluatedWorkExperienceQualification : WorkExperienceQualification
    {
        public Rank Evaluation { get; set; }

        public EvaluatedWorkExperienceQualification(WorkExperienceQualification workExperience)
        {
            Id = workExperience.Id;
            Type = workExperience.Type;
            JobTitle = workExperience.JobTitle;
            MinimumYears = workExperience.MinimumYears;
        }

        public EvaluatedWorkExperienceQualification(WorkExperienceQualification workExperience, Rank rank)
        {
            Id = workExperience.Id;
            Type = workExperience.Type;
            JobTitle = workExperience.JobTitle;
            MinimumYears = workExperience.MinimumYears;
            Evaluation = rank;
        }
    }
    public class EvaluatedDesiredSkill : DesiredSkill
    {
        public Rank Evaluation { get; set; }

        public EvaluatedDesiredSkill(DesiredSkill skill)
        {
            Id = skill.Id;
            Type = skill.Type;
            Skill = skill.Skill;
            MinimumYears = skill.MinimumYears;
        }

        public EvaluatedDesiredSkill(DesiredSkill skill, Rank rank)
        {
            Id = skill.Id;
            Type = skill.Type;
            Skill = skill.Skill;
            MinimumYears = skill.MinimumYears;
            Evaluation = rank;
        }
    }
}
