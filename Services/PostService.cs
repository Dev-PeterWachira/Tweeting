using Tweeting_book.Data;
using Contracts.V1;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;  
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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

        
    }
}