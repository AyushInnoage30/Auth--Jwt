using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Azure;
using Azure.Core;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using WebApplication5.Models;

namespace WebApplication5.Controllers
{
    [Controller]
    [Route("[controller]")]
    public class Auth : Controller
    {
        public readonly IConfiguration _config;
       

        public Auth(IConfiguration config ) { 
        _config = config;
       
        }

        [HttpPost]
        [Route("api/login")]
        public IActionResult Login([FromBody] Login login)
        {
            try
            {
              
                if (login.Username != "admin" || login.Password != "1234567890")
                {
                    return BadRequest("Invalid User try next time !");
                }


                var token = GenerateJwtToken(login.Username);
                Response.Cookies.Append("AccessToken", token);
                var router= Request.GetDisplayUrl();
                return Ok((RedirectToAction("redirect", "/dashboad")));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        private string GenerateJwtToken(string?  user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, user),
            //new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
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
}
/*Response.Cookies.Append("AuthToken", token, new CookieOptions
{
    HttpOnly = true, // Prevents JavaScript access
    Secure = true,   // Only send cookie over HTTPS
    SameSite = SameSiteMode.Strict, // Protects against CSRF
    Expires = DateTime.UtcNow.AddHours(1) // Token expiration
});*/
