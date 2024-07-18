using System.Security.Claims;
using API.Contracts;
using API.DTOs;
using API.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class UsersController(IUserRepository userRepository, IMapper mapper) : BaseApiController
    {
        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDTO>>> GetUsers()
        {
            var users = await userRepository.GetMembersAsync();
            
            return Ok(users);
        }

       
        [HttpGet("{id:int}")]
        public async Task<ActionResult<AppUser>> GetUserById(int id)
        {
            var user = await userRepository.GetAppUserByIdAsync(id);
            if(user == null){
                return NotFound();
            }

            return user;
        }

        [HttpGet("{username}")]
        public async Task<ActionResult<MemberDTO>> GetUser(string username)
        {
            var member = await userRepository.GetMemberAsync(username);
            if(member == null){
                return NotFound();
            }

            return member;
        }

        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDTO updateDto)
        {

            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if(username == null){
                return BadRequest("User not found.");
            }

            var member = await userRepository.GetAppUserByUsernameAsync(username);
            if(member == null){
                return BadRequest("User not found.");
            }

            mapper.Map(updateDto, member);

            if(await userRepository.SaveAllAsync()) return NoContent();

            return BadRequest("Failed to update user.");
        }
    }
}
