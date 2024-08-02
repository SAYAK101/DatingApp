using System.Security.Cryptography;
using System.Text;
using API.Contracts;
using API.DTOs;
using API.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class AccountController(IUserRepository userRepository, ITokenService tokenService, IMapper mapper) : BaseApiController
{
    [HttpPost("register")]
    public async Task<ActionResult<UserResponseDTO>> Register(RegisterDTO userDto)
    {
        if (userDto.Username == null || userDto.Username == "" || userDto.Username == " ")
        {
            return BadRequest("Username is Empty.");
        }

        var userExists = await UserExists(userDto.Username);
        if (userExists)
        {
            return BadRequest("Username is taken.");
        }
    
        using var hmac = new HMACSHA512();

        var user = mapper.Map<AppUser>(userDto);
        user.PasswordHash =  hmac.ComputeHash(Encoding.UTF8.GetBytes(userDto.Password));
        user.PasswordSalt = hmac.Key;
        

        userRepository.Add(user);
        await userRepository.SaveAllAsync();

        return Ok(new UserResponseDTO
        {
            Username = user.UserName,
            Token = tokenService.CreateToken(user),
            KnownAs = user.KnownAs
        });
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserResponseDTO>> Login(LoginDTO login)
    {
        var user = await userRepository.GetAppUserByUsernameAsync(login.Username);
        if (user == null)
        {
            return Unauthorized("User Name not exists.");
        }

        using var hmac = new HMACSHA512(user.PasswordSalt);

        var computerHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(login.Password));

        for (int i = 0; i < computerHash.Length; i++)
        {
            if (computerHash[i] != user.PasswordHash[i])
                return Unauthorized("Password Incorrect.");
        }
        
        return Ok(
            new UserResponseDTO { Username = user.UserName, Token = tokenService.CreateToken(user), PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain == true)?.Url, KnownAs = user.KnownAs }
        );
    }

    [HttpPost("update")]
    public async Task<ActionResult<AppUser>> Update(UpdateLoginDTO login)
    {
        var user = await userRepository.GetAppUserByUsernameAsync(login.Username);
        if (user == null)
        {
            return Unauthorized("User Name not exists.");
        }

        using var hmac = new HMACSHA512(user.PasswordSalt);

        var computerHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(login.Password));

        for (int i = 0; i < computerHash.Length; i++)
        {
            if (computerHash[i] != user.PasswordHash[i])
                return Unauthorized("Password Incorrect.");
        }

        using var hmacUpdate = new HMACSHA512();
        user.UserName = login.UpdateUsername.ToLower();
        user.PasswordSalt = hmacUpdate.Key;
        user.PasswordHash = hmacUpdate.ComputeHash(Encoding.UTF8.GetBytes(login.UpdatePassword));

        await userRepository.SaveAllAsync();
        return Ok(user);
    }

    private async Task<bool> UserExists(string username)
    {
        if(!string.IsNullOrWhiteSpace(username)
            && !string.IsNullOrEmpty(username)){
              var user =  await userRepository.GetAppUserByUsernameAsync(username);
              if(user == null) return false;
              if(user.UserName.Equals(username, StringComparison.CurrentCultureIgnoreCase)) return true;
            }
        return false;
    }
}
