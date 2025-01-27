using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BazaMuzyczna.Models;

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
        [HttpPost]
        public async Task<ActionResult<Playback>> PostPlayback(Playback playback)
        {
            _context.Playback.Add(playback);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPlayback", new { id = playback.Id }, playback);
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
