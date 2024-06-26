﻿using System;
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
            var applicationDbContext = _context.Posts?
            .Include(p => p.User)?
            .Include(p => p.PostTags)
            .ThenInclude(pt => pt.Tag);
    
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
                .Include(p => p.PostTags)
                .ThenInclude(pt => pt.Tag)
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
        public async Task<IActionResult> Create([Bind("Title,Content,CreatedAt,TagList")] PostModel postModel)
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

                string[] postTags = postModel.TagList.Split(",",StringSplitOptions.RemoveEmptyEntries);
                var existingTags = await _context.Tags.ToListAsync();

                foreach (var tagName in postTags)
                {
                    string trimTag = tagName.Trim();
                    var tag = existingTags.FirstOrDefault(t => t.Name.Equals(trimTag, StringComparison.OrdinalIgnoreCase));

                    if (tag == null)
                    {
                        tag = new TagModel {  Name = trimTag };
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

            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", postModel.UserId);
            return View(postModel);
        }

        // POST: Post/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Content,TagList")] PostModel postModel)
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

        // POST: Post/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var postModel = await _context.Posts.FindAsync(id);
            var postTagsToRemove = await _context.PostTags.Where(pt => pt.PostId == id).ToListAsync();

            if (postTagsToRemove != null)
            {
                _context.PostTags.RemoveRange(postTagsToRemove);
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
