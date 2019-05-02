using System.Collections.Generic;
using System.Linq;
using Job_Portal_System.Client;
using Job_Portal_System.Models;

namespace Job_Portal_System.RankingSystem
{
    internal class Operator
    {
        private static void NormalizeEvaluations(List<List<Rank>> ranks, int i)
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

            ranks.ForEach(r => r[i].SetRate(min, range));
        }

        private static void NormalizeEvaluations(List<List<Rank>> ranks, JobVacancy jobVacancy)
        {
            var i = 0;
            jobVacancy.EducationQualifications.ForEach(education =>
            {
                NormalizeEvaluations(ranks, i++);
            });
            jobVacancy.WorkExperienceQualifications.ForEach(workExperience =>
            {
                NormalizeEvaluations(ranks, i++);
            });
            jobVacancy.DesiredSkills.ForEach(skill =>
            {
                NormalizeEvaluations(ranks, i++);
            });
            NormalizeEvaluations(ranks, i);
        }

        //private static Dictionary<Resume, double> GetFinalRanks(
        //    Dictionary<Resume, List<double>> rankedResumes)
        //{
        //    var ranks = new Dictionary<Resume, double>();
        //    foreach (var rankedResume in rankedResumes)
        //    {
        //        ranks.Add(rankedResume.Key, 
        //            rankedResume.Value.Aggregate(0.0, (counter, rank) => counter + rank));
        //    }
        //    return ranks;
        //}

        //public static List<Resume> GetRecommendedResumes(List<Resume> fetchedResumes, JobVacancy jobVacancy)
        //{
        //    var rankedResumes = new Dictionary<Resume, List<double>>();
        //    fetchedResumes.ForEach(evaluatedResume =>
        //    {
        //        rankedResumes.Add(evaluatedResume, EvaluateResume(evaluatedResume, jobVacancy).ToList());
        //    });

        //    NormalizeEvaluations(rankedResumes, jobVacancy);

        //    var ranks = GetFinalRanks(rankedResumes);

        //    return (from entry in ranks orderby entry.Value descending select entry.Key).Take(10).ToList();
        //}

        public static void GetEvaluations(List<EvaluatedResume> evaluatedResumes, 
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
