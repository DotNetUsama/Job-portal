using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using AccordTestPart3.Exceptions;
using Job_Portal_System.Client;
using Job_Portal_System.Models;
using Job_Portal_System.RankingSystem.Abstracts;

namespace Job_Portal_System.RankingSystem
{
    [Serializable]
    internal class ResumeDecider : AbstractDecider
    {
        [NonSerialized]
        public JobVacancy JobVacancy;

        public ResumeDecider(JobVacancy jobVacancy) :
            base(GetParameters(jobVacancy), GetTarget())
        {
            JobVacancy = jobVacancy;
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

        public void AddResume(KeyValuePair<Resume, bool> resume)
        {
            var decidableResume = Operator.GetDecidableResume(resume.Key, JobVacancy);
            if (resume.Value) decidableResume.Accept();
            AddNewRow(decidableResume);
        }

        public void Infer(Evaluation evaluation)
        {
            var weaknesses = Infer(new DecidableResume(evaluation.ToArray()));
            foreach (var weakness in weaknesses)
            {
                var match = Regex.Match(weakness, @"[0-9]+");
                if (match.Success)
                {
                    var id = long.Parse(match.Value);

                    if (Regex.Match(weakness, @"E[0-9]+").Success)
                        evaluation.EducationsRanks[id].IsWeakness = true;

                    if (Regex.Match(weakness, @"W[0-9]+").Success)
                        evaluation.WorkExperiencesRanks[id].IsWeakness = true;

                    if (Regex.Match(weakness, @"S[0-9]+").Success)
                        evaluation.SkillsRanks[id].IsWeakness = true;
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
