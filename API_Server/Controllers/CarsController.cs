using API_Server.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ModelServer.Models;

namespace API_Server.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CarsController(CarCompanySourceContext context) : ControllerBase
    {
        private readonly CarCompanySourceContext _context = context;

        [HttpGet("cars")]
        public async Task<ActionResult<List<CarDto>>> GetCars(int page = 1, int pageSize = 20)
        {
            var cars = await _context.Cars
                .Include(c => c.CarCompanyNavigation)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new CarDto
                {
                    Id = c.Id,
                    Model = c.Model,
                    Year = c.Year,
                    CarCompanyName = c.CarCompanyNavigation.Name
                })
                .ToListAsync();

            return Ok(cars);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Car>> GetCar(int id)
        {
            var car = await _context.Cars
                .Include(c => c.CarCompanyNavigation)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (car == null)
                return NotFound();

            return Ok(new CarDto
            {
                Id = car.Id,
                Model = car.Model,
                Year = car.Year,
                CarCompanyName = car.CarCompanyNavigation.Name
            });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<CarDto>> PostCar([FromBody] CreateCarDto dto)
        {
            var company = await _context.CarCompanies.FindAsync(dto.CarCompnyId);
            if (company == null)
            {
                return BadRequest("Invalid CarCompanyId.");
            }

            Car car = new()
            {
                Model = dto.Model,
                Year = dto.Year,
                CarCompany = company.Id
            };

            _context.Cars.Add(car);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCar), new { id = car.Id }, new CarDto
            {
                Id = car.Id,
                Model = car.Model,
                Year = car.Year,
                CarCompanyName = company.Name
            });
            
         }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCar(int id, [FromBody] CreateCarDto dto)
        {
            var car = await _context.Cars.FindAsync(id);
            if (car == null)
            {
                return NotFound();
            }

            var company = await _context.CarCompanies.FindAsync(dto.CarCompnyId);
            if (company == null)
            {
                return BadRequest("Invalid CarCompanyId.");
            }

            car.Model = dto.Model;
            car.Year = dto.Year;
            car.CarCompany = company.Id;

            _context.Entry(car).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCar(int id)
        {
            var car = await _context.Cars.FindAsync(id);
            if (car == null)
                return NotFound();

            _context.Cars.Remove(car);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CarExists(int id) =>
            _context.Cars.Any(e => e.Id == id);
    }
}
