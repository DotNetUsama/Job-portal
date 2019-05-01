using System;
using System.Collections.Generic;
using System.Linq;

namespace Job_Portal_System.Client
{
    [Serializable]
    public class Evaluation
    {
        public Dictionary<long, Rank> EducationsRanks { get; set; } = new Dictionary<long, Rank>();
        public Dictionary<long, Rank> WorkExperiencesRanks { get; set; } = new Dictionary<long, Rank>();
        public Dictionary<long, Rank> SkillsRanks { get; set; } = new Dictionary<long, Rank>();
        public Rank SalaryRank { get; set; }

        public void AddEducationRank(long id, double rate)
        {
            EducationsRanks.Add(id, new Rank { Rate = rate });
        }

        public void AddWorkExperienceRank(long id, double rate)
        {
            WorkExperiencesRanks.Add(id, new Rank { Rate = rate });
        }

        public void AddSkillRank(long id, double rate)
        {
            SkillsRanks.Add(id, new Rank { Rate = rate });
        }

        public void AddSalaryRank(double rate)
        {
            SalaryRank = new Rank { Rate = rate };
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
        public double Rate { get; set; } = 0;
        public bool IsWeakness { get; set; } = false;
    }
}
