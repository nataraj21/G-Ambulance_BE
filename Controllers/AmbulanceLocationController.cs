using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AmbulanceAPI.Data;
using AmbulanceAPI.Models;
using AmbulanceAPI.Helpers;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace AmbulanceAPI.Controllers
{
    [ApiController]
    [Route("api/ambulance")]
    public class AmbulanceLocationController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IServiceScopeFactory _scopeFactory;

        public AmbulanceLocationController(ApplicationDbContext context, IServiceScopeFactory scopeFactory)
        {
            _context = context;
            _scopeFactory = scopeFactory;
        }

        [HttpPost("update")]
        public async Task<IActionResult> Update([FromBody] LocationUpdateModel model, [FromQuery] string? vehicleNumber = null)
        {
            if (model == null) return BadRequest("Invalid location data.");

            // Try to get vehicle number from claims (JWT), query param, or the request body
            var vNumber = User.Claims.FirstOrDefault(c => c.Type == "VehicleNumber")?.Value?.Trim() 
                          ?? vehicleNumber?.Trim()
                          ?? model.VehicleNumber?.Trim();

            if (string.IsNullOrEmpty(vNumber))
            {
                return BadRequest("Vehicle number is required (via token or query param).");
            }

            var newLocation = new LocationModel
            {
                VehicleNumber = vNumber,
                Latitude = model.Latitude,
                Longitude = model.Longitude,
                Speed = model.Speed,
                UpdatedTime = DateTime.UtcNow
            };

            try
            {
                _context.Ambulances.Add(newLocation);
                await _context.SaveChangesAsync();
                
                // Fire and forget proximity check safely using a new scope
                _ = Task.Run(async () => await CheckProximityAndNotify(newLocation.Id));

                return Ok(new { message = "Ambulance location stored.", vehicle = vNumber });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error storing location: {ex.Message}");
            }
        }

        [HttpGet("live/{vehicleNumber}")]
        public async Task<ActionResult<LocationModel>> GetLiveLocation(string vehicleNumber)
        {
            if (string.IsNullOrEmpty(vehicleNumber)) return BadRequest("Vehicle number is required.");
            
            var trimmedVehicleNumber = vehicleNumber.Trim();
            var location = await _context.Ambulances
                .Where(l => l.VehicleNumber == trimmedVehicleNumber)
                .OrderByDescending(l => l.UpdatedTime)
                .FirstOrDefaultAsync();

            if (location == null) return NotFound("Location not found.");

            return Ok(location);
        }

        [HttpGet("all-live")]
        public async Task<ActionResult<IEnumerable<LocationModel>>> GetAllLiveLocations()
        {
            var result = await _context.Ambulances
                .GroupBy(l => l.VehicleNumber)
                .Select(g => g.OrderByDescending(l => l.UpdatedTime).FirstOrDefault())
                .ToListAsync();
            
            return Ok(result);
        }

        private async Task CheckProximityAndNotify(int locationId)
        {
            try
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var ambulance = await db.Ambulances.FindAsync(locationId);
                    if (ambulance == null) return;

                    const double PROXIMITY_THRESHOLD = 1.5; // 1.5 km
                    var signals = await db.TrafficSignals.ToListAsync();

                    foreach (var signal in signals)
                    {
                        double dist = GeoHelper.GetDistance(ambulance.Latitude, ambulance.Longitude, signal.Latitude, signal.Longitude);
                        if (dist <= PROXIMITY_THRESHOLD)
                        {
                            var recentAlert = await db.Alerts.AnyAsync(a => 
                                a.VehicleNumber == ambulance.VehicleNumber && 
                                a.SignalId == signal.Id && 
                                a.Timestamp > DateTime.UtcNow.AddMinutes(-2));

                            if (!recentAlert)
                            {
                                // Find officer near signal (limit to 1km)
                                var nearestOfficer = db.OfficerLocations
                                    .Include(o => o.User)
                                    .AsEnumerable()
                                    .Where(o => GeoHelper.GetDistance(signal.Latitude, signal.Longitude, o.Latitude, o.Longitude) <= 1.0)
                                    .OrderBy(o => GeoHelper.GetDistance(signal.Latitude, signal.Longitude, o.Latitude, o.Longitude))
                                    .FirstOrDefault();

                                if (nearestOfficer != null)
                                {
                                    db.Alerts.Add(new Alert
                                    {
                                        UserId = nearestOfficer.UserId,
                                        VehicleNumber = ambulance.VehicleNumber,
                                        SignalId = signal.Id,
                                        Message = $"Ambulance {ambulance.VehicleNumber} is approaching {signal.SignalName}.",
                                        Timestamp = DateTime.UtcNow,
                                        IsRead = false
                                    });
                                    await db.SaveChangesAsync();
                                }
                            }
                        }
                    }
                }
            }
            catch { /* Ignore background errors */ }
        }
    }
}
