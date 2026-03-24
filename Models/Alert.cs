using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AmbulanceAPI.Models
{
    [Table("Alerts")]
    public class Alert
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }
        public string VehicleNumber { get; set; } = string.Empty;
        public int SignalId { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public bool IsRead { get; set; } = false;

        [ForeignKey("UserId")]
        public User User { get; set; }

        [ForeignKey("SignalId")]
        public SignalModel Signal { get; set; }
    }
}
