using System.Security.Cryptography;
using System.Text;
using API.Contracts;
using API.Data;
using API.DTOs;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AccountController(DataContext context, ITokenService tokenService) : BaseApiController
{
    [HttpPost("register")]
    public async Task<ActionResult<UserResponseDTO>> Register(RegisterDTO userDto)
    {
        if (userDto.username == null || userDto.username == "" || userDto.username == " ")
        {
            return BadRequest("Username is Empty.");
        }
        var userExists = await UserExists(userDto.username);
        if (userExists)
        {
            return BadRequest("Username is taken.");
        }

        using var hmac = new HMACSHA512();
        var user = new AppUser
        {
            UserName = userDto.username.ToLower(),
            PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(userDto.password)),
            PasswordSalt = hmac.Key
        };

        context.Users.Add(user);
        await context.SaveChangesAsync();

        return Ok(new UserResponseDTO
        {
            Username = user.UserName,
            Token = tokenService.CreateToken(user)
        });
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserResponseDTO>> Login(LoginDTO login)
    {
        var user = await context.Users.FirstOrDefaultAsync(x => x.UserName == login.Username.ToLower());
        if (user == null)
        {
            return Unauthorized("User Name not exists.");
        }

        using var hmac = new HMACSHA512(user.PasswordSalt);

        var computerHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(login.Password));

        for (int i = 0; i < computerHash.Length; i++)
        {
            if (computerHash[i] != user.PasswordHash[i]) return Unauthorized("Password Incorrect.");
        }

        return Ok(new UserResponseDTO
        {
            Username = user.UserName,
            Token = tokenService.CreateToken(user)
        });

    }

    [HttpPost("update")]
    public async Task<ActionResult<AppUser>> Update(UpdateLoginDTO login)
    {
        var user = await context.Users.FirstOrDefaultAsync(x => x.UserName == login.Username);
        if (user == null)
        {
            return Unauthorized("User Name not exists.");
        }

        using var hmac = new HMACSHA512(user.PasswordSalt);

        var computerHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(login.Password));

        for (int i = 0; i < computerHash.Length; i++)
        {
            if (computerHash[i] != user.PasswordHash[i]) return Unauthorized("Password Incorrect.");
        }

        using var hmacUpdate = new HMACSHA512();
        user.UserName = login.UpdateUsername.ToLower();
        user.PasswordSalt = hmacUpdate.Key;
        user.PasswordHash = hmacUpdate.ComputeHash(Encoding.UTF8.GetBytes(login.UpdatePassword));

        await context.SaveChangesAsync();
        return Ok(user);

    }

    private async Task<bool> UserExists(string username)
    {
        return await context.Users.AnyAsync(x => !string.IsNullOrWhiteSpace(username) && !string.IsNullOrEmpty(username) && x.UserName.ToLower() == username.ToLower());
    }
}
