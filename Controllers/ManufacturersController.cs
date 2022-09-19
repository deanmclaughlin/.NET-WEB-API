using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API_Assignment.Data;
using API_Assignment.Models;

namespace API_Assignment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ManufacturersController : ControllerBase
    {
        private readonly DatabaseContext _context;

        public ManufacturersController(DatabaseContext context)
        {
            _context = context;
        }

        // GET: api/Manufacturers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<VehicleManufacturer>>> GetManufacturers()
        {
            if (_context.Manufacturers == null)
            {
                return Problem("Entity set 'DatabaseContext.Manufacturers' is null.");
            }
            return await _context.Manufacturers.ToListAsync();
        }

        // GET: api/Manufacturers/5
        [HttpGet("{ManufacturerID}")]
        public async Task<ActionResult<VehicleManufacturer>> GetVehicleManufacturer(int ManufacturerID)
        {
            if (_context.Manufacturers == null)
            {
                return Problem("Entity set 'DatabaseContext.Manufacturers' is null.");
            }
            int id = ManufacturerID;

            var vehicleManufacturer = await _context.Manufacturers.FindAsync(id);

            if (vehicleManufacturer == null)
            {
                return NotFound("Invalid Manufacturer ID.");
            }

            return vehicleManufacturer;
        }

        // PUT: api/Manufacturers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{ManufacturerID}")]
        public async Task<IActionResult> PutVehicleManufacturer(int ManufacturerID, string? ManufacturerName)
        {
            if (_context.Manufacturers == null)
            {
                return Problem("Entity set 'DatabaseContext.Manufacturers' is null.");
            }
            int id = ManufacturerID;
            if (!VehicleManufacturerExists(id))
            {
                return NotFound("Bad Manufacturer ID.");
            }

            var manufacturer = _context.Manufacturers.Where(man => man.ID == id).Single();
            ManufacturerName = (string.IsNullOrWhiteSpace(ManufacturerName)) ? manufacturer.Name : ManufacturerName.Trim();

            if (ManufacturerNameExists(ManufacturerName??""))
            {
                return BadRequest("Manufacturer is already in the database.");
            }

            manufacturer.Name = ManufacturerName;
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

        // POST: api/Manufacturers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<VehicleManufacturer>> PostVehicleManufacturer(string ManufacturerName)
        {
            if (_context.Manufacturers == null)
            {
                return Problem("Entity set 'DatabaseContext.Manufacturers' is null.");
            }
            if (string.IsNullOrWhiteSpace(ManufacturerName))
            {
                return BadRequest("Manufacturer's name must be filled in.");
            }
            if (ManufacturerNameExists(ManufacturerName.Trim()))
            {
                return BadRequest("Manufacturer is already in the database.");
            }
            try
            {
                _context.Manufacturers.Add(new VehicleManufacturer() { Name = ManufacturerName.Trim() });
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch
            {
                return StatusCode(500);
            }
        }

        // DELETE: api/Manufacturers/5
        [HttpDelete("{ManufacturerID}")]
        public async Task<IActionResult> DeleteVehicleManufacturer(int ManufacturerID)
        {
            if (_context.Manufacturers == null)
            {
                return Problem("Entity set 'DatabaseContext.Manufacturers' is null.");
            }
            int id = ManufacturerID;
            var vehicleManufacturer = await _context.Manufacturers.FindAsync(id);
            if (vehicleManufacturer == null)
            {
                return NotFound("Invalid Manufacturer ID.");
            }

            _context.Manufacturers.Remove(vehicleManufacturer);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool VehicleManufacturerExists(int id)
        {
            return (_context.Manufacturers?.Any(e => e.ID == id)).GetValueOrDefault();
        }
        private bool ManufacturerNameExists(string name)
        {
            return (_context.Manufacturers?.Any(e => e.Name == name)).GetValueOrDefault();
        }
    }
}
