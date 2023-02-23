using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AstraBlog.Data;
using AstraBlog.Models;
using AstraBlog.Services.Interfaces;

namespace AstraBlog.Controllers
{
    public class TagsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IBlogPostService _blogPostService;

        public TagsController(ApplicationDbContext context, IBlogPostService blogPostService)
        {
            _context = context;
            _blogPostService = blogPostService;
        }

        // GET: Tags
        public async Task<IActionResult> Index()
        {
            IEnumerable<Tag> model = await _blogPostService.GetTagsAsync();
            return View(model);
        }

        // GET: Tags/Details/5 details for one tag(id 5)
        public async Task<IActionResult> Details(int? id, int? pageNum)
        {
            if (id == null)
            {
                return NotFound();
            }

            Tag tag = await _blogPostService.GetTagByIdAsync(id.Value);
                 //tag, name, and posts related to that tag


            if (tag == null)
            {
                return NotFound();
            }

            int page = pageNum ?? 1;
            ViewData["Page"] = page;


            return View(tag);
        }

        // GET: Tags/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Tags/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] Tag tag)
        {
            if (ModelState.IsValid)
            {
                
                await _blogPostService.AddNewTagAsync(tag);   


                return RedirectToAction(nameof(Index));
            }
            return View(tag);
        }

        // GET: Tags/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null )
            {
                return NotFound();
            }

            Tag tag = await _blogPostService.GetTagByIdAsync(id.Value);

            if (tag == null)
            {
                return NotFound();
            }
            return View(tag);
        }

        // POST: Tags/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Tag tag)
        {
            if (id != tag.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _blogPostService.UpdateTagAsync(tag);

                 
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (! await TagExists(tag.Id))
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
            return View(tag);
        }

        // GET: Tags/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Tags == null)
            {
                return NotFound();
            }
          
            var tag = await _blogPostService.GetTagByIdAsync(id.Value);
            
            if (tag == null)
            {
                return NotFound();
            }

            return View(tag);
        }

        // POST: Tags/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var tag = await _blogPostService.GetTagByIdAsync(id);
           
            if (tag != null)
            {
                await _blogPostService.DeleteTagAsync(tag);
            }

           

            
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> TagExists(int id)
        {
          return (await _blogPostService.GetTagsAsync()).Any(t => t.Id == id);
        }
    }
}
