using BazaMuzyczna.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using BazaMuzyczna.DataTransferObjects;
using Microsoft.EntityFrameworkCore;
using BazaMuzyczna.Models;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;

namespace BazaMuzyczna.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly JwtService _jwtService;
        private readonly AppDbContext _context;

        public AuthController(JwtService jwtService, AppDbContext context)
        {
            _jwtService = jwtService;
            _context = context;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO request)
        {
            // Pobierz użytkownika z bazy danych na podstawie nazwy użytkownika
            var user = await _context.User.FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null)
            {
                return Unauthorized(new { Message = "Invalid username or password" });
            }

            if (!(user.Password == request.Password))
            {
                return Unauthorized(new { Message = "Invalid username or password" });
            }

            var token = _jwtService.GenerateToken(user.Name, user.Id);
            return Ok(new { Token = token });
        }
    }
}
