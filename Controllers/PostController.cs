using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DevBook.Data;
using DevBook.Models;
using System.Security.Claims;

namespace DevBook.Controllers
{
    public class PostController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PostController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Post
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Posts.Include(p => p.User);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Post/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var postModel = await _context.Posts
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (postModel == null)
            {
                return NotFound();
            }

            return View(postModel);
        }

        // GET: Post/Create
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Post/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Content,CreatedAt,TagsList")] PostModel postModel)
        {

            if (ModelState.IsValid)
            {

                if (User.Identity.IsAuthenticated)
                {
                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                    if (!string.IsNullOrEmpty(userId))
                        postModel.UserId = userId;
                }

                postModel.CreatedAt = DateTime.Now;
           
                _context.Add(postModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", postModel.UserId);
            return View(postModel);
        }

        // GET: Post/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var postModel = await _context.Posts.FindAsync(id);
            if (postModel == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", postModel.UserId);
            return View(postModel);
        }

        // POST: Post/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Content,UserId,CreatedAt,UpdatedAt")] PostModel postModel)
        {
            if (id != postModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(postModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostModelExists(postModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", postModel.UserId);
            return View(postModel);
        }

        // GET: Post/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var postModel = await _context.Posts
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (postModel == null)
            {
                return NotFound();
            }

            return View(postModel);
        }

        // POST: Post/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var postModel = await _context.Posts.FindAsync(id);
            if (postModel != null)
            {
                _context.Posts.Remove(postModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PostModelExists(int id)
        {
            return _context.Posts.Any(e => e.Id == id);
        }
    }
}
