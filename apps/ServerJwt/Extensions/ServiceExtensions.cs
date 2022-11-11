using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ServerJwt.Data;
using ServerJwt.Services;

namespace ServerJwt.Extensions
{
  public static class ServiceExtensions
  {
    public static void ConfigureJWT(this IServiceCollection services)
    {
      services.AddAuthentication(opt =>
      {
        opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
      })
          .AddJwtBearer(options =>
          {
            options.TokenValidationParameters = new TokenValidationParameters
            {
              ValidateIssuer = true,
              ValidateAudience = true,
              ValidateLifetime = true,
              ValidateIssuerSigningKey = true,
              ValidIssuer = "https://localhost:5001",
              ValidAudience = "https://localhost:5001",
              IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("superSecretKey@345"))
            };
          });
    }

    public static void ConfigureCORS(this IServiceCollection services)
    {
      services.AddCors(options =>
      {
        options.AddPolicy("EnableCORS", builder =>
        {
          builder.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
        });
      });
    }

    public static void ConfigureDbContext(this IServiceCollection services, IConfiguration configuration)
    {
      services.AddDbContext<UserContext>(opts =>
            opts.UseSqlServer(configuration["ConnectionString:UserDB"]));
    }

    public static void ConfigureJwtServices(this IServiceCollection services)
    {
      services.AddTransient<ITokenService, TokenService>();
    }
  }
}
