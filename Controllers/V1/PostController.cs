using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using Contracts.V1; 
using  Tweeting_book.Data;
using Tweeting_book.Services;

namespace Tweeting_book.Controllers
{
    [ApiController]
    [Route("api/V1/[controller]")]
    public class PostController : ControllerBase
    {
        // private List<Post> _posts;
        private readonly IPostService _postService;

        public PostController(IPostService postService)
        {
            _postService = postService;
        }
       

        [HttpGet(ApiRoutes.Posts.GetAll)] 
        public async Task<IActionResult> Get()         
        {
            return Ok(await _postService.GetPosts());
        }
        
        [HttpGet(ApiRoutes.Posts.Get)]

        public async Task<IActionResult> Get([FromRoute] Guid postId)
        {
            var post = await _postService.GetPostByIdAsync(postId);
            
            if(post == null)
            return NotFound();

            return Ok(post);
        }

        [HttpPost(ApiRoutes.Posts.Create)]

        public async Task<IActionResult> Create([FromBody] CreatePostRequest postRequest)
        {
          var Post = new Post{Name = postRequest.Name};
        
          
          await _postService.CreatePostAsync(Post);

          var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.ToUriComponent()}";
          var locationUri = baseUrl + "/" + ApiRoutes.Posts.Get.Replace("{postId}", Post.Id.ToString());

          var response = new PostResponse { Id = Post.Id };
          return Created(locationUri, response);
        } 


           [HttpPut(ApiRoutes.Posts.Update)]
            public async Task<IActionResult> Update([FromRoute] Guid postId, [FromBody] UpdatedPostRequest request)
        {
            var post = new Post
            {
                Id = postId,
                Name = request.Name
            };
            
            if(post == null)
            return NotFound();

            var updated = await _postService.UpdatePostAsync(post);
            if (updated)
            return Ok(post);

            return NotFound();
        }

        [HttpDelete(ApiRoutes.Posts.Delete)]
        public async Task<IActionResult> Delete([FromRoute] Guid postId)
        {
            var deleted = await _postService.DeletePostAsync(postId);
            if(deleted )
            return NoContent();

            return NotFound();
        }

        }
    }
