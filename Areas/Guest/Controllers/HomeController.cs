using DevBook.Data;
using DevBook.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace DevBook.Areas.Guest.Controllers
{
    [Area("Guest")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Posts?
           .Include(p => p.User)?
           .Include(p => p.PostTags)
           .ThenInclude(pt => pt.Tag)
           .Include(p => p.Comments);

            return View(await applicationDbContext.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var postModel = await _context.Posts
                .Include(p => p.Comments)
                .Include(p => p.PostTags)
                .ThenInclude(pt => pt.Tag)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (postModel == null)
            {
                return NotFound();
            }

            return View(postModel);
        }

        public async Task<IActionResult> SearchPosts(string searchWord)
        {

            var applicationDbContext = _context.Posts?
            .Include(p => p.User)?
            .Include(p => p.PostTags)
            .ThenInclude(pt => pt.Tag);

            if (searchWord == null || searchWord == "")
            {
                return View("Index", await applicationDbContext.ToListAsync());
            }

            var filteredPosts = await applicationDbContext
            .Where(p => p.Title.Contains(searchWord) || p.PostTags.Any(pt => pt.Tag.Name.Contains(searchWord)))
            .ToListAsync();

            return View("Index", filteredPosts);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
