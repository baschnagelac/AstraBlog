﻿using AstraBlog.Models;

namespace AstraBlog.Services.Interfaces
{
    public interface IBlogPostService
    {
        //CRUD Services for BlogPosts
        #region BlogPost CRUD Methods
        public Task AddBlogPostAsync(BlogPost blogPost);
        public Task UpdateBlogPostAsync(BlogPost blogPost);
        /// <summary>
        /// Geta single BlogPost by Id (integer)
        /// </summary>
        /// <param name="blogPostId"></param>
        /// <returns></returns>
        public Task<BlogPost> GetBlogPostAsync(int blogPostId);
        /// <summary>
        /// Geta single BlogPost by Slug (string)
        /// </summary>
        /// <param name="blogPostSlug"></param>
        /// <returns></returns>
        public Task<BlogPost> GetBlogPostAsync(string blogPostSlug);
        public Task DeleteBlogPostAsync(BlogPost blogPost);
        #endregion

        #region Get BlogPosts Methods
        public Task<IEnumerable<BlogPost>> GetBlogPostsAsync();
        public Task<IEnumerable<BlogPost>> GetPopularPostsAsync();
        public Task<IEnumerable<BlogPost>> GetPopularPostsAsync(int count);
        public Task<IEnumerable<BlogPost>> GetRecentPostsAsync();
        public Task<IEnumerable<BlogPost>> GetRecentPostsAsync(int count);
        #endregion

        #region Category CRUD Methods
        public Task AddCategoryAsync(Category category);
        public Task UpdateCategoryAsync(Category category);
        public Task<Category> GetCategoryAsync(int categoryId);
        public Task DeleteCategoryAsync(Category category);
        #endregion

        public Task<IEnumerable<Category>> GetCategoriesAsync();

        //additional methods

        //public Task AddTagsToBlogPostAsync(IEnumerable<int> tagsIds, int blogPostId);

        //public Task<bool> IsTagOnBlogPostAsync(int tagId, int blogPostId);

        //public Task RemoveAllBlogTagsAsync(int blogPostId);

        //public IEnumerable<BlogPost> Search(string searchString);

        public Task<bool> ValidateSlugAsync(string title, int blogId);
    }
}