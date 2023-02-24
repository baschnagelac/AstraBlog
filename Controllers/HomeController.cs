using AstraBlog.Data;
using AstraBlog.Models;
using AstraBlog.Models.ViewModels;
using AstraBlog.Services.Interfaces;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using X.PagedList;

namespace AstraBlog.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IBlogPostService _blogPostService;
        private readonly IEmailSender _emailService;
        private readonly UserManager<BlogUser> _userManager;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, IBlogPostService blogPostService, IEmailSender emailSender, UserManager<BlogUser> userManager)
        {
            _logger = logger;
            _context = context;
            _blogPostService = blogPostService;
            _emailService = emailSender;    
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(int? pageNum, string? swalMessage = null)
        {
            ViewData["SwalMessage"] = swalMessage;

            int pageSize = 3;
            int page = pageNum ?? 1;

            
            
            IPagedList<BlogPost> model = (await _blogPostService.GetRecentPostsAsync()).ToPagedList(page,pageSize);

            return View(model);
        }

        public IActionResult SearchIndex(string? searchString, int? pageNum)
        {
            int pageSize = 5;
            int page = pageNum ?? 1;

            IPagedList<BlogPost> model = (_blogPostService.SearchBlogPosts(searchString)).ToPagedList(page, pageSize);

            return View(nameof(Index), model);
        }

        // EmailContact

        //GET:

        public async Task<IActionResult> EmailContact()
        {
            
            string? blogUserId = _userManager.GetUserId(User);

            BlogUser? blogUser = await _context.Users
                                             .FirstOrDefaultAsync(c => c.Id == blogUserId);

            if (blogUser == null)
            {
                return NotFound();
            }

            return View(blogUser);
        }

        //POST

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EmailContact(BlogUser blogUser, string? message)
        {

            ModelState.Remove("FirstName");
            ModelState.Remove("LastName");
            
            if (ModelState.IsValid)
            {
                string? swalMessage = string.Empty;

                try
                {
                    await _emailService.SendEmailAsync(blogUser.Email!,
                                                       "Contact me from my blog",
                                                       message!);

                    swalMessage = "Success: Message Sent!";


                    return RedirectToAction(nameof(Index), new { swalMessage });
                }
                catch (Exception)
                {
                    swalMessage = "Error: Message Send Failed!";
                    return RedirectToAction(nameof(Index), new { swalMessage });
                    throw;
                }
            }

            return View(blogUser);
        }


        public async Task<IActionResult> MostPopular(int? pageNum)
        {
            int pageSize = 3;
            int page = pageNum ?? 1;



            IPagedList<BlogPost> model = (await _blogPostService.GetPopularPostsAsync()).ToPagedList(page, pageSize);

            return View(model);
        }

        public async Task<IActionResult> MostRecent(int? pageNum)
        {
            int pageSize = 3;
            int page = pageNum ?? 1;



            IPagedList<BlogPost> model = (await _blogPostService.GetRecentPostsAsync()).ToPagedList(page, pageSize);

            return View(model);
        }




        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}