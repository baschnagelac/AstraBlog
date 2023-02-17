using AstraBlog.Data;
using AstraBlog.Helpers;
using AstraBlog.Models;
using AstraBlog.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AstraBlog.Services
{
    public class BlogPostService : IBlogPostService
    {
        private readonly ApplicationDbContext _context;

        public BlogPostService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task AddBlogPostAsync(BlogPost blogPost)
        {

            try
            {
                await _context.AddAsync(blogPost);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }




        public async Task AddCategoryAsync(Category category)
        {
            try
            {
                _context.Add(category);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task DeleteBlogPostAsync(BlogPost blogPost)
        {
            try
            {
                blogPost.IsDeleted = true;
                await UpdateBlogPostAsync(blogPost);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task DeleteCategoryAsync(Category category)
        {
            try
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<BlogPost> GetBlogPostAsync(int blogPostId)
        {
            try
            {
                BlogPost? blogPost = await _context.BlogPosts
                                                  .Include(b => b.Category)
                                                  .Include(b => b.Tags)
                                                  .Include(b => b.Comments)
                                                  .FirstOrDefaultAsync(b => b.Id == blogPostId);

                return blogPost!;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<BlogPost> GetBlogPostAsync(string blogPostSlug)
        {
            try
            {
                BlogPost? blogPost = await _context.BlogPosts
                                                  .Include(b => b.Category)
                                                  .Include(b => b.Tags)
                                                  .Include(b => b.Comments)
                                                  .FirstOrDefaultAsync(b => b.Slug == blogPostSlug);

                return blogPost!;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<IEnumerable<BlogPost>> GetBlogPostsAsync()
        {
            try
            {
                IEnumerable<BlogPost> blogPosts = await _context.BlogPosts
                                                                .Include(b => b.Category)
                                                                .Include(b => b.Tags)
                                                                .Include(b => b.Comments)
                                                                .ToListAsync();

                return blogPosts;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<IEnumerable<Category>> GetCategoriesAsync()
        {
            IEnumerable<Category> categories = await _context.Categories
                                                             .ToListAsync();

            return categories!;
        }

        public async Task<Category> GetCategoryAsync(int categoryId)
        {
            try
            {
                Category? category = await _context.Categories
                                                   .FirstOrDefaultAsync(b => b.Id == categoryId);

                return category!;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<IEnumerable<BlogPost>> GetPopularPostsAsync()
        {
            try
            {
                IEnumerable<BlogPost> blogPosts = await _context.BlogPosts
                                                                .Where(b => b.IsPublished == true && b.IsDeleted == false)
                                                                .Include(b => b.Category)
                                                                .Include(b => b.Tags)
                                                                .Include(b => b.Comments)
                                                                    .ThenInclude(c => c.Author)
                                                                .ToListAsync();

                return blogPosts.OrderByDescending(b => b.Comments.Count);
            }

            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// Gets the specified number of Popular posts based on count. 
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>

        public async Task<IEnumerable<BlogPost>> GetPopularPostsAsync(int count)
        {
            try
            {
                IEnumerable<BlogPost> blogPosts = await _context.BlogPosts
                                                                .Where(b => b.IsPublished == true && b.IsDeleted == false)
                                                                .Include(b => b.Category)
                                                                .Include(b => b.Tags)
                                                                .Include(b => b.Comments)
                                                                    .ThenInclude(c => c.Author)
                                                                .ToListAsync();

                return blogPosts.OrderByDescending(b => b.Comments.Count).Take(count);
            }

            catch (Exception)
            {

                throw;
            }
        }
        public async Task<IEnumerable<BlogPost>> GetRecentPostsAsync()
        {
            try
            {
                IEnumerable<BlogPost> blogPosts = await _context.BlogPosts
                                                                .Where(b => b.IsPublished == true && b.IsDeleted == false)
                                                                .Include(b => b.Category)
                                                                .Include(b => b.Tags)
                                                                .Include(b => b.Comments)
                                                                    .ThenInclude(c => c.Author)
                                                                .ToListAsync();

                return blogPosts.OrderByDescending(b => b.Created);
            }

            catch (Exception)
            {

                throw;
            }
        }

        public async Task<IEnumerable<BlogPost>> GetRecentPostsAsync(int count)
        {
            try
            {
                IEnumerable<BlogPost> blogPosts = await _context.BlogPosts
                                                                .Where(b => b.IsPublished == true && b.IsDeleted == false)
                                                                .Include(b => b.Category)
                                                                .Include(b => b.Tags)
                                                                .Include(b => b.Comments)
                                                                    .ThenInclude(c => c.Author)
                                                                .ToListAsync();

                return blogPosts.OrderByDescending(b => b.Created).Take(count);
            }

            catch (Exception)
            {

                throw;
            }
        }

        public async Task UpdateBlogPostAsync(BlogPost blogPost)
        {
            try
            {
                _context.Update(blogPost);

                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task UpdateCategoryAsync(Category category)
        {
            _context.Update(category);

            await _context.SaveChangesAsync();
        }

        public async Task<bool> ValidateSlugAsync(string title, int blogId)
        {
            try
            {
                string newSlug = StringHelper.BlogSlug(title);

                if(blogId == 0)
                {
                    return !await _context.BlogPosts.AnyAsync(b => b.Slug == newSlug);
                } 
                else
                {
                    BlogPost? blogPost = await _context.BlogPosts.AsNoTracking().FirstOrDefaultAsync(b => b.Id == blogId);

                    string? oldSlug = blogPost?.Slug;

                    if(!string.Equals(oldSlug, newSlug))
                    {
                        return !await _context.BlogPosts.AnyAsync(b => b.Id != blogPost!.Id && b.Slug == newSlug);
                    }
                }
                return true;
            }
            catch (Exception)
            {

                throw;
            }
        }


    }
}


