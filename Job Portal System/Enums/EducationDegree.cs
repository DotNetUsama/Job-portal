using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Job_Portal_System.Enums
{
    public enum EducationDegree
    {
        [Display(Name = "Other")]
        Other = 1,
        [Display(Name = "High school")]
        HighSchool = 2,
        [Display(Name = "Associate")]
        Associate = 3,
        [Display(Name = "Bachelore")]
        Bachelore = 4,
        [Display(Name = "Master")]
        Master = 5,
        [Display(Name = "Dectorate")]
        Dectorate = 6,
    }
}
