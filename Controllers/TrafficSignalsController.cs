using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AmbulanceAPI.Data;
using AmbulanceAPI.Models;

namespace AmbulanceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TrafficSignalsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TrafficSignalsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SignalModel>>> GetSignals()
        {
            var signals = await _context.TrafficSignals.ToListAsync();
            
            // If database is empty, seed some default signals for demo purposes
            if (signals.Count == 0)
            {
                var defaultSignals = new List<SignalModel>
                {
                    new SignalModel { SignalName = "Puducherry Central", Latitude = 11.9344, Longitude = 79.8300, Status = "Green" },
                    new SignalModel { SignalName = "Anna Salai Junction", Latitude = 11.9310, Longitude = 79.8250, Status = "Red" },
                    new SignalModel { SignalName = "Beach Road Crossing", Latitude = 11.9330, Longitude = 79.8350, Status = "Green" }
                };
                
                _context.TrafficSignals.AddRange(defaultSignals);
                await _context.SaveChangesAsync();
                signals = defaultSignals;
            }
            
            return signals;
        }

        [HttpPost("update-status")]
        public async Task<IActionResult> UpdateSignalStatus(int id, string status)
        {
            var signal = await _context.TrafficSignals.FindAsync(id);
            if (signal == null) return NotFound();

            signal.Status = status;
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
