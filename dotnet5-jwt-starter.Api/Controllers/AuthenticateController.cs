using dotnet5_jwt_starter.Api.Authentication;
using dotnet5_jwt_starter.Api.Contracts.V1.Resposes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace dotnet5_jwt_starter.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        #region Declaration
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;

        public AuthenticateController(UserManager<ApplicationUser> userManager, IConfiguration configuration, ApplicationDbContext context)
        {
            _userManager = userManager;
            _configuration = configuration;
            _context = context;
        }

        #endregion

        #region EndPoints

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await _userManager.FindByNameAsync(model.Username);
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                JwtSecurityToken token = await GenerateToken(user);

                var refreshToken = await SaveRefreshTokenAsync(token, user);

                return Ok(new AuthSuccessResponse
                {
                    Token = new JwtSecurityTokenHandler().WriteToken(token),
                    RefreshToken = refreshToken.Token
                });
            }
            return Unauthorized();
        }


        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            var userExists = await _userManager.FindByNameAsync(model.Username);
            if (userExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User already exists!" });

            ApplicationUser user = new ApplicationUser()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Username
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User creation failed! Please check user details and try again." });

            return Ok(new Response { Status = "Success", Message = "User created successfully!" });
        }


        [HttpPost]
        [Route("refresh-token")]
        public async Task<IActionResult> RefreshAsync(string refreshToken)
        {
            var refToken = _context.RefreshTokens.SingleOrDefault(r => r.Token == refreshToken);
            if (refreshToken == null)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, new Response { Status = "Error", Message = "This refresh token doesn't exist." });
            }

            if (refToken.Used)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, new Response { Status = "Error", Message = "This refresh token has been used." });
            }

            if (DateTime.UtcNow > refToken.ExpiryDate)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, new Response { Status = "Error", Message = "This refresh token is expired." });
            }

            if (refToken.invalidated)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, new Response { Status = "Error", Message = "This refresh token is invalidated." });
            }


            var user = await _userManager.FindByIdAsync(refToken.UserId);
            if (refreshToken == null)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, new Response { Status = "Error", Message = "User Not Found" });
            }


            // Change refresh token used status in database
            refToken.Used = true;
            _context.RefreshTokens.Update(refToken);
            await _context.SaveChangesAsync();

            // Generate new token and save it in database
            JwtSecurityToken newToken = await GenerateToken(user);
            var newRefreshToken = await SaveRefreshTokenAsync(newToken, user);

            return Ok(new AuthSuccessResponse
            {
                Token = new JwtSecurityTokenHandler().WriteToken(newToken),
                RefreshToken = newRefreshToken.Token
            });
        }

        #endregion

        #region Methods

        private async Task<JwtSecurityToken> GenerateToken(ApplicationUser user)
        {
            var userRoles = await _userManager.GetRolesAsync(user);

            var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                };

            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));


            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: GetTokenExpireDate(),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

            return token;
        }

        private async Task<RefreshToken> SaveRefreshTokenAsync(JwtSecurityToken token, ApplicationUser user)
        {
            var refreshToken = new RefreshToken
            {
                Token = Guid.NewGuid().ToString(),
                JwtId = token.Id,
                UserId = user.Id,
                CreatetionDate = DateTime.UtcNow,
                ExpiryDate = GetRefreshTokenExpireDate()
            };

            await _context.RefreshTokens.AddAsync(refreshToken);
            await _context.SaveChangesAsync();

            return refreshToken;
        }

        private DateTime GetTokenExpireDate()
        {
            //DateTime tokenLifeTime = DateTime.Parse(_configuration["JWT:TokenLifeTime"]);
            //DateTime expires = DateTime.UtcNow
            //    .AddHours(tokenLifeTime.Hour)
            //    .AddMinutes(tokenLifeTime.Minute)
            //    .AddSeconds(tokenLifeTime.Second);

            int tokenLifeTime = int.Parse(_configuration["JWT:TokenLifeTimeInMinutes"]);
            DateTime expires = DateTime.UtcNow
                .AddMinutes(tokenLifeTime);

            return expires;
        }

        private DateTime GetRefreshTokenExpireDate()
        {
            //DateTime tokenLifeTime = DateTime.Parse(_configuration["JWT:TokenLifeTime"]).AddDays(1);
            //DateTime expires = DateTime.UtcNow.AddHours(tokenLifeTime.Hour).AddMinutes(tokenLifeTime.Minute).AddSeconds(tokenLifeTime.Second);

            int refreshTokenLifeTime = int.Parse(_configuration["JWT:RefreshTokenLifeInMinutes"]);
            DateTime expires = DateTime.UtcNow
                .AddMinutes(refreshTokenLifeTime);

            return expires;
        }

        #endregion

    }
}
