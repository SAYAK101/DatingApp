using API.Contracts;
using API.DTOs;
using API.Entities;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class UserRepository(DataContext context, IMapper mapper) : IUserRepository
{
    public void Add(AppUser user)
    {
        context.Add(user);
    }

    public async Task<AppUser?> GetAppUserByIdAsync(int id)
    {
        return await context.Users.FindAsync(id); 
    }

    public async Task<AppUser?> GetAppUserByUsernameAsync(string username)
    {
        return await context.Users.Include(x => x.Photos).SingleOrDefaultAsync(x => x.UserName == username); 
    }

    public async Task<IEnumerable<AppUser>> GetAppUsersAsync()
    {
        return await context.Users.Include(x => x.Photos).ToListAsync();
    }

    public async Task<MemberDTO?> GetMemberAsync(string username)
    {
        return await context.Users
        .Where(x => x.UserName == username)
        .ProjectTo<MemberDTO>(mapper.ConfigurationProvider)
        .SingleOrDefaultAsync(); 
    }

    public async Task<IEnumerable<MemberDTO>> GetMembersAsync()
    {
        return await context.Users
        .ProjectTo<MemberDTO>(mapper.ConfigurationProvider)
        .ToListAsync();
    }

    public async Task<bool> SaveAllAsync()
    {
        return await context.SaveChangesAsync() > 0;
    }

    public void Update(AppUser user)
    {
        context.Entry(user).State = EntityState.Modified;
    }
}
