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
    public class DealershipsController : ControllerBase
    {
        private readonly DatabaseContext _context;

        public DealershipsController(DatabaseContext context)
        {
            _context = context;
        }

        // GET: api/Dealerships
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Dealership>>> GetDealerships()
        {
            if (_context.Dealerships == null)
            {
                return Problem("Entity set 'DatabaseContext.Dealerships' is null.");
            }
            return await _context.Dealerships.ToListAsync();
        }

        // GET: api/Dealerships/5
        [HttpGet("{DealerID}")]
        public async Task<ActionResult<Dealership>> GetDealership(int DealerID)
        {
            if (_context.Dealerships == null)
            {
                return Problem("Entity set 'DatabaseContext.Dealerships' is null.");
            }
            int id = DealerID;
            var dealership = await _context.Dealerships.FindAsync(id);

            if (dealership == null)
            {
                return NotFound("Bad Dealership ID.");
            }

            return dealership;
        }

        // PUT: api/Dealerships/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{DealerID}")]
        public async Task<IActionResult> PutDealership(int DealerID, string? DealerName, int ManufacturerID,
                                                       string? DealerAddress, string? DealerPhone)
        {
            if (_context.Dealerships == null)
            {
                return Problem("Entity set 'DatabaseContext.Dealerships' is null.");
            }
            int id = DealerID;
            if (!DealershipExists(id))
            {
                return NotFound("Invalid Dealership ID.");
            }
            if (ManufacturerID!=0 && !ManufacturerExists(ManufacturerID))
            {
                return NotFound("Invalid Manufacturer ID.");
            }
            if (!string.IsNullOrWhiteSpace(DealerPhone) &&
                (DealerPhone.Trim().Length != 10 || !Int64.TryParse(DealerPhone.Trim(), out long phoneNo)))
            {
                return BadRequest("Phone number must be 10 digits, no spaces or other characters.");
            }

            var dealership = _context.Dealerships.Where(dealer => dealer.ID == id).Single();  
            DealerName = (string.IsNullOrWhiteSpace(DealerName)) ? dealership.Name : DealerName.Trim();
            ManufacturerID = (ManufacturerID == 0) ? dealership.ManufacturerID : ManufacturerID;
            DealerAddress = (string.IsNullOrWhiteSpace(DealerAddress)) ? dealership.Address : DealerAddress.Trim();
            DealerPhone = (string.IsNullOrWhiteSpace(DealerPhone)) ? dealership.PhoneNumber : DealerPhone.Trim();

            if (DuplicateDealer(DealerName??"", ManufacturerID, DealerAddress??"", DealerPhone??""))
            {
                return BadRequest("This dealership is already in the database.");
            }

            dealership.Name = DealerName;
            dealership.ManufacturerID = ManufacturerID;
            dealership.Address = DealerAddress;
            dealership.PhoneNumber = DealerPhone;
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

        // POST: api/Dealerships
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Dealership>> PostDealership(string DealershipName, int ManufacturerID,
                                                                   string DealershipAddress, string DealershipPhone)
        {
            if (_context.Dealerships == null)
            {
                return Problem("Entity set 'DatabaseContext.Dealerships' is null.");
            }
            if (string.IsNullOrWhiteSpace(DealershipName) || ManufacturerID == 0 ||
                string.IsNullOrWhiteSpace(DealershipAddress) || string.IsNullOrWhiteSpace(DealershipPhone))
            {
                return BadRequest("No field can be left blank.");
            }
            if (!ManufacturerExists(ManufacturerID))
            {
                return NotFound("Invalid Manufacturer ID.");
            }
            if (DuplicateDealer(DealershipName.Trim(), ManufacturerID, DealershipAddress.Trim(), DealershipPhone.Trim()))
            {
                return BadRequest("This dealership is already in the database.");
            }
            if (DealershipPhone.Trim().Length != 10 || !Int64.TryParse(DealershipPhone.Trim(), out long phoneNo))
            {
                return BadRequest("Phone number must be 10 digits, no spaces or other characters.");
            }

            try
            {
                _context.Dealerships.Add(new Dealership()
                    { Name = DealershipName.Trim(), ManufacturerID = ManufacturerID,
                      Address = DealershipAddress.Trim(), PhoneNumber = DealershipPhone.Trim() }
                );
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch
            {
                return StatusCode(500);
            }
        }

        // DELETE: api/Dealerships/5
        [HttpDelete("{DealerID}")]
        public async Task<IActionResult> DeleteDealership(int DealerID)
        {
            if (_context.Dealerships == null)
            {
                return Problem("Entity set 'DatabaseContext.Dealerships' is null.");
            }
            int id = DealerID;
            var dealership = await _context.Dealerships.FindAsync(id);
            if (dealership == null)
            {
                return NotFound("Bad Dealership ID.");
            }

            _context.Dealerships.Remove(dealership);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DealershipExists(int id)
        {
            return (_context.Dealerships?.Any(e => e.ID == id)).GetValueOrDefault();
        }
        private bool DuplicateDealer(string dealName, int manId, string dealAdd, string dealPhone)
        {
            return (_context.Dealerships?.Any(e => e.Name == dealName)).GetValueOrDefault()
                   && (_context.Dealerships?.Any(e => e.ManufacturerID == manId)).GetValueOrDefault()
                   && (_context.Dealerships?.Any(e => e.Address == dealAdd)).GetValueOrDefault()
                   && (_context.Dealerships?.Any(e => e.PhoneNumber == dealPhone)).GetValueOrDefault();
        }
        private bool ManufacturerExists(int id)
        {
            return (_context.Manufacturers?.Any(e => e.ID == id)).GetValueOrDefault();
        }
    }
}
