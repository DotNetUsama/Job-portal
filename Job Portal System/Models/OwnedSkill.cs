using System.ComponentModel.DataAnnotations;
using Job_Portal_System.Utilities.RankingSystem.Interfaces;

namespace Job_Portal_System.Models
{
    public class OwnedSkill : INumerable
    {
        public string Id { get; set; }

        [Required]
        [Range(1, 25)]
        public int Years { get; set; }

        public Resume Resume { get; set; }
        public Skill Skill { get; set; }
        public long SkillId { get; set; }

        public long ToNumber()
        {
            return SkillId;
        }
    }
}
