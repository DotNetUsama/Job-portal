using System.ComponentModel.DataAnnotations;
using Job_Portal_System.Enums;
using Job_Portal_System.RankingSystem.Interfaces;

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
            return SkillId;
        }
        
        public double GetRank(OwnedSkill skill)
        {
            return ((QualificationType)Type).GetWeight() * skill.Years;
        }
        
        public void SetMinAndRange(double min, double range)
        {
            Min = min;
            Range = range;
        }
    }
}
