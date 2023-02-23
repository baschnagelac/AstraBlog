using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AstraBlog.Data;
using AstraBlog.Models;
using Microsoft.AspNetCore.Identity;
using AstraBlog.Services.Interfaces;

namespace AstraBlog.Controllers
{
    public class CommentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<BlogUser> _userManager;
        private readonly IBlogPostService _blogPostService;

        public CommentsController(ApplicationDbContext context, UserManager<BlogUser> userManager, IBlogPostService blogPostService)
        {
            _context = context;
            _userManager = userManager;
            _blogPostService = blogPostService;
        }

        // GET: Comments
        public async Task<IActionResult> Index()
        {
            string? userId = _userManager.GetUserId(User);

            var comments = await _blogPostService.GetCommentsAsync();
            return View(comments);
        }

        // GET: Comments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Comment comment = await _blogPostService.GetCommentByIdAsync(id.Value);
            
            if (comment == null)
            {
                return NotFound();
            }

            return View(comment);
        }

        // GET: Comments/Create
        public IActionResult Create()
        {
            string? userId = _userManager.GetUserId(User);
            

            return View();
        }

        // POST: Comments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Body,Created,Updated,UpdateReason,BlogPostId,AuthorId")] Comment comment, string? slug)
        {
            ModelState.Remove("AuthorId");
            
            if (ModelState.IsValid)
            {
                comment.AuthorId = _userManager.GetUserId(User);

               

                comment.Created = DateTime.UtcNow;

                await _blogPostService.AddNewCommentAsync(comment);
                
                
            }
            return RedirectToAction("Details", "BlogPosts", new { slug = slug });
        }

        // GET: Comments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Comment comment = await _blogPostService.GetCommentByIdAsync(id.Value);
            if (comment == null)
            {
                return NotFound();
            }

            string? userId = _userManager.GetUserId(User);

            return View(comment);
        }

        // POST: Comments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Body,Created,Updated,UpdateReason,BlogPostId,AuthorId")] Comment comment)
        {
            if (id != comment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    string? authorId = _userManager.GetUserId(User);
                    comment.Created = DateTime.SpecifyKind(comment.Created, DateTimeKind.Utc);

                    await _blogPostService.UpdateCommentAsync(comment);

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (! await CommentExists(comment.Id))
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

            return View(comment);
        }

        // GET: Comments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Comments == null)
            {
                return NotFound();
            }

            var comment = await _blogPostService.GetCommentByIdAsync(id.Value);
            if (comment == null)
            {
                return NotFound();
            }

            return View(comment);
        }

        // POST: Comments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Comments == null)
            {
                return NotFound();
            }
            var comment = await _blogPostService.GetCommentByIdAsync(id);
            
            if (comment != null)
            {
                await _blogPostService.DeleteCommentAsync(comment);
            }
            
            
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> CommentExists(int id)
        {
            return (await _blogPostService.GetTagsAsync()).Any(e => e.Id == id);
        }
    }
}
