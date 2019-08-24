using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ChatServer.Engine.Database;
using ChatServer.Engine.Network;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace ServerWeb.Controllers
{
    public class APIController : Controller
    {
        private readonly APIHandler _apiHandler;
        private readonly UserManager<IDUser> _usermanager;
        private readonly SignInManager<IDUser> _signinManager;

        public APIController(APIHandler aPIHandler,
            UserManager<IDUser> userManager,
            SignInManager<IDUser> signInManager)
        {
            _apiHandler = aPIHandler;
            _usermanager = userManager;
            _signinManager = signInManager;
        }
        public IActionResult Index()
        {
            return Ok("The Api is working");
        }

        [Authorize]
        public IActionResult GetUsers()
        {
            return Ok(_apiHandler.GetUsers());
        }

        public async Task<IActionResult> Login(string username, string password)
        {
            var res = await _signinManager
                .PasswordSignInAsync(username.Trim(), password.Trim(), true, true);
            if(res == Microsoft.AspNetCore.Identity.SignInResult.Success)
            {
                var user = await _usermanager.FindByNameAsync(username);

                return Ok(GetToken(user));
            }
        }

        private String GetToken(IdentityUser user)
        {
            var utcNow = DateTime.UtcNow;

            var claims = new Claim[]
            {
                        new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                        new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Iat, utcNow.ToString())
            };

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.configuration.GetValue<String>("Tokens:Key")));
            var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
            var jwt = new JwtSecurityToken(
                signingCredentials: signingCredentials,
                claims: claims,
                notBefore: utcNow,
                expires: utcNow.AddSeconds(120)
                );

            return new JwtSecurityTokenHandler().WriteToken(jwt);

        }
    }
}