using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Identity.Data;
using ModelServer.Models;
using API_Server.Dto;

namespace API_Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController(UserManager<CarAppUser> userManager, JwtHandler jwtHandler) : ControllerBase
    {
        [HttpPost("Login")]
        public async Task<ActionResult<LoginResult>> LoginAsync(Dto.LoginRequest request)
        {
            var user = await userManager.FindByNameAsync(request.UserName);
            if (user == null)
                return Unauthorized("User not found.");

            var valid = await userManager.CheckPasswordAsync(user, request.Password);
            if (!valid)
                return Unauthorized("Invalid password.");

            var token = await jwtHandler.GetTokenAsync(user);
            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            var roles = await userManager.GetRolesAsync(user);
            var userRole = roles.FirstOrDefault() ?? "User";

            return Ok(new LoginResult
            {
                Success = true,
                Message = "Login successful",
                Token = tokenString,
                Role = userRole,
            });
        }
    }
}
