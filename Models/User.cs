using System.ComponentModel.DataAnnotations;

namespace AmbulanceAPI.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = "Driver"; // "Driver" or "Officer"

        public string? VehicleNumber { get; set; } // Only for Drivers

        [Required]
        public string FullName { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<OfficerLocation> Locations { get; set; }
        public ICollection<Alert> Alerts { get; set; }
    }
}
