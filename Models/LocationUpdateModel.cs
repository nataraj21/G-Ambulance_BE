namespace AmbulanceAPI.Models
{
    public class LocationUpdateModel
    {
        public string? VehicleNumber { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Speed { get; set; }
    }
}
