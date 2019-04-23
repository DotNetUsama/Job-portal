using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Job_Portal_System.Enums
{
    public enum JobType
    {
        [Description("Part time")] PartTime,
        [Description("Full time")] FullTime,
        [Description("Internship")] Internship,
        [Description("Temporary")] Temporary,
        [Description("Commission")] Commission,
        [Description("Contract")] Contract,
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
