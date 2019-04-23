using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Job_Portal_System.Models
{
    public class JobTitleSimilarity
    {
        public string Id { get; set; }

        public JobTitle JobTitle { get; set; }
        public SimilarJobTitle SimilarTitle { get; set; }
        
    }
}
