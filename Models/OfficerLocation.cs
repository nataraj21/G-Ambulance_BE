namespace AmbulanceAPI.Models
{
    public class OfficerLocation
    {
        public int Id { get; set; }
        public int OfficerId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime UpdatedTime { get; set; }

        public Officer Officer { get; set; }
    }
}