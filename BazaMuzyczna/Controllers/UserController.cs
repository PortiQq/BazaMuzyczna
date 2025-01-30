using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BazaMuzyczna.Models;
using BazaMuzyczna.DataTransferObjects;
using Microsoft.AspNetCore.Authorization;

namespace BazaMuzyczna.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UserController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/User
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetUser()
        {
            var users = await _context.User
                .Select(u => new UserDTO
                {
                    Id = u.Id,
                    Name = u.Name,
                    Email = u.Email,
                    Albums = u.Albums.Select(a => new AlbumNameDTO
                    {
                        Name = a.Name
                    }).ToList()
                }).ToListAsync();

            return users;
        }

        // GET: api/User/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDTO>> GetUser(int id)
        {

            var user = await _context.User
                   .Where(u => u.Id == id)
                   .Select(u => new UserDTO
                   {
                       Id = u.Id,
                       Name = u.Name,
                       Email = u.Email,
                       Albums = u.Albums.Select(a => new AlbumNameDTO
                       {
                           Name = a.Name
                       }).ToList()
                   })
                   .FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // PUT: api/User
        [Authorize]
        [HttpPut("EditAccount")]
        public async Task<IActionResult> PutUser(UserRequestDTO updatedUser)
        {

            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim == null)
            {
                return Unauthorized(new { message = "Invalid token: UserId not found" });
            }

            int userId = int.Parse(userIdClaim.Value);

            var user = await _context.User.FindAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            user.Name = updatedUser.Name;
            user.Email = updatedUser.Email;
            user.Password = updatedUser.Password;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(userId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/User/register
        [HttpPost("register")]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            _context.User.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", user);
        }

        // DELETE: api/User/delete
        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteUser()
        {
            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim == null)
            {
                return Unauthorized(new { message = "Invalid token: UserId not found" });
            }

            int userId = int.Parse(userIdClaim.Value);

            var user = await _context.User.FindAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            _context.User.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(int id)
        {
            return _context.User.Any(e => e.Id == id);
        }
    }
}
