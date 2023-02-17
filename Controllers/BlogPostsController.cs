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

using System.Diagnostics;
using System.Collections;
using AstraBlog.Services.Interfaces;
using AstraBlog.Services;
using AstraBlog.Helpers;

namespace AstraBlog.Controllers
{
    [Authorize(Roles = "Admin")]
    public class BlogPostsController : Controller
    {
        
        private readonly IImageService _imageService;
        private readonly IBlogPostService _blogPostService;


        public BlogPostsController( IImageService imageService, IBlogPostService blogPostService)
        {
            
            _imageService = imageService;
            _blogPostService = blogPostService;
        }

        public async Task<IActionResult> AdminPage()
        {


            var blogPost = await _blogPostService.GetBlogPostsAsync();
            return View(blogPost);
        }
        // GET: BlogPosts/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(string slug)
        {
            if (string.IsNullOrEmpty(slug))
            {
                return NotFound();
            }

            var blogPost = await _blogPostService.GetBlogPostAsync(slug);
            if (blogPost == null)
            {
                return NotFound();
            }

            return View(blogPost); 
        }

        // GET: BlogPosts/Create


        public async Task<IActionResult> Create()
        {

            

            ViewData["CategoryList"] = new SelectList(await _blogPostService.GetCategoriesAsync(), "Id", "Name");

            

            //to do: tags



            return View(new BlogPost());
        }

        // POST: BlogPosts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Create([Bind("Id,Title,Abstract,Content,Created,Updated,Slug,IsDeleted,IsPublished,ImageFile,CategoryId")] BlogPost blogPost, IEnumerable<int> selected)
        {



            if (ModelState.IsValid)
            {


                //slug blogpost
                if (!await _blogPostService.ValidateSlugAsync(blogPost.Title!, blogPost.Id))
                {
                    ModelState.AddModelError("Title", "A similar Title or Slug is already in use.");

                    ViewData["CategoryList"] = new SelectList(await _blogPostService.GetCategoriesAsync(), "Id", "Name");
                    return View(blogPost);
                }
                blogPost.Slug = StringHelper.BlogSlug(blogPost.Title!);


                //format date(s)
                blogPost.Created = DataUtility.GetPostGresDate(DateTime.UtcNow);



                // INSERT IMAGE SERVICE 

                if (blogPost.ImageFile != null)
                {
                    blogPost.ImageData = await _imageService.ConvertFileToByteArrayAsync(blogPost.ImageFile);
                    blogPost.ImageType = blogPost.ImageFile.ContentType;
                }

                //Call SERVICE to save new blog post 
                await _blogPostService.AddBlogPostAsync(blogPost);



                //TODO:  add tags to blogpost 




                return RedirectToAction(nameof(AdminPage));
            }
            


            return View(blogPost);
        }

        // GET: BlogPosts/Edit/5

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blogPost = await _blogPostService.GetBlogPostAsync(id.Value);
            if (blogPost == null)
            {
                return NotFound();
            }
            ViewData["CategoryList"] = new SelectList(await _blogPostService.GetCategoriesAsync(), "Id", "Name");





            return View(blogPost);
        }

        // POST: BlogPosts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Abstract,Content,Created,Updated,Slug,IsDeleted,IsPublished,ImageData,ImageType,ImageFile,CategoryId")] BlogPost blogPost, IEnumerable<int> selected)
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


                    //image service
                    if (blogPost.ImageFile != null)
                    {
                        blogPost.ImageData = await _imageService.ConvertFileToByteArrayAsync(blogPost.ImageFile);
                        blogPost.ImageType = blogPost.ImageFile.ContentType;
                    }

                    //slug blogpost
                    if (!await _blogPostService.ValidateSlugAsync(blogPost.Title!, blogPost.Id))
                    {
                        ModelState.AddModelError("Title", "A similar Title or Slug is already in use.");

                        ViewData["CategoryList"] = new SelectList(await _blogPostService.GetCategoriesAsync(), "Id", "Name");
                        return View(blogPost);
                    }
                    blogPost.Slug = StringHelper.BlogSlug(blogPost.Title!);


                    //call service to  update blog posts
                    await _blogPostService.UpdateBlogPostAsync(blogPost);

                    //todo: edit tags

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (! await BlogPostExists(blogPost.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(AdminPage));
            }
            ViewData["CategoryId"] = new SelectList(await _blogPostService.GetCategoriesAsync(), "Id", "Name");

            return View(blogPost);
        }

        // GET: BlogPosts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null )
            {
                return NotFound();
            }

            var blogPost = await _blogPostService.GetBlogPostAsync(id.Value);
            if (blogPost == null)
            {
                return NotFound();
            }

            return View(blogPost);
        }

        // POST: BlogPosts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blogPost = await _blogPostService.GetBlogPostAsync(id.Value);
            if (blogPost != null)
            {
                await _blogPostService.DeleteBlogPostAsync(blogPost);
            }


            return RedirectToAction(nameof(AdminPage));
        }

        private async Task<bool> BlogPostExists(int id)
        {
            return (await _blogPostService.GetBlogPostsAsync()).Any(b => b.Id == id);
        }
    }
}
