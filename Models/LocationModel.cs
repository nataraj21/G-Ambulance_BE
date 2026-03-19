using System;

namespace AmbulanceAPI.Models
{
    public class LocationModel
    {
        public int Id { get; set; }
        public string VehicleNumber { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Speed { get; set; }
        public DateTime UpdatedTime { get; set; }
    }
}
