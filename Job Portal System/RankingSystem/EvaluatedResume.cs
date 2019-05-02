﻿using System.Linq;
using Job_Portal_System.Client;
using Job_Portal_System.Models;

namespace Job_Portal_System.RankingSystem
{
    public class EvaluatedResume
    {
        public Resume Resume { get; set; }
        public Applicant Applicant { get; set; }
        public Evaluation Evaluation { get; set; }
        public bool Accepted { get; set; }

        public void Evaluate(JobVacancy jobVacancy)
        {
            var evaluation = new Evaluation();
            jobVacancy.EducationQualifications.ForEach(educationQualification =>
            {
                var matchingEducations = Resume.Educations
                    .Where(e => e.ToNumber() == educationQualification.ToNumber())
                    .ToList();
                evaluation.AddEducationRank(
                    educationQualification.FieldOfStudyId,
                    educationQualification.GetRank(matchingEducations));
            });
            jobVacancy.WorkExperienceQualifications.ForEach(workExperienceQualification =>
            {
                var matchingWorkExperiences = Resume.WorkExperiences
                    .Where(e => e.ToNumber() == workExperienceQualification.ToNumber())
                    .ToList();
                evaluation.AddWorkExperienceRank(
                    workExperienceQualification.JobTitleId,
                    workExperienceQualification.GetRank(matchingWorkExperiences));
            });
            jobVacancy.DesiredSkills.ForEach(skill =>
            {
                var matchingSkill = Resume.OwnedSkills
                    .Find(e => e.ToNumber() == skill.ToNumber());
                if (matchingSkill != null)
                    evaluation.AddSkillRank(skill.SkillId, skill.GetRank(matchingSkill));
                else
                    evaluation.AddSkillRank(skill.SkillId, 0);
            });
            evaluation.AddSalaryRank(jobVacancy.MaxSalary - Resume.MinSalary);
            Evaluation = evaluation;
        }
    }
}
