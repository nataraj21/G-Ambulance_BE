namespace AmbulanceAPI.Models
{
    public class Officer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }

        public ICollection<OfficerLocation> Locations { get; set; }
    }
}