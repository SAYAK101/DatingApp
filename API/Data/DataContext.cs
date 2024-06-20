using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class DataContext(DbContextOptions dbContextOptions) : DbContext(dbContextOptions)
    {
        public DbSet<AppUser> Users { get; set; }
    }
}
