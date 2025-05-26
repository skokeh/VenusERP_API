using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;
using VenusERP_API.Models;
using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using VenusERP_API.Data;

namespace VenusERP_API.Controllers
{

    public class LoginController : BaseApiController
    {
        private readonly IConfiguration _config;
        private readonly DataContext _context;
        public LoginController(IConfiguration config,DataContext context)
        {
            _config = config;
            _context = context;
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login([FromBody] UserLogin userLogin)
        {
            var user = Authenticate(userLogin);
            if (user != null)
            {
                var token = GenerateToken(user);
                var userResponse = new UserResponse
                {
                    Token = token,
                    UserName = "",
                    Expiry = ""
                };
                return Ok(userResponse);
            }

            return StatusCode(500,"Please Check UserName Or Password"); 
        }

        private string GenerateToken(UserModel user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {


                   new Claim(ClaimTypes.NameIdentifier,user.Code),
                   new Claim("UserName", user.EngName.ToString()),
                   new Claim("Id", user.ID.ToString()),
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
