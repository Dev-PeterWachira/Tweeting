using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Tweeting_book.Domain
{
    public class RefreshToken
    {
        [Key]
        public string Token {get; set;} = string.Empty;

        public string JwtId{get; set;} = string.Empty;

        public DateTime CreationDate{get; set;}

        public DateTime ExpiryDate{get; set;}

        public bool IsInvalidated{get; set;}

        public bool Used{get; set;}

        public string UserId{get; set;} = string.Empty;

        [ForeignKey(nameof(UserId))]
        public IdentityUser User {get; set;} 
    }
}