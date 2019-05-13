namespace Job_Portal_System.ViewModels
{
    public abstract class AbstractSearchResultViewModel
    {
        public string Query { get; set; }

        public int TotalPages { get; set; }
        public int PageNumber { get; set; }
        public bool IsFirst { get; set; }
        public bool IsLast { get; set; }
    }
}
