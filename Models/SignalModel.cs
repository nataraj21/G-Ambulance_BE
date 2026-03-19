using System;

namespace AmbulanceAPI.Models
{
    public class SignalModel
    {
        public int Id { get; set; }
        public string SignalName { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Status { get; set; } = "Green"; // Default to Green
    }
}
