using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AccordTestPart3.Exceptions;
using Job_Portal_System.Client;
using Job_Portal_System.Models;
using Job_Portal_System.RankingSystem.Abstracts;

namespace Job_Portal_System.RankingSystem
{
    internal class ResumeDecider : AbstractDecider
    {
        public JobVacancy JobVacancy;

        private readonly List<double> _averages;

        public ResumeDecider(JobVacancy jobVacancy) :
            base(GetParameters(jobVacancy), GetTarget())
        {
            JobVacancy = jobVacancy;
            _averages = new List<double>();
        }

        private static Dictionary<string, Type> GetParameters(JobVacancy jobVacancy)
        {
            var parameters = new Dictionary<string, Type>();
            jobVacancy.EducationQualifications.
                ForEach(e => parameters.Add($"E{e.ToNumber().ToString()}", typeof(double)));
            jobVacancy.WorkExperienceQualifications
                .ForEach(w => parameters.Add($"W{w.ToNumber().ToString()}", typeof(double)));
            jobVacancy.DesiredSkills.
                ForEach(s => parameters.Add($"S{s.ToNumber().ToString()}", typeof(double)));
            parameters.Add("Salary", typeof(double));
            return parameters;
        }

        private static KeyValuePair<string, Type> GetTarget()
        {
            return new KeyValuePair<string, Type>("Accepted", typeof(string));
        }

        public void AddRow(Evaluation evaluation, bool isAccepted)
        {
            var decidableResume = new DecidableResume(evaluation.ToArray(), isAccepted);
            AddNewRow(decidableResume);
        }

        public new OurDecisionTree BuildDecisionTree()
        {
            var tree = base.BuildDecisionTree();
            for (var i = 0; i < Rows[0].Length - 1; i++)
            {
                _averages.Add(Rows.Average(r => (double)r[i]));
            }

            return tree;
        }

        public void Infer(Evaluation evaluation)
        {
            var decidableResume = new DecidableResume(evaluation.ToArray());
            var weaknesses = Infer(decidableResume);
            if (weaknesses.Count == 0)
            {
                var row = decidableResume.GetRow();
                for (var i = 0; i < _averages.Count; i++)
                {
                    if (_averages[i] > (double)row[i]) weaknesses.Add(Attributes[i].Name);
                }
            }
            foreach (var weakness in weaknesses)
            {
                var match = Regex.Match(weakness, @"[0-9]+");
                if (match.Success)
                {
                    var id = long.Parse(match.Value);

                    if (Regex.Match(weakness, @"E[0-9]+").Success)
                        evaluation.EducationsRanks.FindLast(e => e.Key == id).Value.IsWeakness = true;

                    if (Regex.Match(weakness, @"W[0-9]+").Success)
                        evaluation.WorkExperiencesRanks.FindLast(e => e.Key == id).Value.IsWeakness = true;

                    if (Regex.Match(weakness, @"S[0-9]+").Success)
                        evaluation.SkillsRanks.FindLast(e => e.Key == id).Value.IsWeakness = true;
                }
                else if(weakness == "Salary")
                {
                    evaluation.SalaryRank.IsWeakness = true;
                }
            }
        }

        protected override void AddNewRow(Decidable row)
        {
            if (row.GetType() == typeof(DecidableResume))
                AddRow(row);
            else
                throw new InvalidParameterException();
        }
    }
}
