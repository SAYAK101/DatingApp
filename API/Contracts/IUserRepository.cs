using API.DTOs;
using API.Entities;

namespace API.Contracts;

public interface IUserRepository
{
    void Update(AppUser user);

    Task<bool> SaveAllAsync();

    Task<IEnumerable<AppUser>> GetAppUsersAsync();

    Task<IEnumerable<MemberDTO>> GetMembersAsync();

    Task<MemberDTO?> GetMemberAsync(string username);

    Task<AppUser?> GetAppUserByIdAsync(int id);

    Task<AppUser?> GetAppUserByUsernameAsync(string username);
}
