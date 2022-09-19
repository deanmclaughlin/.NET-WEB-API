using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API_Assignment.Data;
using API_Assignment.Models;
using System.Text.RegularExpressions;

namespace API_Assignment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VehiclesController : ControllerBase
    {
        private readonly DatabaseContext _context;

        public VehiclesController(DatabaseContext context)
        {
            _context = context;
        }

        // GET: api/Vehicles
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Vehicle>>> GetVehicles()
        {
            if (_context.Vehicles == null)
            {
                return Problem("Entity set 'DatabaseContext.Vehicles' is null.");
            }
            return await _context.Vehicles.ToListAsync();
        }

        // GET: api/Vehicles/5
        [HttpGet("{VIN}")]
        public async Task<ActionResult<Vehicle>> GetVehicle(string VIN)
        {
            if (_context.Vehicles == null)
            {
                return Problem("Entity set 'DatabaseContext.Vehicles' is null.");
            }
            string vin = VIN.Trim().ToUpper();
            var vehicle = await _context.Vehicles.FindAsync(vin);

            if (vehicle == null)
            {
                return NotFound("Bad VIN provided.");
            }

            return vehicle;
        }

        // PUT: api/Vehicles/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{VIN}")]
        public async Task<IActionResult> PutVehicle(string VIN, int ModelID, int DealerID, string? TrimLevel)
        {
            if (_context.Vehicles == null)
            {
                return Problem("Entity set 'DatabaseContext.Vehicles' is null.");
            }
            string vin = VIN.Trim().ToUpper();
            if (!VehicleExists(vin))
            {
                return NotFound("Bad VIN.");
            }
            if (ModelID != 0 && !ModelExists(ModelID))
            {
                return NotFound("Invalid ModelID.");
            }
            if (DealerID != 0 && !DealerExists(DealerID))
            {
                return NotFound("Invalid Dealership ID.");
            }

            var vehicle = _context.Vehicles.Where(car => car.VIN == vin).Single();
            vehicle.ModelID = (ModelID == 0) ? vehicle.ModelID : ModelID;
            vehicle.DealershipID = (DealerID == 0) ? vehicle.DealershipID : DealerID;
            vehicle.TrimLevel = (string.IsNullOrWhiteSpace(TrimLevel)) ? vehicle.TrimLevel : TrimLevel.Trim();

            try
            {
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch
            {
                return StatusCode(500);
            }
        }

        // POST: api/Vehicles
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Vehicle>> PostVehicle(string VIN, int ModelID, int DealershipID, string TrimLevel)
        {
            VIN = VIN.Trim().ToUpper();
            if (_context.Vehicles == null)
            {
                return Problem("Entity set 'DatabaseContext.Vehicles' is null.");
            }
            if (VehicleExists(VIN))
            {
                return BadRequest("Vehicle with this VIN already exists.");
            }
            if (!ModelExists(ModelID))
            {
                return NotFound("Invalid Model ID.");
            }
            if (!DealerExists(DealershipID))
            {
                return NotFound("Invalid Dealership ID.");
            }
            if (VIN.Length != 17 || !(new Regex(@"^[A-Z0-9]+$")).IsMatch(VIN))
            {
                return BadRequest("VIN must be 17 characters (letters and digits ONLY).");
            }

            try
            {
                _context.Vehicles.Add(new Vehicle()
                    { VIN = VIN, ModelID = ModelID, DealershipID = DealershipID, TrimLevel = TrimLevel.Trim() }
                );
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch
            {
                return StatusCode(500);
            }
        }

        // DELETE: api/Vehicles/5
        [HttpDelete("{VIN}")]
        public async Task<IActionResult> DeleteVehicle(string VIN)
        {
            if (_context.Vehicles == null)
            {
                return Problem("Entity set 'DatabaseContext.Vehicles' is null.");
            }
            string vin = VIN.Trim().ToUpper();
            var vehicle = await _context.Vehicles.FindAsync(vin);
            if (vehicle == null)
            {
                return NotFound("Bad VIN provided.");
            }

            _context.Vehicles.Remove(vehicle);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool VehicleExists(string id)
        {
            return (_context.Vehicles?.Any(e => e.VIN == id)).GetValueOrDefault();
        }
        private bool ModelExists(int id)
        {
            return (_context.Models?.Any(e => e.ID == id)).GetValueOrDefault();
        }
        private bool DealerExists(int id)
        {
            return (_context.Dealerships?.Any(e => e.ID == id)).GetValueOrDefault();
        }

    }
}
