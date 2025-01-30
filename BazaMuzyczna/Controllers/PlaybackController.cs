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
    public class PlaybackController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PlaybackController(AppDbContext context)
        {
            _context = context;
        }

        //POST: api/Playback/add
        [Authorize]
        [HttpPost("add")]
        public async Task<IActionResult> AddPlayback([FromBody] PlaybackRequestDTO request)
        {
            try
            {
                var userIdClaim = User.FindFirst("UserId");
                if (userIdClaim == null)
                {
                    return Unauthorized(new { message = "Invalid token: UserId not found" });
                }

                if (request.TrackId <= 0)
                {
                    return BadRequest(new { message = "trackId." });
                }

                int userId = int.Parse(userIdClaim.Value);

                var userIdParam = new Npgsql.NpgsqlParameter("user_id", userId);
                var trackIdParam = new Npgsql.NpgsqlParameter("track_id", request.TrackId);

                await _context.Database.ExecuteSqlRawAsync(
                    "CALL upsert_playback(@user_id, @track_id)",
                    userIdParam,
                    trackIdParam
                );

                return Ok(new { message = "Playback added successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "An error occurred", details = ex.Message });
            }
        }

        // GET: api/Playback/favourites
        [Authorize]
        [HttpGet("favourites")]
        public async Task<ActionResult<IEnumerable<PlaybackDTO>>> GetFavouritePlaybacks()
        {
            try
            {
                var userIdClaim = User.FindFirst("UserId");
                if (userIdClaim == null)
                {
                    return Unauthorized(new { message = "Invalid token: UserId not found" });
                }
                int userId = int.Parse(userIdClaim.Value);

                var playbacks = await _context.Playback
                    .Where(p => p.UserId == userId && p.Quantity > 2)
                    .OrderByDescending(p => p.Quantity)
                    .Select(p => new PlaybackDTO
                    {
                        Id = p.Id,
                        TrackId = p.TrackId,
                        Quantity = p.Quantity,
                        Track = new TrackTitleDTO
                        {
                            Title = p.Track.Title
                        }
                    })
                    .ToListAsync();

                return Ok(playbacks);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "An error occurred", details = ex.Message });
            }
        }


        // GET: api/Playback
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Playback>>> GetPlayback()
        {
            return await _context.Playback.ToListAsync();
        }

        // GET: api/Playback/5
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<Playback>> GetPlayback(int id)
        {
            var playback = await _context.Playback.FindAsync(id);

            if (playback == null)
            {
                return NotFound();
            }

            return playback;
        }

        private bool PlaybackExists(int id)     
        {
            return _context.Playback.Any(e => e.Id == id);
        }
    }
}
