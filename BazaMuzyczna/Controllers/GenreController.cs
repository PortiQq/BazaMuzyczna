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
    public class GenreController : ControllerBase
    {
        private readonly AppDbContext _context;

        public GenreController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Genre
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GenreNameDTO>>> GetGenre()
        {
            var genres = await _context.Genre
                .Select(a => new GenreNameDTO
                {
                    Id = a.Id,
                    Name = a.Name
                }).ToListAsync();

                return genres;
        }

        // GET: api/Genre/5
        [HttpGet("{id}")]
        public async Task<ActionResult<GenreDTO>> GetGenre(int id)
        {
            var genre = await _context.Genre
                   .Where(a => a.Id == id)
                   .Include(a => a.Tracks)
                   .Select(a => new GenreDTO
                   {
                       Id = a.Id,
                       Name = a.Name,
                       Tracks = a.Tracks.Select(t => new TrackTitleDTO
                       {
                           Title = t.Title
                       }).ToList()
                   })
                   .FirstOrDefaultAsync();

            if (genre == null)
            {
                return NotFound();
            }

            return genre;
        }

        // PUT: api/Genre/5
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutGenre(int id, Genre updatedGenre)
        {
            var genre = await _context.Genre.FindAsync(id);
            if (genre == null)
            {
                return NotFound();
            }
            
            genre.Name = updatedGenre.Name;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GenreExists(id))
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

        // POST: api/Genre
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Genre>> PostGenre(Genre genre)
        {
            _context.Genre.Add(genre);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetGenre", genre);
        }

        // DELETE: api/Genre/5
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGenre(int id)
        {
            var genre = await _context.Genre.FindAsync(id);
            if (genre == null)
            {
                return NotFound();
            }

            _context.Genre.Remove(genre);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool GenreExists(int id)
        {
            return _context.Genre.Any(e => e.Id == id);
        }
    }
}
