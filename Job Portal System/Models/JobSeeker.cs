
namespace Job_Portal_System.Models
{
    public class JobSeeker
    {
        public string Id { get; set; }
        
        public bool IsSeeking { get; set; } = true;

        public User User { get; set; }
        public string UserId { get; set; }
    }
}
