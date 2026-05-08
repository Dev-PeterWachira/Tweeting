
namespace Contracts.V1.Requests{
public class UserLoginRequest
{
    public string Email {get; set;} = string.Empty;

    public string password {get; set;} = string.Empty;
}
}