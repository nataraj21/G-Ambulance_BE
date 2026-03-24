using Microsoft.EntityFrameworkCore;
using AmbulanceAPI.Models;

namespace AmbulanceAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<LocationModel> Ambulances { get; set; }
        public DbSet<SignalModel> TrafficSignals { get; set; }
        public DbSet<OfficerLocation> OfficerLocations { get; set; }
        public DbSet<Alert> Alerts { get; set; }
        public DbSet<User> Users { get; set; }
    }
}
