using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace WebApplication5.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : Controller
    {
        private readonly IConfiguration _config;

        public AuthController(IConfiguration config)
        {
            _config = config;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest login)
        {
            try
            {
                if (login.Username != "admin" || login.Password != "1234567890")
                {
                    return BadRequest("Invalid Username or Password!");
                }

                var token = GenerateJwtToken(login.Username);

                //👇👇 This line add token in cookies 
                Response.Cookies.Append("AccessToken", token, new CookieOptions
                {
                    HttpOnly = true, // Prevents access from JavaScript  (document.cookie)
                    Secure = true, // Ensures cookies are sent only over HTTPS
                    SameSite = SameSiteMode.Strict // Prevents CSRF attacks
                });

                return Ok(new { Message = "User Login Successful" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        //[Authorize] only validates the token from the Authorization header, not from cookies. Authorization: Bearer <token> 
        [Authorize]
        [HttpGet("logout")]
        public IActionResult Logout()
        {
            //ContainsKey is a method used to check if a specific key exists in a dictionary-like collection
            if (Request.Cookies.ContainsKey("AccessToken"))
            {
                Response.Cookies.Delete("AccessToken");
                return Ok("Logout Successfully");
            }
            return BadRequest("User is not logged in");
        }

        private string GenerateJwtToken(string username)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}


//verification Of Password
//BCrypt.Net.BCrypt.Verify(login.Password, "1234567890");