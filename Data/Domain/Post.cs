using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Tweeting_book.Data
{
    public class Post
    {
        public Guid Id {get; set;} 

        public string Name {get; set;} = string.Empty;

        public string UserId {get; set;} = string.Empty;

        public List<string> Tags {get; set;} = new();

        [ForeignKey(nameof(UserId))]
        public IdentityUser? User {get; set;}
    }
}

