using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Job_Portal_System.Enums;
using Job_Portal_System.RankingSystem.Helpers;
using Job_Portal_System.RankingSystem.Interfaces;

namespace Job_Portal_System.Models
{
    public class EducationQualification : INumerable
    {
        public string Id { get; set; }

        [Required]
        public int Type { get; set; }

        [Required]
        [Range(1, 25)]
        public int MinimumYears { get; set; }

        [Required]
        public int Degree { get; set; }

        public double Min { get; set; }
        public double Range { get; set; } = 1;

        public JobVacancy JobVacancy { get; set; }
        public string JobVacancyId { get; set; }
        public FieldOfStudy FieldOfStudy { get; set; }
        public long FieldOfStudyId { get; set; }

        public long ToNumber()
        {
            return FieldOfStudyId;
        }
        
        public double GetRank(List<Education> educations)
        {
            if (educations.Count == 0) return 0;

            var starts = educations.OrderBy(e => e.StartDate).ToList();
            var ends = educations.OrderBy(e => e.StartDate).ToList();

            var processings = new List<Education> { starts[0] };

            double rank = 0;
            var previousDate = starts[0].StartDate;

            for (int i = 1, j = 0; i < starts.Count || j < ends.Count;)
            {
                DateTime currentDate;
                int maxDegree;
                var endDate = ends[j].EndDate.GetValueOrDefault(DateTime.Now);
                if (i < starts.Count && starts[i].StartDate < endDate)
                {
                    currentDate = starts[i].StartDate;
                    maxDegree = processings.Count == 0 ? 0 : processings.Max(e => e.Degree);
                    processings.Add(starts[i++]);
                }
                else
                {
                    currentDate = endDate;
                    maxDegree = processings.Max(e => e.Degree);
                    processings.Remove(ends[j++]);
                }
                rank += maxDegree * HelperFunctions.GetYears(previousDate, currentDate);
                previousDate = currentDate;
            }
            return rank;
        }

        public void SetMinAndRange(double min, double range)
        {
            Min = min;
            Range = range;
        }
    }
}
