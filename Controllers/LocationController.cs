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

            return Ok(new { message = "Location updated successfully.", locationId = model.Id });
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
    }
}
