using Contracts.V1;
using Tweeting_book.Data;
namespace Tweeting_book.Services
{

public interface IPostService
{
    List <Post> GetPosts();

    Post GetPostById (Guid PostId);

    bool UpdatePost (Post PostToUpdate);

    bool DeletePost (Guid PostId);
        
            
        
}
}