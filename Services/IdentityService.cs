using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Tweeting_book.Data;
using Tweeting_book.Domain;
using Tweeting_book.Migrations;

namespace Tweeting_book.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly JwtSettings _jwtSettings;
        private readonly TokenValidationParameters _tokenValidationParameters;
        private readonly DataContext _context;

        public IdentityService(
            UserManager<IdentityUser> userManager,
            JwtSettings jwtSettings,
            TokenValidationParameters tokenValidationParameters,
            DataContext context)
        {
            _userManager = userManager;
            _jwtSettings = jwtSettings;
            _tokenValidationParameters = tokenValidationParameters;
            _context = context;
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
            }


            var newUserId = Guid.NewGuid();
            var newUser = new IdentityUser
            {
                Id = newUserId.ToString(),
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
            }
             await _userManager.AddClaimAsync(newUser, new Claim("tags.view", "true"));

            return await GenerateAuthenticationResultForUserAsync(newUser);
        }

        public async Task<AuthenticationResult> LoginAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return new AuthenticationResult
                {
                    Success = false,
                    Errors = new[] { "User does not exist" }
                };
            }

            var userHasValidPassword = await _userManager.CheckPasswordAsync(user, password);

            if (!userHasValidPassword)
            {
                return new AuthenticationResult
                {
                    Success = false,
                    Errors = new[] { "Invalid password" }
                };
            }

            return await GenerateAuthenticationResultForUserAsync(user);
        }

        public async Task<AuthenticationResult> RefreshTokenAsync(string token, string refreshToken)
        {
            var validatedToken = GetPrincipalFromToken(token);   //checks if the token is well formed.

            if (validatedToken == null)
            {
                return new AuthenticationResult
                {
                    Success = false,
                    Errors = new[] { "Invalid token" }
                };
            }

            var expiryDateUnix =
                long.Parse(validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Exp).Value);  // extracts exp claim from jwt.

            var expiryDateTimeUtc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)   // convert unix time to readable datetime.
                .AddSeconds(expiryDateUnix);
                // .Subtract(_jwtSettings.TokenLifeTime);

            if (expiryDateTimeUtc > DateTime.UtcNow) // check if token is still valid.
            {
                return new AuthenticationResult
                {
                    Success = false,
                    Errors = new[] { "This token hasn't expired yet" }
                };
            }

            var jti = validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Jti).Value;  // extract jti which links access token and refresh token.

            var storedRefreshToken = await _context.RefreshTokens  // find refresh token in the database.
                .SingleOrDefaultAsync(x => x.Token == refreshToken);

            if (storedRefreshToken == null)
            {
                return new AuthenticationResult
                {
                    Success = false,
                    Errors = new[] { "This refresh token doesn't exist" }
                };
            }

            if (DateTime.UtcNow > storedRefreshToken.ExpiryDate)
            {
                return new AuthenticationResult
                {
                    Success = false,
                    Errors = new[] { "This refresh token has expired" }
                };
            }

            if (storedRefreshToken.IsInvalidated)  // check if refreshtoken is valid.
            {
                return new AuthenticationResult
                {
                    Success = false,
                    Errors = new[] { "This refresh token has been invalidated" }
                };
            }

            if (storedRefreshToken.Used)  // check if the refresh token has been used.
            {
                return new AuthenticationResult
                {
                    Success = false,
                    Errors = new[] { "This refresh token has already been used." }
                };
            }

            if (storedRefreshToken.JwtId != jti)  // verify jwt.
            {
                return new AuthenticationResult
                {
                    Success = false,
                    Errors = new[] { "This refresh token doesn't match this JWT." }
                };
            }

            // Mark as used
            storedRefreshToken.Used = true;
            _context.RefreshTokens.Update(storedRefreshToken);
            await _context.SaveChangesAsync();

            // Get user and generate new tokens
            var user = await _userManager.FindByIdAsync(
                validatedToken.Claims.Single(x => x.Type == "id").Value);

            if (user == null)
            {
                return new AuthenticationResult
                { 
                    Success = false,
                    Errors = new[] { "User not found" }
                };
            }

            return await GenerateAuthenticationResultForUserAsync(user);
        }

        private async Task<AuthenticationResult> GenerateAuthenticationResultForUserAsync(IdentityUser user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();   // tokenHandler builds and reads jwt.
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);  // key = secret key for signing tokens(prevents tampering.)

            var claims = new List<Claim>  // Build Jwt claims.
            
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email!),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                    new Claim("id", user.Id)
                };

                var userClaims = await _userManager.GetClaimsAsync(user);  // allows roles and permissions.

                claims.AddRange(userClaims);


            var tokenDescriptor = new SecurityTokenDescriptor   //defines how our jwt will look like.
            {
                Subject = new ClaimsIdentity(claims),
             
                Expires = DateTime.UtcNow.Add(_jwtSettings.TokenLifeTime),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            var refreshToken = new RefreshToken  // create refresh token.
            {
                Token = Guid.NewGuid().ToString(),
                JwtId = tokenDescriptor.Subject.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Jti).Value, //links refresh token to this exact jwt
                UserId = user.Id,  //token owner.
                CreationDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddMonths(6),
                Used = false,
                IsInvalidated = false
            };

            await _context.RefreshTokens.AddAsync(refreshToken);  // save refresh token to db.
            await _context.SaveChangesAsync();

            return new AuthenticationResult  // return response.
            {
                Success = true,
                Token = tokenString,
                RefreshToken = refreshToken.Token            
            };
        }

        private ClaimsPrincipal? GetPrincipalFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();  // reads jwts, validates jwts and extracts claims.

            try
            {
                var principal = tokenHandler.ValidateToken(  // validate token.
                    token,
                    _tokenValidationParameters,
                    out var validatedToken);

                if (!IsJwtWithValidSecurityAlgorithm(validatedToken))
                {
                    return null;
                }

                return principal;
            }
            catch
            {
                return null;
            }
        }

        private bool IsJwtWithValidSecurityAlgorithm(SecurityToken validatedToken)
        {
            return (validatedToken is JwtSecurityToken jwtSecurityToken) &&  // ensure token is jwt.
                jwtSecurityToken.Header.Alg.Equals(    // ensures token was signed by hmac sha256.
                    SecurityAlgorithms.HmacSha256,
                    StringComparison.InvariantCultureIgnoreCase);
        }
    }
}