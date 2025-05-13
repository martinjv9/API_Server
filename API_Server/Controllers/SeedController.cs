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
    public class SeedController(
        CarCompanySourceContext context,
        IHostEnvironment env,
        UserManager<CarAppUser> userManager,
        RoleManager<IdentityRole> roleManager) : ControllerBase
    {
        private readonly string _csvPath = Path.Combine(env.ContentRootPath, "Data", "CarData_1000.csv");
        private readonly CarCompanySourceContext _context = context;
        private readonly IHostEnvironment _env = env;
        private readonly UserManager<CarAppUser> _userManager = userManager;
        private readonly RoleManager<IdentityRole> _roleManager = roleManager;

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
            var userName = "admin";
            var adminEmail = "admin@email.com";
            var adminPassword = "Passw0rd!";

            if (!await _roleManager.RoleExistsAsync("Admin"))
            {
                var roleResult = await _roleManager.CreateAsync(new IdentityRole("Admin"));
                if (!roleResult.Succeeded)
                {
                    return BadRequest("Failed to create Admin role.");
                }
            }

            var existingUser = await _userManager.FindByEmailAsync(adminEmail);
            if (existingUser != null)
            {
                return Ok("Admin already exists.");
            }

            var user = new CarAppUser
            {
                UserName = userName,
                Email = adminEmail,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, adminPassword);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }
            await _userManager.AddToRoleAsync(user, "Admin");


            return Ok("Admin user created successfully.");
        }

        [HttpPost("user")]
        public async Task<IActionResult> SeedUserAsync()
        {
            var userName = "user";
            var userEmail = "user@email.com";
            var userPassword = "Passw0rd!";

            if (!await _roleManager.RoleExistsAsync("User"))
            {
                var roleResult = await _roleManager.CreateAsync(new IdentityRole("User"));
                if (!roleResult.Succeeded)
                {
                    return BadRequest("Failed to create User role.");
                }
            }

            var existingUser = await _userManager.FindByEmailAsync(userEmail);
            if (existingUser != null)
            {
                return Ok("Regular user already exists.");
            }

            var user = new CarAppUser
            {
                UserName = userName,
                Email = userEmail,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, userPassword);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }
            await _userManager.AddToRoleAsync(user, "User");


            return Ok("Regular user created successfully.");
        }

        [HttpDelete("delete-user")]
        public async Task<IActionResult> DeleteUserAsync([FromQuery] string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return BadRequest("Email is required.");

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return NotFound("User not found.");

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                return BadRequest("Failed to delete user.");
            }

            return Ok($"User '{email}' has been deleted.");
        }

    }
}
