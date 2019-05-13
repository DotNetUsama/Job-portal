namespace Job_Portal_System.Models
{
    public class SeekedJobTitle
    {
        public string Id { get; set; }

        public Resume Resume { get; set; }
        public string ResumeId { get; set; }
        public JobTitle JobTitle { get; set; }
        public long JobTitleId { get; set; }
    }
}
