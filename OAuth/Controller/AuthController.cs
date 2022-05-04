using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OAuth.Models;
using OAuth.Models.Response;

namespace OAuth.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;

        public AuthController(UserManager<IdentityUser> userManager)
        {
            this._userManager = userManager;
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterUser user)
        {
            if (ModelState.IsValid)
            {
                var userExists = await _userManager.FindByEmailAsync(user.Email);

                //CHECK IF USER EXISTS
                if (userExists != null)
                {
                    return BadRequest(
                        new UserAuthorized
                        {
                            Error = new List<string>() { "Invalid model" },
                            IsSuccess = false
                        });
                }

                //CREATE USER -- IDENTITY
                var createUser = new RegisterUser()
                {
                    Email = user.Email,
                    UserName = user.Email,
                    Fname = user.Fname,
                    Lname = user.Lname
                } as IdentityUser;

                //REGISTER USER
                var newUser = await _userManager.CreateAsync(createUser, user.Password);

                if (newUser.Succeeded)
                {
                    return Ok(new UserAuthorized
                    {
                        IsSuccess = true,
                        Token = GenerateToken(createUser)
                    });
                }
                else
                {
                    return BadRequest(new UserAuthorized
                    {
                        IsSuccess = false,
                        Error = newUser.Errors.Select(x => x.Description).ToList()
                    });
                }
            }
            

            return BadRequest(
                new UserAuthorized
                {
                    IsSuccess = false,
                    Error = new List<string>() { "Invalid model" }
                }
                );
        }

        [HttpPost]
        [Route("LOGIN")]
        public async Task<IActionResult> Login(LoginUser user)
        {
            //CHECK EMAIL IS VALID
            var userExists = await _userManager.FindByEmailAsync(user.Email);

            //IF EXISTS GET CHECK IF PROVIDED INFO IS CORRECT
            if(userExists == null)
            {
                return BadRequest(new UserAuthorized
                {
                    Error = new List<string>() { "Invalid credentials" },
                    IsSuccess = false
                });
            }
           
            ////CREATE IDENTITY USER
            //var identUser = new IdentityUser
            //  {
            //    Email = user.Email
            //  };

            //CHECK PASSWORD IS CORRECT
            var passwordCorrect = await _userManager.CheckPasswordAsync(userExists, user.Password);

            //PASS TOKEN IF PASSWORD IS CORRECT
            if (passwordCorrect)
            {
                return Ok(new UserAuthorized
                {
                    IsSuccess = true,
                    Token = GenerateToken(userExists)
                });
            }

            return BadRequest(new UserAuthorized
            {
                Error = new List<string>() { "Invalid model provided" },
                IsSuccess = false
            });

        }

        //METHOD TO GENERATE TOKEN
        public string GenerateToken(IdentityUser user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var key = new SymmetricSecurityKey(Encoding.UTF32.GetBytes("Thisisthesecretkeysformyjwtsolution"));

            var tokenClaims = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("id", user.Id),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                }),
                Expires = DateTime.Now.AddHours(5),
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            };

            var tokenWrite = tokenHandler.CreateToken(tokenClaims);
            var tokenString = tokenHandler.WriteToken(tokenWrite);

            return tokenString;
        }
    }
}