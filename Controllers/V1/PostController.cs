using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using Contracts.V1; 
using  Tweeting_book.Data;

namespace Tweeting_book.Controllers
{
    [ApiController]
    [Route("api/V1/[controller]")]
    public class PostController : ControllerBase
    {
        private List<Post> _posts;

        public PostController()
        {
            _posts = new List<Post>(); 
            for (var i = 0; i < 5; i++)
            {
                _posts.Add(new Post { Id = Guid.NewGuid().ToString() });  
            }
        }

        [HttpGet(ApiRoutes.Posts.GetAll)] 
        public IActionResult Get()         
        {
            return Ok(_posts);
        }
    }
}