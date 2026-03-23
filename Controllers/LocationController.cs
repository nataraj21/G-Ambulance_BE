using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AmbulanceAPI.Data;
using AmbulanceAPI.Models;
using Microsoft.AspNetCore.Http.HttpResults;

namespace AmbulanceAPI.Controllers
{
    [ApiController]
    [Route("api/ambulance")]
    public class LocationController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public LocationController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("update-location")]
        public async Task<IActionResult> UpdateLocation([FromBody] LocationModel model)
        {
            if (model == null)
            {
                return BadRequest("Invalid location data.");
            }

            model.UpdatedTime = DateTime.UtcNow;
            
            _context.Ambulances.Add(model);
            await _context.SaveChangesAsync();

            // Check for proximity to traffic signals
            await CheckProximityAndNotify(model);

            return Ok(new { message = "Location updated successfully.", locationId = model.Id });
        }

        private async Task CheckProximityAndNotify(LocationModel ambulance)
        {
            const double PROXIMITY_THRESHOLD = 1.5; // 1500 meters
            var signals = await _context.TrafficSignals.ToListAsync();

            foreach (var signal in signals)
            {
                double distanceToSignal = GetDistance(ambulance.Latitude, ambulance.Longitude, signal.Latitude, signal.Longitude);

                if (distanceToSignal <= PROXIMITY_THRESHOLD)
                {
                    // Check if an alert was already sent for this vehicle and signal in the last 2 minutes
                    var recentAlert = await _context.Alerts
                        .Where(a => a.VehicleNumber == ambulance.VehicleNumber && 
                                    a.SignalId == signal.Id && 
                                    a.Timestamp > DateTime.UtcNow.AddMinutes(-2))
                        .AnyAsync();

                    if (!recentAlert)
                    {
                        // Find the nearest officer to this signal within 1km
                        var nearestOfficerLocation = _context.OfficerLocations
                            .Include(o => o.Officer)
                            .AsEnumerable()
                            .Where(o => GetDistance(signal.Latitude, signal.Longitude, o.Latitude, o.Longitude) <= 1.0)
                            .OrderBy(o => GetDistance(signal.Latitude, signal.Longitude, o.Latitude, o.Longitude))
                            .FirstOrDefault();

                        if (nearestOfficerLocation != null)
                        {
                            var alert = new Alert
                            {
                                OfficerId = nearestOfficerLocation.OfficerId,
                                VehicleNumber = ambulance.VehicleNumber,
                                SignalId = signal.Id,
                                Message = $"Ambulance {ambulance.VehicleNumber} is approaching {signal.SignalName}.",
                                Timestamp = DateTime.UtcNow,
                                IsRead = false
                            };

                            _context.Alerts.Add(alert);
                            await _context.SaveChangesAsync();
                        }
                    }
                }
            }
        }

        [HttpGet("officer-alerts/{officerId}")]
        public async Task<IActionResult> GetOfficerAlerts(int officerId)
        {
            var alerts = await _context.Alerts
                .Where(a => a.OfficerId == officerId)
                .OrderByDescending(a => a.Timestamp)
                .Take(20)
                .ToListAsync();

            return Ok(alerts);
        }

        [HttpPost("mark-alert-read/{alertId}")]
        public async Task<IActionResult> MarkAlertAsRead(int alertId)
        {
            var alert = await _context.Alerts.FindAsync(alertId);
            if (alert == null) return NotFound();

            alert.IsRead = true;
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpGet("all-locations")]
        public async Task<ActionResult<IEnumerable<LocationModel>>> GetAllLocations()
        {
            // For live tracking, we normally only want the latest position of each vehicle.
            // But the frontend filters it, so we'll just return all for now.
            // In a real app, you'd probably return only the latest record per vehicle.
            var result = await _context.Ambulances
                .OrderByDescending(l => l.UpdatedTime)
                .Take(100)
                .ToListAsync();
            return Ok(result);
                
        }

        [HttpGet("location")]
        public async Task<ActionResult<LocationModel>> GetLocation([FromQuery] string vehicleNumber)
        {
            var location = await _context.Ambulances
                .Where(l => l.VehicleNumber == vehicleNumber)
                .OrderByDescending(l => l.UpdatedTime)
                .FirstOrDefaultAsync();

            if (location == null)
            {
                return NotFound("Location not found for the specified vehicle.");
            }

            return location;
        }

        private double GetDistance(double lat1, double lon1, double lat2, double lon2)
        {
            var r = 6371.0; // Radius of the Earth in km
            var dLat = ToRadians(lat2 - lat1);
            var dLon = ToRadians(lon2 - lon1);
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return r * c;
        }

        private double ToRadians(double angle)
        {
            return Math.PI * angle / 180.0;
        }
    }
}
