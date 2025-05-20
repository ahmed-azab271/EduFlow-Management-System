using Core.Entites.Identity;
using Core.IServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Repository.Identity;
using Services;
using System.Text;

namespace APIs.Exctensions
{
    public static class IdentityServices
    {
        public static IServiceCollection AddIdentityServices (this IServiceCollection Services , IConfiguration configuration)
        {
            Services.AddScoped(typeof(ITokenService), typeof(TokenService));
            Services.AddIdentity<ApplicationUser, IdentityRole>()
                    .AddEntityFrameworkStores<AppIdentityDbContext>();

            // ( Services.AddAuthentication ) for alllowind Dependancy Inhection to UserManger / SigninManger / RoleManger
            Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = configuration["JWT:ValidIssuer"],
                    ValidateAudience = true,
                    ValidAudience = configuration["JWT:ValidAudience"],
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Key"]))
                };
            });

            return Services;
        }
    }
}
