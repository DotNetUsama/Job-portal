using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Job_Portal_System.Models
{
    public class FieldOfStudySimilarity
    {
        public string Id { get; set; }

        public FieldOfStudy FieldOfStudy { get; set; }
        public SimilarFieldOfStudyTitle SimilarTitle { get; set; }
    }
}
