using System.Security.Cryptography;
using System.Text;
using System.Text.Unicode;
using API.Data;
using API.DTO;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly DataContext _dataContext;
        private readonly ITokenService _tokenService;
      public AccountController(DataContext dataContext, ITokenService tokenService)
      {
            _tokenService = tokenService;
            _dataContext = dataContext;
      }

      [HttpPost]
      [Route("register")]
      public async Task<ActionResult<UserDto>> Register([FromBody] RegisterDto registeruser)
      {
            if(await UserExists(registeruser.UserName)) return BadRequest("Username is taken");
        
            using var hmac = new HMACSHA512();
            var user= new AppUser
            {
                UserName = registeruser.UserName.ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registeruser.Password)),
                PasswordSalt = hmac.Key
            };

            _dataContext.Users.Add(user);
            await _dataContext.SaveChangesAsync();
            return new UserDto()
            {
              UserName = user.UserName,
              Token = _tokenService.CreateToken(user),
              PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url
            };
      }

      [HttpPost]
      [Route("login")]
      public async Task<ActionResult<UserDto>> Login(LoginDto login)
      {
        var user = await _dataContext.Users.Include(p => p.Photos).SingleOrDefaultAsync(x => x.UserName == login.UserName);
        if(user == null) return Unauthorized("invalid username");

        using var hmac = new HMACSHA512(user.PasswordSalt);

        var computeHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(login.Password));

        for(int i =0; i < computeHash.Length; i++ )
        {
          if(computeHash[i] != user.PasswordHash[i]) return Unauthorized("invalid password");
        }

        return new UserDto()
            {
              UserName = user.UserName,
              Token = _tokenService.CreateToken(user),
              PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url
            };
      }

      private async Task<bool> UserExists(string username)
      {
        return await _dataContext.Users.AnyAsync(x => x.UserName == username.ToLower());
      }
    }
}