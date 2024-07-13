using DevBook.Data;
using DevBook.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DevBook.Areas.Guest.Controllers
{
    [Area("Guest")]
    public class CommentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CommentController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Comments.Include(c => c.User);

            return View(await applicationDbContext.ToListAsync());
        }

        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserId,Content,PostId")] CommentModel commentModel)
        {
            if (ModelState.IsValid)
            {
                if (User.Identity.IsAuthenticated)
                {
                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                    if (!string.IsNullOrEmpty(userId))
                        commentModel.UserId = userId;
                }
                commentModel.CreatedAt = DateTime.Now;
                _context.Add(commentModel);
                await _context.SaveChangesAsync();
                TempData["success"] = "Comment added successfully";
                return RedirectToAction("Details", "Home", new { id = commentModel.PostId });
            }
            TempData["error"] = "Error occurred while adding comment";
            return View(commentModel);
        }


    }
}
