

using Microsoft.IdentityModel.Tokens;
namespace Tweeting_book.Domain{
public class AuthenticationResult
{
    public bool Success {get; set;}

    public string Token {get; set;} = string.Empty;

    public string RefreshToken {get; set;} = string.Empty;

    public IEnumerable<string> Errors {get; set;} = Array.Empty<string>();
}
}