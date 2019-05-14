using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using ExampleJWT.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace ExampleJWT.Controllers
{
    [Route("api/[controller]")]
    public class TokenController : Controller
    {
        private IConfiguration _config;

        public TokenController(IConfiguration config)
        {
            _config = config;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> CreateToken([FromBody]AccessToken access)
        {
            IActionResult response = Unauthorized();
            var user = Authenticate(access);

            if (user != null)
            {
                var tokenString = BuildToken(access);
                response = Ok(new { token = tokenString });

                var datepd = DateTime.UtcNow.AddHours(7).AddYears(3); //Expired Token

                user.password = access.password;
                user.token = tokenString.ToString();
                user.dateexpd = datepd;
                
            }

            return response;
        }

        private string BuildToken(AccessToken access)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
              _config["Jwt:Issuer"],
              expires: DateTime.UtcNow.AddHours(7).AddYears(3),
              signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private AccessToken Authenticate(AccessToken access)
        {
            AccessToken data = new AccessToken();
            if (access.username == "camellabs" && access.password == "camellabs")
            {
                data = access;
            }
            else
            {
                data = null;
            }
            
            return data;
        }
    }
}