
using Tweeting_book.Domain;
using System.Threading.Tasks;

namespace Tweeting_book.Services
{
    public interface IIdentityService
    {
        Task<AuthenticationResult> RegisterAsync(string email, string password);

        Task<AuthenticationResult>LoginAsync(string email, string password);

        Task<AuthenticationResult>RefreshTokenAsync(string token, string refreshToken);
    }
}