using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using VenusERP_API.Data;
using VenusERP_API.Models;

namespace VenusERP_API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    
    public class TokenController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly DataContext _context;

        public TokenController(IConfiguration config, DataContext context)
        {
            _config = config;
            _context = context;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult> Login()
        {
            if (!Request.HasFormContentType)
            {
                return StatusCode(StatusCodes.Status415UnsupportedMediaType);
            }
            var form = await HttpContext.Request.ReadFormAsync();
            var userName = form["username"].ToString();
            var password = form["password"].ToString();
            var userLogin = new UserLogin
            {
                UserName = userName,
                Password = password
            };
            var user = Authenticate(userLogin);

            if (user != null)
            {
                var token = GenerateToken(user);
                var userResponse = new UserResponse
                {
                    Token = token,
                    Access_token=token,
                    Token_type ="bearer",
                    UserName = "",
                    Expiry = ""
                };
                return Ok(userResponse);
            }

            return NotFound("user not found");
        }

        private string GenerateToken(UserModel user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                   new Claim(ClaimTypes.NameIdentifier,user.Code),
                   new Claim("UserName", user.EngName.ToString()),
                    new Claim("Id", user.ID.ToString())
            };
            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
                _config["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddDays(7),
                signingCredentials: credentials);


            return new JwtSecurityTokenHandler().WriteToken(token);

        }


        private UserModel Authenticate(UserLogin userLogin)
        {

            var user = _context.sys_Users.FirstOrDefault(x => x.Code == userLogin.UserName);

            if (user != null && BCrypt.Net.BCrypt.Verify(userLogin.Password.Trim(), user.Password.Trim()))
            {
                return user;

            }
            return null;
        }
    }
}
