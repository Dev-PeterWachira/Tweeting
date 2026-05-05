

using Microsoft.IdentityModel.Tokens;

public class AuthenticatonResult
{
    public bool Success {get; set;}

    public string Token {get; set;} = string.Empty;

    public IEnumerable<string> ErrorMessage {get; set;} 
}