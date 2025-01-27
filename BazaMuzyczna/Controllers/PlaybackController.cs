using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BazaMuzyczna.Models;
using BazaMuzyczna.DataTransferObject;

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

        // GET: api/Playback
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Playback>>> GetPlayback()
        {
            return await _context.Playback.ToListAsync();
        }

        // GET: api/Playback/5
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

        // POST: api/Playback
        [HttpPost()]
        public async Task<ActionResult<Playback>> PostPlayback(Playback playback)
        {
            _context.Playback.Add(playback);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPlayback", playback);
        }

        //POST: api/Playback/add
        [HttpPost("add")]
        public async Task<IActionResult> AddPlayback([FromBody] PlaybackRequestDTO request)
        {
            // Wyświetlanie wartości w konsoli
            Console.WriteLine($"UserId: {request.UserId}, TrackId: {request.TrackId}");

            if (request.UserId <= 0 || request.TrackId <= 0)
            {
                return BadRequest(new { message = "Invalid userId or trackId." });
            }

            try
            {
                var userIdParam = new Npgsql.NpgsqlParameter("user_id", request.UserId);
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



        // DELETE: api/Playback/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlayback(int id)
        {
            var playback = await _context.Playback.FindAsync(id);
            if (playback == null)
            {
                return NotFound();
            }

            _context.Playback.Remove(playback);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PlaybackExists(int id)     
        {
            return _context.Playback.Any(e => e.Id == id);
        }
    }
}
