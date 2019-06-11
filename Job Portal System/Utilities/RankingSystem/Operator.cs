using System.Collections.Generic;
using System.Linq;
using Job_Portal_System.Enums;
using Job_Portal_System.Models;

namespace Job_Portal_System.Utilities.RankingSystem
{
    internal class Operator
    {
        private static double[] NormalizeEvaluations(List<List<Rank>> ranks, int i, double weight = 1)
        {
            var min = ranks.First().First().Rate;
            var max = ranks.First().First().Rate;

            foreach (var rank in ranks)
            {
                var element = rank[i].Rate;
                if (element < min) min = element;
                if (element > max) max = element;
            }
            var range = max - min;

            ranks.ForEach(r => r[i].SetRate(min, range, weight));
            return new[] {min, range};
        }

        private static void NormalizeEvaluations(List<List<Rank>> ranks, JobVacancy jobVacancy)
        {
            var i = 0;
            jobVacancy.EducationQualifications.ForEach(education =>
            {
                education.SetMinAndRange(NormalizeEvaluations(ranks, i++, ((QualificationType)education.Type).GetWeight()));
            });
            jobVacancy.WorkExperienceQualifications.ForEach(workExperience =>
            {
                workExperience.SetMinAndRange(NormalizeEvaluations(ranks, i++, ((QualificationType)workExperience.Type).GetWeight()));
            });
            jobVacancy.DesiredSkills.ForEach(skill =>
            {
                skill.SetMinAndRange(NormalizeEvaluations(ranks, i++, ((QualificationType)skill.Type).GetWeight()));
            });
            jobVacancy.SetMinAndRange(NormalizeEvaluations(ranks, i));
        }

        private static Dictionary<Resume, double> GetFinalRanks(IEnumerable<EvaluatedResume> rankedResumes)
        {
            var ranks = new Dictionary<Resume, double>();
            foreach (var rankedResume in rankedResumes)
            {
                var resumeRanks = rankedResume.Evaluation.ToList();
                ranks.Add(rankedResume.Resume,
                    resumeRanks.Aggregate(0.0, (counter, rank) => counter + rank));
            }
            return ranks;
        }

        public static List<Resume> GetRecommendedResumes(List<EvaluatedResume> fetchedResumes, JobVacancy jobVacancy)
        {
            fetchedResumes.ForEach(r => r.Evaluate(jobVacancy));

            NormalizeEvaluations(fetchedResumes.Select(e => e.Evaluation.GetRanksList()).ToList(), jobVacancy);

            var ranks = GetFinalRanks(fetchedResumes);

            return ranks
                .OrderByDescending(r => r.Value)
                .Select(r => r.Key)
                .Take(10)
                .ToList();
        }

        public static void CalculateAndNormalizeEvaluations(List<EvaluatedResume> evaluatedResumes, 
            JobVacancy jobVacancy)
        {
            evaluatedResumes.ForEach(r => r.Evaluate(jobVacancy));

            NormalizeEvaluations(evaluatedResumes.Select(e => e.Evaluation.GetRanksList()).ToList(), jobVacancy);

            var decider = new ResumeDecider(jobVacancy);
            foreach (var evaluatedResume in evaluatedResumes)
            {
                decider.AddRow(evaluatedResume.Evaluation, evaluatedResume.Accepted);
            }
            decider.BuildDecisionTree();
            foreach (var evaluatedResume in evaluatedResumes)
            {
                if (!evaluatedResume.Accepted) decider.Infer(evaluatedResume.Evaluation);
            }
        }
    }
}
