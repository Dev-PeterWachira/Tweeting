using Tweeting_book.Data;
using Contracts.V1;
using Microsoft.EntityFrameworkCore;  
using System;
using Tweeting_book.Migrations;

namespace Tweeting_book.Services
{
    public class PostService : IPostService
    {
        private readonly DataContext _dataContext;

       
        public PostService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

      
        public async Task<List<Post>> GetPosts()
        {
            return await _dataContext.posts.ToListAsync(); 
        }

      
        public async Task<Post?> GetPostByIdAsync(Guid postId)
        {
            return await _dataContext.posts.FirstOrDefaultAsync(x => x.Id == postId);
        }

        public async Task<bool> UpdatePostAsync(Post postToUpdate)
        {
            _dataContext.posts.Update(postToUpdate);
            var updated = await _dataContext.SaveChangesAsync();  
            return updated > 0; 
        }

        public async Task<bool> CreatePostAsync (Post post)
        {
            await _dataContext.posts.AddAsync(post);
            var created = await _dataContext.SaveChangesAsync();
            return created > 0;
        }

     
        public async Task<bool> DeletePostAsync(Guid postId)
        {
            var post = await GetPostByIdAsync(postId);
            
            if (post == null) 
                return false;

            _dataContext.posts.Remove(post);
            var deleted = await _dataContext.SaveChangesAsync();
            return deleted > 0;  
        }


        public async Task<bool>UserOwnsPostAsync(Guid postId, string UserId)
        {
            var post = await _dataContext.posts.AsNoTracking().SingleOrDefaultAsync(x => x.Id == postId);
            if(post == null)
            {
                return false;
            }

            if(post.UserId != UserId)
            {
                return false;
            }

            return true;
        }

        public async Task<List<string>> GetAllTagsAsync()
        {
            var posts = await _dataContext.posts.ToListAsync();

            if (posts.Any())
            {
                return posts.SelectMany(p => p.Tags).Distinct().ToList();
            }

            return new List<string>();     
               }
        
        public async Task<string?> GetTagByNameAsync(string tagName)
        {
            var posts = await _dataContext.posts.ToListAsync();
            var tags = posts.SelectMany(p => p.Tags).Distinct().ToList();
            return tags.FirstOrDefault(t => t.Equals(tagName, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<bool> CreateTagsAsync(string tagName)
        {
            var posts = await _dataContext.posts.ToListAsync();
            var tags = posts.SelectMany(p => p.Tags).Distinct().ToList();

            if(tags.Any(t => t.Equals(tagName, StringComparison.OrdinalIgnoreCase)))
            {
                return false;
            }

            return true;
        }


        public async Task<bool> UpdateTagAsync(string tagName, string newTagName)
        {
            var posts = await _dataContext.posts
            .Where(p => p.Tags.Any(t => t.Equals(tagName,StringComparison.OrdinalIgnoreCase))).ToListAsync();

            if (!posts.Any())
            {
                return false;
            }
               foreach (var post in posts)
    {
        var tagIndex = post.Tags.FindIndex(t => t.Equals(tagName, StringComparison.OrdinalIgnoreCase));
        if (tagIndex != -1)
        {
            post.Tags[tagIndex] = newTagName;
        }
    }

               await _dataContext.SaveChangesAsync();
                 return true;
}

         public async Task<bool> DeleteTagAsync(string tagName)
{
    var posts = await _dataContext.posts
        .Where(p => p.Tags.Any(t => t.Equals(tagName, StringComparison.OrdinalIgnoreCase)))
        .ToListAsync();

    if (!posts.Any())
    {
        return false; // Tag not found
    }

    foreach (var post in posts)
    {
        post.Tags.RemoveAll(t => t.Equals(tagName, StringComparison.OrdinalIgnoreCase));
    }

            await _dataContext.SaveChangesAsync();
                return true;
}


        }
    }
