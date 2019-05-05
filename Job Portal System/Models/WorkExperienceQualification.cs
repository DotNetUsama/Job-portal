using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Job_Portal_System.Enums;
using Job_Portal_System.RankingSystem.Helpers;
using Job_Portal_System.RankingSystem.Interfaces;

namespace Job_Portal_System.Models
{
    public class WorkExperienceQualification : INumerable
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
        public JobTitle JobTitle { get; set; }
        public long JobTitleId { get; set; }

        public long ToNumber()
        {
            return JobTitleId;
        }


        public double GetRank(List<WorkExperience> workExperiences)
        {
            if (workExperiences.Count == 0) return 0;


            var starts = workExperiences.OrderBy(e => e.StartDate).ToList();
            var ends = workExperiences.OrderBy(e => e.StartDate).ToList();

            var processings = new List<WorkExperience> { starts[0] };

            double rank = 0;
            var previousDate = starts[0].StartDate;

            for (int i = 1, j = 0; i < starts.Count || j < ends.Count;)
            {
                DateTime currentDate;
                var endDate = ends[j].EndDate.GetValueOrDefault(DateTime.Now);
                if (i < starts.Count && starts[i].StartDate < endDate)
                {
                    currentDate = starts[i].StartDate;
                    rank += processings.Count == 0 ? 0 : HelperFunctions.GetYears(previousDate, currentDate);
                    processings.Add(starts[i++]);
                }
                else
                {
                    currentDate = endDate;
                    rank += HelperFunctions.GetYears(previousDate, currentDate);
                    processings.Remove(ends[j++]);
                }
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
