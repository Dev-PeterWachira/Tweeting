using Contracts.V1;
using Tweeting_book.Data;
namespace Tweeting_book.Services
{

public interface IPostService
{
    Task <List<Post>> GetPosts();

    Task<Post?> GetPostByIdAsync (Guid postId);

    Task<bool> CreatePostAsync (Post post);

   Task <bool> UpdatePostAsync (Post postToUpdate);

    Task <bool> DeletePostAsync (Guid postId);

    Task <bool> UserOwnsPostAsync (Guid postId, string UserId);
        
            
        
}
}