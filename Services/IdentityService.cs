
using Tweeting_book.Domain;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace Tweeting_book.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly JwtSettings _jwtSettings;

        public IdentityService(UserManager<IdentityUser> userManager, JwtSettings jwtSettings)
        {
            _userManager = userManager;
            _jwtSettings = jwtSettings;
        }

        public Task<AuthenticationResult> LoginAsync(string email, string password)
        {
            throw new NotImplementedException();
        }

        public async Task<AuthenticationResult> RegisterAsync(string email, string password)
        {
            var existingUser = await _userManager.FindByEmailAsync(email);

            if (existingUser != null)
            {
                return new AuthenticationResult
                {
                    Success = false,
                    Errors = new[] { "User with that email already exists" }
                };

                // return GenerateAuthenticationResultForUser();


            }

            var newUser = new IdentityUser
            {
                Email = email,
                UserName = email
            };

            var createdUser = await _userManager.CreateAsync(newUser, password);

            if (!createdUser.Succeeded)
            {
                return new AuthenticationResult
                {
                    Success = false,
                    Errors = createdUser.Errors.Select(x => x.Description)
                };

                // return GeneratedAuthenticationForUser(newUser);
            }

            JwtSecurityTokenHandler tokenHandler;
            SecurityToken token;
            GenerateAuthenticationResultForUser(newUser, out tokenHandler, out token);

            return new AuthenticationResult
            {

                Success = true,
                Token = tokenHandler.WriteToken(token)

            };




        }

        private void GenerateAuthenticationResultForUser(IdentityUser newUser, out JwtSecurityTokenHandler tokenHandler, out SecurityToken token)
        {
            tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, newUser.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Email, newUser.Email),
                    new Claim("Id", newUser.Id)

                }),

                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)

            };
            token = tokenHandler.CreateToken(tokenDescriptor);
        }
    }
}