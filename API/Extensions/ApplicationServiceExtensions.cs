using API.Contracts;
using API.Data;
using API.Helpers;
using API.Services;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplicationService(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();
        services.AddDbContext<DataContext>(opt => opt.UseSqlite(configuration.GetConnectionString("DefaultConnection")));
        services.AddCors();
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IPhotoService, PhotoService>();
        services.Configure<CloudinarySettings>(configuration.GetSection("CloudinarySetting"));
        return services;
    }
}
