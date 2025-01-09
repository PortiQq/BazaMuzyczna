using BazaMuzyczna.Models;
using Microsoft.AspNetCore.Mvc;

namespace BazaMuzyczna.Controllers
{
    [Route("/api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UserController(AppDbContext context)
        {
            _context = context;
        }
    }
}
