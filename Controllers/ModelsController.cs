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
    public class ModelsController : ControllerBase
    {
        private readonly DatabaseContext _context;

        public ModelsController(DatabaseContext context)
        {
            _context = context;
        }

        // GET: api/Models
        [HttpGet]
        public async Task<ActionResult<IEnumerable<VehicleModel>>> GetModels()
        {
            if (_context.Models == null)
            {
                return Problem("Entity set 'DatabaseContext.Models' is null.");
            }
            return await _context.Models.ToListAsync();
        }

        // GET: api/Models/5
        [HttpGet("{ModelID}")]
        public async Task<ActionResult<VehicleModel>> GetVehicleModel(int ModelID)
        {
            if (_context.Models == null)
            {
                return Problem("Entity set 'DatabaseContext.Models' is null.");
            }
            int id = ModelID;
            var vehicleModel = await _context.Models.FindAsync(id);

            if (vehicleModel == null)
            {
                return NotFound("Bad Model ID.");
            }

            return vehicleModel;
        }

        // PUT: api/Models/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{ModelID}")]
        public async Task<IActionResult> PutVehicleModel(int ModelID, int ManufacturerID, string? ModelName)
        {
            if (_context.Models == null)
            {
                return Problem("Entity set 'DatabaseContext.Models' is null.");
            }
            int id = ModelID;
            if (!VehicleModelExists(id))
            {
                return NotFound("Invalid Model ID.");
            }
            if (ManufacturerID !=0 && !ManufacturerExists(ManufacturerID))
            {
                return NotFound("Invalid Manufacturer ID.");
            }

            var model = _context.Models.Where(mod => mod.ID == id).Single();
            ManufacturerID = (ManufacturerID == 0) ? model.ManufacturerID : ManufacturerID;
            ModelName = (string.IsNullOrWhiteSpace(ModelName)) ? model.Name : ModelName.Trim();

            if (MakeModelExists(ManufacturerID, ModelName??""))
            {
                return BadRequest("This make & model is already in the database.");
            }

            model.ManufacturerID = ManufacturerID;
            model.Name = ModelName;
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

        // POST: api/Models
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<VehicleModel>> PostVehicleModel(int ManufacturerID, string ModelName)
        {
            if (_context.Models == null)
            {
                return Problem("Entity set 'DatabaseContext.Models' is null.");
            }
            if (string.IsNullOrWhiteSpace(ModelName) || ManufacturerID == 0)
            {
                return BadRequest("No field can be left blank.");
            }
            if (!ManufacturerExists(ManufacturerID))
            {
                return NotFound("Invalid Manufacturer ID.");
            }
			if (MakeModelExists(ManufacturerID, ModelName.Trim()))
			{
				return BadRequest("This make & model is already in the database.");
			}

            try
            {
                _context.Models.Add(new VehicleModel() { Name = ModelName.Trim(), ManufacturerID = ManufacturerID });
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch
            {
                return StatusCode(500);
            }
        }
        
        // DELETE: api/Models/5
        [HttpDelete("{ModelID}")]
        public async Task<IActionResult> DeleteVehicleModel(int ModelID)
        {
            if (_context.Models == null)
            {
                return Problem("Entity set 'DatabaseContext.Models' is null.");
            }
            int id = ModelID;
            var vehicleModel = await _context.Models.FindAsync(id);
            if (vehicleModel == null)
            {
                return NotFound("Bad Model ID.");
            }

            _context.Models.Remove(vehicleModel);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool VehicleModelExists(int id)
        {
            return (_context.Models?.Any(e => e.ID == id)).GetValueOrDefault();
        }
        private bool ManufacturerExists(int id)
        {
            return (_context.Manufacturers?.Any(e => e.ID == id)).GetValueOrDefault();
        }
		private bool MakeModelExists(int makeId, string modName)
        {
            return (_context.Manufacturers?.Any(e => e.ID == makeId)).GetValueOrDefault()
			        && (_context.Models?.Any(e => e.Name == modName)).GetValueOrDefault();
        }

    }
}
