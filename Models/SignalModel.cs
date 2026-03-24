using System;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations.Schema;

namespace AmbulanceAPI.Models
{
    [Table("TrafficSignals")]
    public class SignalModel
    {
        public int Id { get; set; }
        public string SignalName { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Status { get; set; } = "Green"; // Default to Green
    }
}
