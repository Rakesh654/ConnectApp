using System.Security.Cryptography;
using System.Text;
using System.Text.Unicode;
using API.Data;
using API.DTO;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly DataContext _dataContext;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;
        
      public AccountController(UserManager<AppUser> userManager, ITokenService tokenService, IMapper mapper)
      {
            _userManager = userManager;
            _mapper = mapper;
            _tokenService = tokenService;
      }

      [HttpPost]
      [Route("register")]
      public async Task<ActionResult<UserDto>> Register([FromBody] RegisterDto registeruser)
      {
            if(await UserExists(registeruser.UserName)) return BadRequest("Username is taken");

            var user = _mapper.Map<AppUser>(registeruser);
           
            user.UserName = registeruser.UserName.ToLower();

           var result = await _userManager.CreateAsync(user, registeruser.Password);

            if(!result.Succeeded) return BadRequest(result.Errors);

            var roleResult = await _userManager.AddToRoleAsync(user, "Member");

            if(!roleResult.Succeeded) return BadRequest(result.Errors);

            return new UserDto()
            {
              UserName = user.UserName,
              Token = await _tokenService.CreateToken(user),
              PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url,
              KnownAs = user.KnownAs,
              Gender = user.Gender
            };
      }

      [HttpPost]
      [Route("login")]
      public async Task<ActionResult<UserDto>> Login(LoginDto login)
      {
        var user = await _userManager.Users.Include(p => p.Photos).SingleOrDefaultAsync(x => x.UserName == login.UserName);
        if(user == null) return Unauthorized("invalid username");

        var result = await _userManager.CheckPasswordAsync(user, login.Password);

        if(!result) return Unauthorized("Invalid Password");

        return new UserDto()
            {
              UserName = user.UserName,
              Token =   await _tokenService.CreateToken(user),
              PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url,
              KnownAs = user.KnownAs,
              Gender = user.Gender
            };
      }

      private async Task<bool> UserExists(string username)
      {
        return await _userManager.Users.AnyAsync(x => x.UserName == username.ToLower());
      }
    }
}