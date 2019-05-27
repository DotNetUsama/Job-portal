namespace Job_Portal_System.Models
{
    public class GeoDistance
    {
        public string Id { get; set; }

        public uint Distance { get; set; }

        public City City1 { get; set; }
        public string City1Id { get; set; }

        public City City2 { get; set; }
        public string City2Id { get; set; }
    }
}
