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
    public class TrackController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TrackController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Track
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Track>>> GetTrack()
        {
            try
            {
                var tracks = await _context.Track
                    .Select(t => new TrackDetailsDTO
                    {
                        Id = t.Id,
                        Title = t.Title,
                        Duration = t.Duration,
                        AlbumId = t.AlbumId,
                        Album = new AlbumNameDTO
                        {
                            Name = t.Album.Name,
                        },
                        Genre = new GenreNameDTO
                        {
                            Name = t.Genre.Name
                        }
                    }).ToListAsync();

                return Ok(tracks);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "An error occurred", details = ex.Message });
            }
        }

        // GET: api/Track/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TrackDetailsDTO>> GetTrack(int id)
        {
            try
            {
                var track = await _context.Track
                    .Where(t => t.Id == id)
                    .Select(t => new TrackDetailsDTO
                    {
                        Id = t.Id,
                        Title = t.Title,
                        Duration = t.Duration,
                        AlbumId = t.AlbumId,
                        Album = new AlbumNameDTO
                        {
                            Name = t.Album.Name,
                        },
                        Genre = new GenreNameDTO
                        {
                            Name = t.Genre.Name
                        }
                    })
                    .FirstOrDefaultAsync();

                if (track == null)
                {
                    return NotFound(new { message = "Track not found" });
                }

                return Ok(track);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "An error occurred", details = ex.Message });
            }
        }

        // PUT: api/Track/5
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTrack(int id, TrackRequestDTO updatedTrack)
        {
            // Pobranie tracka wraz z albumem, aby sprawdzić właściciela
            var track = await _context.Track
                .Include(t => t.Album) // Dołącz album powiązany z utworem
                .FirstOrDefaultAsync(t => t.Id == id);

            if (track == null)
            {
                return NotFound();
            }

            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim == null)
            {
                return Unauthorized(new { message = "Invalid token: UserId not found" });
            }
            int userId = int.Parse(userIdClaim.Value);

            // Sprawdzenie czy obecny album tego utworu należy do użytkownika
            var currentAlbum = await _context.Album
                .FirstOrDefaultAsync(a => a.Id == track.AlbumId && a.UserId == userId);

            if (currentAlbum == null)
            {
                return Forbid(); // 403 Forbidden
            }

            // Sprawdzenie czy nowy Album należy do użytkownika
            var newAlbum = await _context.Album
                .FirstOrDefaultAsync(a => a.Id == updatedTrack.AlbumId && a.UserId == userId);

            if (newAlbum == null)
            {
                return Forbid("You cannot assign this track to an album you do not own");
            }


            track.Title = updatedTrack.Title;
            track.Duration = updatedTrack.Duration;
            track.AlbumId = updatedTrack.AlbumId;
            track.GenreId = updatedTrack.GenreId;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TrackExists(id))
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

        // POST: api/Track
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Track>> PostTrack(TrackRequestDTO track)
        {
            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim == null)
            {
                return Unauthorized(new { message = "Invalid token: UserId not found" });
            }
            int userId = int.Parse(userIdClaim.Value);

            // Sprawdzenie czy album należy do użytkownika
            var album = await _context.Album
                .FirstOrDefaultAsync(a => a.Id == track.AlbumId && a.UserId == userId);


            if (album == null)
            {
                return Forbid("You cannot assign this track to an album you do not own");
            }

            var genreExists = await _context.Genre.AnyAsync(g => g.Id == track.GenreId);
            if (!genreExists)
            {
                return BadRequest(new { message = "Selected genre does not exist" });
            }

            var newTrack = new Track
            {
                Title = track.Title,
                Duration = track.Duration,
                AlbumId = track.AlbumId,
                GenreId = track.GenreId
            };

            _context.Track.Add(newTrack);
            await _context.SaveChangesAsync();


            return NoContent();
        }

        // DELETE: api/Track/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTrack(int id)
        {
            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim == null)
            {
                return Unauthorized(new { message = "Invalid token: UserId not found" });
            }

            int userId = int.Parse(userIdClaim.Value);

            var track = await _context.Track
                .Include(t => t.Album) // Pobranie albumu, do którego należy utwór
                .FirstOrDefaultAsync(t => t.Id == id);

            if (track == null)
            {
                return NotFound();
            }
            if (track.Album.UserId != userId)
            {
                return Forbid(); // 403 Forbidden
            }

            _context.Track.Remove(track);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TrackExists(int id)
        {
            return _context.Track.Any(e => e.Id == id);
        }
    }
}
