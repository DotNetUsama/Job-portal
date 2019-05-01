using System.Collections.Generic;
using System.Linq;
using Job_Portal_System.Client;
using Job_Portal_System.Models;

namespace Job_Portal_System.RankingSystem
{
    internal class Operator
    {
        private static Evaluation CalculateRanks(Resume resume, JobVacancy jobVacancy)
        {
            var rank = new Evaluation();
            jobVacancy.EducationQualifications.ForEach(educationQualification =>
            {
                var matchingEducations = resume.Educations
                    .Where(e => e.ToNumber() == educationQualification.ToNumber())
                    .ToList();
                rank.AddEducationRank(
                    educationQualification.FieldOfStudyId,
                    educationQualification.GetRank(matchingEducations));
            });
            jobVacancy.WorkExperienceQualifications.ForEach(workExperienceQualification =>
            {
                var matchingWorkExperiences = resume.WorkExperiences
                    .Where(e => e.ToNumber() == workExperienceQualification.ToNumber())
                    .ToList();
                rank.AddWorkExperienceRank(
                    workExperienceQualification.JobTitleId,
                    workExperienceQualification.GetRank(matchingWorkExperiences));
            });
            jobVacancy.DesiredSkills.ForEach(skill =>
            {
                var matchingSkill = resume.OwnedSkills
                    .Find(e => e.ToNumber() == skill.ToNumber());
                if (matchingSkill != null)
                    rank.AddSkillRank(skill.SkillId, skill.GetRank(matchingSkill));
                else
                    rank.AddSkillRank(skill.SkillId, 0);
            });
            rank.AddSalaryRank(jobVacancy.MaxSalary - resume.MinSalary);
            return rank;
        }

        private static double[] SetMinsAndRanges(Dictionary<Resume, List<double>> resumes, int i)
        {
            var min = resumes.First().Value[i];
            var max = resumes.First().Value[i];

            foreach (var resume in resumes.Values)
            {
                var element = resume[i];
                if (element < min) min = element;
                if (element > max) max = element;
            }
            var range = max - min;

            var res = new double[2];
            res[0] = min;
            res[1] = range;
            return res;
        }

        private static void SetMinsAndRanges(Dictionary<Resume, List<double>> resumes, 
            JobVacancy jobVacancy)
        {

            var i = 0;
            double[] minRange = null;
            jobVacancy.EducationQualifications.ForEach(education =>
            {
                minRange = SetMinsAndRanges(resumes, i++);
                education.SetMinAndRange(minRange[0], minRange[1]);
            });
            jobVacancy.WorkExperienceQualifications.ForEach(workExperience =>
            {
                minRange = SetMinsAndRanges(resumes, i++);
                workExperience.SetMinAndRange(minRange[0], minRange[1]);
            });
            jobVacancy.DesiredSkills.ForEach(skill =>
            {
                minRange = SetMinsAndRanges(resumes, i++);
                skill.SetMinAndRange(minRange[0], minRange[1]);
            });
            minRange = SetMinsAndRanges(resumes, i);
            jobVacancy.SetMinAndRange(minRange[0], minRange[1]);
        }

        private static double[] NormalizeResumesRanks(Dictionary<Resume, List<double>> resumes, int i)
        {
            var min = resumes.First().Value[i];
            var max = resumes.First().Value[i];

            foreach (var resume in resumes.Values)
            {
                var element = resume[i];
                if (element < min) min = element;
                if (element > max) max = element;
            }
            var range = max - min;

            if (range > 0.00001)
            {
                foreach (var resume in resumes.Values)
                {
                    resume[i] = (resume[i] - min) / range;
                }
            }
            else
            {
                foreach (var resume in resumes.Values)
                {
                    resume[i] = 0;
                }
            }

            var res = new double[2];
            res[0] = min;
            res[1] = range;
            return res;
        }

        private static void NormalizeResumesRanks(Dictionary<Resume, List<double>> resumes,
            JobVacancy jobVacancy)
        {
            var i = 0;
            double[] minRange = null;
            jobVacancy.EducationQualifications.ForEach(education =>
            {
                minRange = NormalizeResumesRanks(resumes, i++);
                education.SetMinAndRange(minRange[0], minRange[1]);
            });
            jobVacancy.WorkExperienceQualifications.ForEach(workExperience =>
            {
                minRange = NormalizeResumesRanks(resumes, i++);
                workExperience.SetMinAndRange(minRange[0], minRange[1]);
            });
            jobVacancy.DesiredSkills.ForEach(skill =>
            {
                minRange = NormalizeResumesRanks(resumes, i++);
                skill.SetMinAndRange(minRange[0], minRange[1]);
            });
            minRange = NormalizeResumesRanks(resumes, i);
            jobVacancy.SetMinAndRange(minRange[0], minRange[1]);
        }

        private static Dictionary<Resume, double> GetFinalRanks(
            Dictionary<Resume, List<double>> rankedResumes)
        {
            var ranks = new Dictionary<Resume, double>();
            foreach (var rankedResume in rankedResumes)
            {
                ranks.Add(rankedResume.Key, 
                    rankedResume.Value.Aggregate(0.0, (counter, rank) => counter + rank));
            }
            return ranks;
        }

        public static DecidableResume GetDecidableResume(Resume resume, JobVacancy jobVacancy)
        {
            var ranks = CalculateRanks(resume, jobVacancy);
            return new DecidableResume(ranks.ToArray());
        }

        public static List<Resume> GetRecommendedResumes(List<Resume> fetchedResumes, JobVacancy jobVacancy)
        {
            var rankedResumes = new Dictionary<Resume, List<double>>();
            fetchedResumes.ForEach(resume =>
            {
                rankedResumes.Add(resume, CalculateRanks(resume, jobVacancy).ToList());
            });

            NormalizeResumesRanks(rankedResumes, jobVacancy);

            var ranks = GetFinalRanks(rankedResumes);

            return (from entry in ranks orderby entry.Value descending select entry.Key).Take(10).ToList();
        }

        public static Dictionary<string, Evaluation> GetEvaluations(
            Dictionary<Applicant, KeyValuePair<Resume, bool>> resumes, 
            JobVacancy jobVacancy)
        {
            var rankedResumes = new Dictionary<Resume, List<double>>();
            var evaluations = new Dictionary<string, Evaluation>();
            foreach (var resume in resumes)
            {
                var evaluation = CalculateRanks(resume.Value.Key, jobVacancy);
                evaluations.Add(resume.Key.Id, evaluation);
                rankedResumes.Add(resume.Value.Key, evaluation.ToList());
            }

            SetMinsAndRanges(rankedResumes, jobVacancy);

            var decider = new ResumeDecider(jobVacancy);
            foreach (var resume in resumes)
            {
                decider.AddResume(resume.Value);
            }
            decider.BuildDecisionTree();
            foreach (var evaluation in evaluations)
            {
                decider.Infer(evaluation.Value);
            }
            return evaluations;
        }
    }
}
