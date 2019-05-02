using System;
using System.Collections.Generic;
using System.Linq;

namespace Job_Portal_System.Client
{
    [Serializable]
    public class Evaluation
    {
        public List<KeyValuePair<long, Rank>> EducationsRanks { get; set; } = new List<KeyValuePair<long, Rank>>();
        public List<KeyValuePair<long, Rank>> WorkExperiencesRanks { get; set; } = new List<KeyValuePair<long, Rank>>();
        public List<KeyValuePair<long, Rank>> SkillsRanks { get; set; } = new List<KeyValuePair<long, Rank>>();
        public Rank SalaryRank { get; set; }

        public void AddEducationRank(long id, double rate)
        {
            EducationsRanks.Add(new KeyValuePair<long, Rank>(id, new Rank { Rate = rate }));
        }

        public void AddWorkExperienceRank(long id, double rate)
        {
            WorkExperiencesRanks.Add(new KeyValuePair<long, Rank>(id, new Rank { Rate = rate }));
        }

        public void AddSkillRank(long id, double rate)
        {
            SkillsRanks.Add(new KeyValuePair<long, Rank>(id, new Rank { Rate = rate }));
        }

        public void AddSalaryRank(double rate)
        {
            SalaryRank = new Rank { Rate = rate };
        }

        public List<Rank> GetRanksList()
        {
            var list = EducationsRanks.Select(e => e.Value).ToList();
            list.AddRange(WorkExperiencesRanks.Select(w => w.Value));
            list.AddRange(SkillsRanks.Select(s => s.Value));
            list.Add(SalaryRank);
            return list;
        }

        public List<double> ToList()
        {
            var list = EducationsRanks
                .Select(e => e.Value.Rate)
                .ToList();
            list.AddRange(WorkExperiencesRanks
                    .Select(e => e.Value.Rate));
            list.AddRange(SkillsRanks
                .Select(e => e.Value.Rate));
            list.Add(SalaryRank.Rate);
            return list;
        }
        
        public double[] ToArray()
        {
            return ToList().ToArray();
        }
    }

    [Serializable]
    public class Rank
    {
        public double Rate { get; set; }
        public bool IsWeakness { get; set; } = false;

        public void SetRate(double min, double range)
        {
            Rate = range > 0.00001 ? (Rate - min) / range : 0;
        }
    }
}
