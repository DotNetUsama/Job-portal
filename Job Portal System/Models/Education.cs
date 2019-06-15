using System;
using System.ComponentModel.DataAnnotations;
using Job_Portal_System.Utilities.RankingSystem.Interfaces;

namespace Job_Portal_System.Models
{
    public class Education : INumerable
    {
        public string Id { get; set; }

        [Required]
        public int Degree { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }

        public Resume Resume { get; set; }
        public string ResumeId { get; set; }

        public FieldOfStudy FieldOfStudy { get; set; }
        public long FieldOfStudyId { get; set; }

        public School School { get; set; }
        public string SchoolId { get; set; }

        public long ToNumber()
        {
            return FieldOfStudy.FieldOfStudySynsetId;
        }
    }
}
