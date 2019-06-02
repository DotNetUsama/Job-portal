using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Job_Portal_System.Enums
{
    public enum JobType
    {
        [Description("Part time")] [Display(Name = "Part time")] PartTime,
        [Description("Full time")] [Display(Name = "Full time")] FullTime,
        [Description("Internship")] [Display(Name = "Intership")] Internship,
        [Description("Temporary")] [Display(Name = "Temporary")] Temporary,
        [Description("Commission")] [Display(Name = "Commission")] Commission,
        [Description("Contract")] [Display(Name = "Contract")] Contract,
    }

    public static class JobTypeMethods
    {
        public static Dictionary<JobType, bool> GetDictionary()
        {
            return Enum.GetValues(typeof(JobType))
                .Cast<JobType>()
                .ToDictionary(value => value, value => false);
        }

        public static Dictionary<JobType, bool> GetDictionary(List<int> jobTypes)
        {
            return Enum.GetValues(typeof(JobType))
                .Cast<JobType>()
                .ToDictionary(value => value, value => jobTypes.Exists(jobType => jobType == (int) value));
        }
    }
}
