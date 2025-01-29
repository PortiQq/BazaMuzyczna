using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BazaMuzyczna.Models;
using Microsoft.AspNetCore.Authorization;
using BazaMuzyczna.DataTransferObjects;

namespace BazaMuzyczna.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AlbumController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AlbumController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Album
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Album>>> GetAlbum()
        {
            try
            {
                var albums = await _context.Album
                    .Include(a => a.User)
                    .Include(a => a.Tracks)
                    .Select(a => new AlbumDTO
                    {
                        Name = a.Name,
                        ReleaseDate = a.ReleaseDate,
                        User = new UserDTO
                        {
                            Name = a.User.Name,
                        },
                        Tracks = a.Tracks.Select(t => new TrackDTO
                        {
                            Title = t.Title,
                            Duration = t.Duration
                        }).ToList()
                    })
            .ToListAsync();

                return Ok(albums);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "An error occurred", details = ex.Message });
            }
        }

        // GET: api/Album/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Album>> GetAlbum(int id)
        {
            try
            {
                var album = await _context.Album
                    .Where(a => a.Id == id)
                    .Include(a => a.User)
                    .Include(a => a.Tracks)
                    .Select(a => new AlbumDTO
                    {
                        Name = a.Name,
                        ReleaseDate = a.ReleaseDate,
                        User = new UserDTO
                        {
                            Name = a.User.Name,
                        },
                        Tracks = a.Tracks.Select(t => new TrackDTO
                        {
                            Title = t.Title,
                            Duration = t.Duration
                        }).ToList()
                    })
                    .FirstOrDefaultAsync();

                if (album == null)
                {
                    return NotFound(new { message = "Album not found" });
                }

                return Ok(album);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "An error occurred", details = ex.Message });
            }
        }

        // GET: api/MyAlbums
        [Authorize]
        [HttpGet("MyAlbums")]
        public async Task<ActionResult<IEnumerable<Album>>> GetMyAlbums()
        {
            try
            {
                var userIdClaim = User.FindFirst("UserId");
                if (userIdClaim == null)
                {
                    return Unauthorized(new { message = "Invalid token: UserId not found" });
                }

                int userId = int.Parse(userIdClaim.Value);

                var albums = await _context.Album
                    .Where(a => a.UserId == userId)
                    .Include(a => a.Tracks)
                    .Select(a => new AlbumDTO
                    {
                    Name = a.Name,
                    ReleaseDate = a.ReleaseDate,
                    User = new UserDTO
                    {
                        Name = a.User.Name,
                    },
                        Tracks = a.Tracks.Select(t => new TrackDTO
                    {
                    Title = t.Title,
                    Duration = t.Duration
                }).ToList()
            })
            .ToListAsync();

            return Ok(albums);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "An error occurred", details = ex.Message });
            }
        }

        //TODO: Autoryzacja własności albumu
        // PUT: api/Album/5
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAlbum(int id, AlbumRequestDTO updatedAlbum)
        { 
            var album = await _context.Album.FindAsync(id);
            if (album == null)
            {
                return NotFound();
            }

            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim == null)
            {
                return Unauthorized(new { message = "Invalid token: UserId not found" });
            }

            int userId = int.Parse(userIdClaim.Value);

            if (album.UserId != userId)
            {
                return Forbid(); // 403 Forbidden
            }

            album.Name = updatedAlbum.Name;
            album.ReleaseDate = updatedAlbum.ReleaseDate;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AlbumExists(id))
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

        // Autoryzacja użytkownika
        // POST: api/Album/add
        [Authorize]
        [HttpPost("add")]
        public async Task<ActionResult<Album>> PostAlbum(AlbumRequestDTO albumRequest)
        {

            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim == null)
            {
                return Unauthorized(new { message = "Invalid token: UserId not found" });
            }

            int userId = int.Parse(userIdClaim.Value);

            var album = new Album
            {
                Name = albumRequest.Name,
                ReleaseDate = albumRequest.ReleaseDate,
                UserId = userId
            };

            _context.Album.Add(album);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAlbum), album);
        }

        // Autoryzacja użytkownika
        // DELETE: api/Album/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAlbum(int id)
        {

            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim == null)
            {
                return Unauthorized(new { message = "Invalid token: UserId not found" });
            }

            int userId = int.Parse(userIdClaim.Value);


            var album = await _context.Album.FindAsync(id);
            if (album == null)
            {
                return NotFound();
            }

            if (album.UserId != userId)
            {
                return Forbid(); // 403 Forbidden
            }

            _context.Album.Remove(album);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AlbumExists(int id)
        {
            return _context.Album.Any(e => e.Id == id);
        }
    }
}
