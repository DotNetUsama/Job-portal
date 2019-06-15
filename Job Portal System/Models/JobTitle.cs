using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Job_Portal_System.Models
{
    public class JobTitle
    {
        public long Id { get; set; }
        
        [Required]
        [StringLength(70)]
        [DataType(DataType.Text)]
        public string Title { get; set; }

        [Required]
        [StringLength(70)]
        [DataType(DataType.Text)]
        public string NormalizedTitle { get; set; }

        public JobTitleSynset JobTitleSynset { get; set; }
        public long JobTitleSynsetId { get; set; }
    }
}
