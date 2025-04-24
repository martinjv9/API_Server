using API_Server.Dto;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ModelServer.Models;
using System.Globalization;

namespace API_Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeedController(CarCompanySourceContext context, IHostEnvironment env, UserManager<CarAppUser> userManager) : ControllerBase
    {
        private readonly string _csvPath = Path.Combine(env.ContentRootPath, "Data", "CarData_1000.csv");
        private readonly CarCompanySourceContext _context = context;
        private readonly UserManager<CarAppUser> _userManager = userManager;

        [HttpPost("Import")]
        public async Task<ActionResult> ImportCsvAsync()
        {
            if (!System.IO.File.Exists(_csvPath))
                return NotFound("CSV file not found.");

            CsvConfiguration config = new(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                HeaderValidated = null
            };

            Dictionary<string, CarCompany> companies = context.CarCompanies
                .AsNoTracking()
                .ToDictionary(c => c.Name, StringComparer.OrdinalIgnoreCase);

            int carCount = 0;

            using StreamReader reader = new(_csvPath);
            using CsvReader csv = new(reader, config);
            List<CarCsvDto> records = csv.GetRecords<CarCsvDto>().ToList();

            foreach (var record in records)
            {
                if (!companies.TryGetValue(record.CarCompany, out var company))
                {
                    company = new CarCompany
                    {
                        Name = record.CarCompany,
                        CountryOrigin = record.CountryOrigin
                    };
                    context.CarCompanies.Add(company);
                    companies.Add(record.CarCompany, company);
                }

                var car = new Car
                {
                    Model = record.Model,
                    Year = record.Year,
                    CarCompanyNavigation = company
                };

                context.Cars.Add(car);
                carCount++;
            }

            await context.SaveChangesAsync();
            return Ok($"{carCount} cars imported.");
        }

        [HttpPost("admin")]
        public async Task<IActionResult> SeedAdminAsync()
        {
            var adminEmail = "admin@example.com";
            var adminPassword = "Admin@1234";

            var existingUser = await _userManager.FindByEmailAsync(adminEmail);
            if (existingUser != null)
            {
                return Ok("Admin already exists.");
            }

            var user = new CarAppUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, adminPassword);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok("Admin user created successfully.");
        }
    }
}
