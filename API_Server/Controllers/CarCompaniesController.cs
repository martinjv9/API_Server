using API_Server.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ModelServer.Models;

namespace API_Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarCompaniesController(CarCompanySourceContext context) : ControllerBase
    {
        private readonly CarCompanySourceContext _context = context;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CarCompany>>> GetCarCompanies()
        {
            var companies = await _context.CarCompanies
                .Select(c => new CarCompanyDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    CountryOrigin = c.CountryOrigin
                })
                .ToListAsync();

            return Ok(companies);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<CarCompany>> GetCarCompany(int id)
        {
            var company = await _context.CarCompanies
               .Include(c => c.Cars)
               .FirstOrDefaultAsync(c => c.Id == id);

            if (company == null)
                return NotFound();

            return Ok(new CarCompanyDto
            {
                Id = company.Id,
                Name = company.Name,
                CountryOrigin = company.CountryOrigin
            });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<CarCompanyDto>> PostCarCompany([FromBody] CreateCarCompanyDto dto)
        {
            var company = new CarCompany
            {
                Name = dto.Name,
                CountryOrigin = dto.CountryOrigin
            };

            _context.CarCompanies.Add(company);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCarCompany), new { id = company.Id }, new CarCompanyDto
            {
                Id = company.Id,
                Name = company.Name,
                CountryOrigin = company.CountryOrigin
            });
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCarCompany(int id, [FromBody] CreateCarCompanyDto dto)
        {
            var company = await _context.CarCompanies.FindAsync(id);
            if (company == null)
                return NotFound();

            company.Name = dto.Name;
            company.CountryOrigin = dto.CountryOrigin;

            _context.Entry(company).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCarCompany(int id)
        {
            var company = await _context.CarCompanies.FindAsync(id);
            if (company == null)
                return NotFound();

            _context.CarCompanies.Remove(company);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CarCompanyExists(int id) =>
            _context.CarCompanies.Any(e => e.Id == id);
    }
}
