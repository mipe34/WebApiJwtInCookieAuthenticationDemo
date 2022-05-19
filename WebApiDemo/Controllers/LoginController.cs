using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using WebApiDemo.Code;

namespace WebApiDemo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LoginController : ControllerBase
    {

        private JwtSettings jwtSettings;
        private TokenValidationParameters tokenValidationParameters;

        public LoginController(JwtSettings jwtSettings, TokenValidationParameters tokenValidationParameters)
        {
            this.jwtSettings = jwtSettings;
            this.tokenValidationParameters = tokenValidationParameters;
        }

        private static IEnumerable<Claim> GetClaims()
        {
            var id = Guid.NewGuid().ToString();
            var role = new Claim(ClaimTypes.Role, "admin");
            IEnumerable<Claim> claims = new Claim[] {
                    new Claim("Id", id),
                    new Claim(ClaimTypes.Name,"mpetrik"),
                    new Claim(ClaimTypes.Email, "mpetrik@scio.cz"),
                    new Claim(ClaimTypes.NameIdentifier, id),
                    new Claim(ClaimTypes.Expiration, DateTime.UtcNow.AddDays(1).ToString("MMM ddd dd yyyy HH:mm:ss tt")),
                    role
            };
            return claims;
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            var token = JwtExtensions.GenTokenkey(jwtSettings, GetClaims());

          //  Response.Cookies.Append("X-Access-Token", token, new CookieOptions() { HttpOnly = true, SameSite = SameSiteMode.Strict });
            //    Response.Cookies.Append("X-Username", user.UserName, new CookieOptions() { HttpOnly = true, SameSite = SameSiteMode.Strict });
            Response.Cookies.Append("X-Refresh-Token", token, new CookieOptions() { HttpOnly = true, SameSite = SameSiteMode.Strict });

            return Ok(token);
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult Refresh()
        {
            if (!Request.Cookies.TryGetValue("X-Refresh-Token", out var refreshToken))
            {
                return BadRequest();
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken validatedToken;
            try
            {
                tokenHandler.ValidateToken(refreshToken, tokenValidationParameters, out validatedToken);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            var jwtToken = (JwtSecurityToken)validatedToken;

            var accessToken = JwtExtensions.GenTokenkey(jwtSettings, jwtToken.Claims);

            Response.Cookies.Append("X-Access-Token", accessToken, new CookieOptions() { HttpOnly = true, SameSite = SameSiteMode.Strict });

            return Ok();
        }
    }
}
