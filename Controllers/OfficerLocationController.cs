using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AmbulanceAPI.Data;
using AmbulanceAPI.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace AmbulanceAPI.Controllers
{
    [ApiController]
    [Route("api/officer")]
    public class OfficerLocationController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public OfficerLocationController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("update")]
        public async Task<IActionResult> Update([FromBody] OfficerLocationUpdateModel model, [FromQuery] int? userId = null)
        {
            if (model == null) return BadRequest("Invalid location data.");

            var uIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            int finalUserId;

            if (string.IsNullOrEmpty(uIdString) || !int.TryParse(uIdString, out finalUserId))
            {
                if (userId.HasValue)
                {
                    finalUserId = userId.Value;
                }
                else
                {
                    return BadRequest("User ID is required (via token or query param).");
                }
            }

            var existing = await _context.OfficerLocations.FirstOrDefaultAsync(o => o.UserId == finalUserId);

            if (existing != null)
            {
                existing.Latitude = model.Latitude;
                existing.Longitude = model.Longitude;
                existing.UpdatedTime = DateTime.UtcNow;
            }
            else
            {
                _context.OfficerLocations.Add(new OfficerLocation
                {
                    UserId = finalUserId,
                    Latitude = model.Latitude,
                    Longitude = model.Longitude,
                    UpdatedTime = DateTime.UtcNow
                });
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "Officer location updated.", userId = finalUserId });
        }

        [HttpGet("alerts")]
        public async Task<IActionResult> GetAlerts([FromQuery] int? userId = null)
        {
            var uIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            int finalUserId;

            if (string.IsNullOrEmpty(uIdString) || !int.TryParse(uIdString, out finalUserId))
            {
                if (userId.HasValue)
                {
                    finalUserId = userId.Value;
                }
                else
                {
                    return BadRequest("User ID is required.");
                }
            }

            var alerts = await _context.Alerts
                .Where(a => a.UserId == finalUserId)
                .OrderByDescending(a => a.Timestamp)
                .Take(20)
                .ToListAsync();

            return Ok(alerts);
        }

        [HttpPost("mark-read/{alertId}")]
        public async Task<IActionResult> MarkAsRead(int alertId)
        {
            var alert = await _context.Alerts.FindAsync(alertId);
            if (alert == null) return NotFound();

            alert.IsRead = true;
            await _context.SaveChangesAsync();
            return Ok(new { message = "Alert marked as read." });
        }
    }
}
