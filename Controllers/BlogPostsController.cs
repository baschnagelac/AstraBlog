using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AstraBlog.Data;
using AstraBlog.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Azure.Identity;
using ContactPro_Astra.Data;
using System.Diagnostics;
using System.Collections;

namespace AstraBlog.Controllers
{
    [Authorize(Roles = "Admin")]
    public class BlogPostsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<BlogUser> _userManager;
        private IEnumerable tagList;

        public BlogPostsController(ApplicationDbContext context, UserManager<BlogUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: BlogPosts
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {


            var applicationDbContext = _context.BlogPosts.Include(b => b.Category).Include(t => t.Tags);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: BlogPosts/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.BlogPosts == null)
            {
                return NotFound();
            }

            var blogPost = await _context.BlogPosts
                .Include(b => b.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (blogPost == null)
            {
                return NotFound();
            }

            return View(blogPost);
        }

        // GET: BlogPosts/Create
        

        public async Task<IActionResult> Create()
        {

            IEnumerable<Category> categoriesList = await _context.Categories.ToListAsync();

            ViewData["CategoryList"] = new MultiSelectList(categoriesList, "Id", "Name");

            IEnumerable<Tag> tagList = await _context.Tags.ToListAsync();

            ViewData["TagList"] = new MultiSelectList(tagList, "Id", "Name");


            //tags ~ query and present list of tags, comments, and the category 
            // IEnumerable<Tag> tagsList = await _context.Tags
            //                                          .ToListAsync();


            // IEnumerable<Comment> commentsList = await _context.Comments
            //                                                   .ToListAsync();

            //Category? cat = await _context.Categories
            //                             .FirstOrDefaultAsync();


            // ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            // ViewData["TagId"] = new MultiSelectList(_context.Tags, "Id", "Name");
            // ViewData["CommentId"] = new SelectList(_context.Comments, "Id", "Name");
            return View();
        }

        // POST: BlogPosts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        
        public async Task<IActionResult> Create([Bind("Id,Title,Abstract,Content,Created,Updated,Slug,IsDeleted,IsPublished,CategoryId")] BlogPost blogPost, IEnumerable<int> selected)
        {

            
            
            if (ModelState.IsValid)
            {
               
                
                blogPost.Created = DataUtility.GetPostGresDate(DateTime.UtcNow);
;

                



                //TODO: INSERT IMAGE SERVICE BELOW

                //if (blogPost.ImageFile != null)
                //{
                //    blogPost.ImageData = await _imageService.ConvertFileToByteArrayAsync(contact.ImageFile);
                //    blogPost.ImageType = blogPost.ImageFile.ContentType;
                //}



                _context.Add(blogPost);
                await _context.BlogPosts.Include(c => c.Tags).ToListAsync();
                await _context.SaveChangesAsync();

                //TODO:  add tags to blogpost 



                //TODO: Add SERVICE CALL ~add x to x asynx~
                return RedirectToAction(nameof(Index));
            }
            //ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", blogPost.CategoryId);
            ViewData["TagList"] = new MultiSelectList(tagList, "Id", "Name");

            return View(blogPost);
        }

        // GET: BlogPosts/Edit/5
      
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.BlogPosts == null)
            {
                return NotFound();
            }

            var blogPost = await _context.BlogPosts
                                         .Include(c => c.Tags)
                                         .Include(c => c.Category)
                                         .FirstOrDefaultAsync(c => c.Id == id);
            if (blogPost == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", blogPost.CategoryId);

            ViewData["TagList"] = new MultiSelectList(tagList, "Id", "Name");



            return View(blogPost);
        }

        // POST: BlogPosts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
     
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Abstract,Content,Created,Updated,Slug,IsDeleted,IsPublished,ImageData,ImageType,CategoryId")] BlogPost blogPost, IEnumerable<int> selected)
        {
            if (id != blogPost.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {

                    blogPost.Created = DataUtility.GetPostGresDate(blogPost.Created);

                    blogPost.Updated = DataUtility.GetPostGresDate(DateTime.UtcNow);


                    _context.Update(blogPost);

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BlogPostExists(blogPost.Id))
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
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", blogPost.CategoryId);
            ViewData["TagList"] = new MultiSelectList(tagList, "Id", "Name");

            return View(blogPost);
        }

        // GET: BlogPosts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.BlogPosts == null)
            {
                return NotFound();
            }

            var blogPost = await _context.BlogPosts
                .Include(b => b.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (blogPost == null)
            {
                return NotFound();
            }

            return View(blogPost);
        }

        // POST: BlogPosts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
    
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.BlogPosts == null)
            {
                return Problem("Entity set 'ApplicationDbContext.BlogPosts'  is null.");
            }
            var blogPost = await _context.BlogPosts.FindAsync(id);
            if (blogPost != null)
            {
                _context.BlogPosts.Remove(blogPost);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BlogPostExists(int id)
        {
            return (_context.BlogPosts?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
