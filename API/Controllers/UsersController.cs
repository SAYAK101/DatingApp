using API.Contracts;
using API.DTOs;
using API.Entities;
using API.Extensions;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class UsersController(
        IUserRepository userRepository,
        IMapper mapper,
        IPhotoService photoService
    ) : BaseApiController
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
            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        [HttpGet("{username}")]
        public async Task<ActionResult<MemberDTO>> GetUser(string username)
        {
            var member = await userRepository.GetMemberAsync(username);
            if (member == null)
            {
                return NotFound();
            }

            return member;
        }

        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDTO updateDto)
        {
            var member = await userRepository.GetAppUserByUsernameAsync(User.GetUsername());
            if (member == null)
            {
                return BadRequest("User not found.");
            }

            mapper.Map(updateDto, member);

            if (await userRepository.SaveAllAsync())
                return NoContent();

            return BadRequest("Failed to update user.");
        }

        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
        {
            var member = await userRepository.GetAppUserByUsernameAsync(User.GetUsername());
            if (member == null)
            {
                return BadRequest("User not found.");
            }

            var result = await photoService.AddPhotoAsync(file);

            if (result.Error != null)
                return BadRequest(result.Error.Message);

            var photo = new Photo
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId
            };

            if(member.Photos.Count == 0) photo.IsMain = true;

            member.Photos.Add(photo);

            if (await userRepository.SaveAllAsync())
                return CreatedAtAction(
                    nameof(GetUser),
                    new { username = member.UserName },
                    mapper.Map<PhotoDto>(photo)
                );

            return BadRequest("Failed to add photo.");
        }

        [HttpPut("set-main-photo/{photoId:int}")]
        public async Task<ActionResult> SetMainPhoto(int photoId)
        {
            var member = await userRepository.GetAppUserByUsernameAsync(User.GetUsername());
            if (member == null)
            {
                return BadRequest("User not found.");
            }

            var photo = member.Photos.Find(x => x.Id == photoId);
            if (photo == null || photo.IsMain)
                return BadRequest("Already set to main photo!");

            var currentMain = member.Photos.FirstOrDefault(x => x.IsMain);
            if (currentMain != null)
                currentMain.IsMain = false;
            photo.IsMain = true;

            if (await userRepository.SaveAllAsync())
                return NoContent();

            return BadRequest("Failed to set main photo.");
        }

        [HttpDelete("delete-photo/{photoId:int}")]
        public async Task<ActionResult> DeletePhoto(int photoId)
        {
            var member = await userRepository.GetAppUserByUsernameAsync(User.GetUsername());
            if (member == null)
            {
                return BadRequest("User not found.");
            }

            var photo = member.Photos.Find(x => x.Id == photoId);
            if (photo == null || photo.IsMain)
                return BadRequest("Cannot be deleted!");

            if (photo.PublicId != null)
            {
                var result = await photoService.DeletePhotoAsync(photo.PublicId);
                if (result.Error != null) return BadRequest(result.Error.Message);
            }

            member.Photos.Remove(photo);

            if (await userRepository.SaveAllAsync()) return Ok();

            return BadRequest("Failed to delete photo.");
        }
    }
}
