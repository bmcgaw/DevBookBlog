using DevBook.Data;
using DevBook.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Security.Claims;

namespace DevBook.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PostController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public PostController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
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

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Content,CreatedAt,TagList")] PostModel postModel, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                // Set the user to the post
                if (User.Identity.IsAuthenticated)
                {
                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                    if (!string.IsNullOrEmpty(userId))
                        postModel.UserId = userId;
                }

                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (file != null && file.Length > 0)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string postPath = Path.Combine(wwwRootPath, @"images\post");

                    using (var fileStream = new FileStream(Path.Combine(postPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    postModel.ImageUrl = @"\images\post\" + fileName;
                }

                // Set the created date and save
                postModel.CreatedAt = DateTime.Now;
                _context.Add(postModel);
                await _context.SaveChangesAsync();

                // Process post tags and save the tags and tag/post relationship
                string[] postTags = postModel.TagList.Split(",", StringSplitOptions.RemoveEmptyEntries);
                var existingTags = await _context.Tags.ToListAsync();

                foreach (var tagName in postTags)
                {
                    string trimTag = tagName.Trim();
                    var tag = existingTags.FirstOrDefault(t => t.Name.Equals(trimTag, StringComparison.OrdinalIgnoreCase));


                    if (tag == null)
                    {
                        tag = new TagModel { Name = char.ToUpper(trimTag[0]) + trimTag.Substring(1) };
                        _context.Tags.Add(tag);
                        await _context.SaveChangesAsync();
                    }

                    var postTag = new PostTagModel
                    {
                        PostId = postModel.Id,
                        TagId = tag.Id
                    };
                    _context.Add(postTag);
                }


                // Save all changes
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(postModel);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var postModel = await
                _context.Posts
                .Include(p => p.PostTags)
                .ThenInclude(pt => pt.Tag)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (postModel == null)
            {
                return NotFound();
            }

            postModel.TagList = string.Join(",", postModel.PostTags.Select(pt => pt.Tag.Name));

            return View(postModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Content,CreatedAt,TagList")] PostModel postModel, IFormFile? file)
        {
            if (id != postModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingPost = await _context.Posts
                        .Include(p => p.PostTags)
                        .ThenInclude(pt => pt.Tag)
                        .FirstOrDefaultAsync(p => p.Id == id);

                    if (existingPost == null)
                    {
                        return NotFound();
                    }

                    string wwwRootPath = _webHostEnvironment.WebRootPath;
                    if (file != null && file.Length > 0)
                    {
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        string postPath = Path.Combine(wwwRootPath, @"images\post");

                        if (!string.IsNullOrEmpty(existingPost.ImageUrl))
                        {
                            string oldImagePath = Path.Combine(wwwRootPath, existingPost.ImageUrl.TrimStart('\\'));

                            if (System.IO.File.Exists(oldImagePath))
                            {
                                System.IO.File.Delete(oldImagePath);
                            }
                        }

                        using (var fileStream = new FileStream(Path.Combine(postPath, fileName), FileMode.Create))
                        {
                            file.CopyTo(fileStream);
                        }
                        existingPost.ImageUrl = @"\images\post\" + fileName;
                    }

                    existingPost.Title = postModel.Title;
                    existingPost.Content = postModel.Content;

                    var newTags = postModel.TagList.Split(",");
                    var existingTags = existingPost.PostTags.Select(pt => pt.Tag.Name).ToList();

                    var tagsToRemove = existingPost.PostTags.Where(pt => !newTags.Contains(pt.Tag.Name)).ToList();
                    var tagsToAdd = newTags.Except(existingTags).ToList();

                    _context.PostTags.RemoveRange(tagsToRemove);
                    foreach (var tagName in tagsToAdd)
                    {
                        var tag = await _context.Tags.FirstOrDefaultAsync(t => t.Name == tagName.Trim());
                        if (tag == null)
                        {
                            tag = new TagModel { Name = tagName.Trim() };
                            _context.Tags.Add(tag);
                        }
                        existingPost.PostTags.Add(new PostTagModel { PostId = existingPost.Id, Tag = tag });
                    }

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
            return View(postModel);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var postModel = await
                _context.Posts
                .Include(p => p.PostTags)
                .ThenInclude(pt => pt.Tag)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (postModel == null)
            {
                return NotFound();
            }

            return View(postModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var postModel = await _context.Posts.FindAsync(id);

            string wwwRootPath = _webHostEnvironment.WebRootPath;
            if (!string.IsNullOrEmpty(postModel.ImageUrl))
            {
                string oldImagePath = Path.Combine(wwwRootPath, postModel.ImageUrl.TrimStart('\\'));

                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }
            }

            var postTagsToRemove = await _context.PostTags.Where(pt => pt.PostId == id).ToListAsync();
            var commentsToRemove = await _context.Comments.Where(c => c.PostId == id).ToListAsync();

            if (postTagsToRemove != null)
            {
                _context.PostTags.RemoveRange(postTagsToRemove);
            }

            if (commentsToRemove != null)
            {
                _context.Comments.RemoveRange(commentsToRemove);
            }

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
