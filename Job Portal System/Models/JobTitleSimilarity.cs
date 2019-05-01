namespace Job_Portal_System.Models
{
    public class JobTitleSimilarity
    {
        public string Id { get; set; }

        public JobTitle JobTitle { get; set; }
        public SimilarJobTitle SimilarTitle { get; set; }
        
    }
}
