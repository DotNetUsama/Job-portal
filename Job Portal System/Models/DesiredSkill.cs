using System.ComponentModel.DataAnnotations;
using Job_Portal_System.Enums;
using Job_Portal_System.Utilities.RankingSystem.Interfaces;

namespace Job_Portal_System.Models
{
    public class DesiredSkill : INumerable
    {
        public string Id { get; set; }

        [Required]
        public int Type { get; set; }

        [Required]
        [Range(1, 25)]
        public int MinimumYears { get; set; }

        public double Min { get; set; }
        public double Range { get; set; } = 1;

        public JobVacancy JobVacancy { get; set; }
        public string JobVacancyId { get; set; }
        public Skill Skill { get; set; }
        public long SkillId { get; set; }

        public long ToNumber()
        {
            return Skill.SkillSynsetId;
        }
        
        public double GetRank(OwnedSkill skill)
        {
            return skill.Years;
        }
        
        public void SetMinAndRange(double[] minRange)
        {
            Min = minRange[0];
            Range = minRange[1];
        }
    }
}
