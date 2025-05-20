using APIs.Exctensions;
using APIs.Helpers;
using APIs.Middlewares;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Repository.Data;
using Repository.Identity;
using System.Threading.Tasks;
using TalabatAPIs.Exctensions;

namespace APIs
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddOpenApi();
            builder.Services.AddSwagger();

            #region DbContexts
            builder.Services.AddDbContext<AppDbContext>(Options =>
            {
                Options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });
            builder.Services.AddDbContext<AppIdentityDbContext>(Options =>
            {
                Options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityConnection"));
            });
            #endregion

            #region Cros Policy
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("MyPolicy", O =>
                {
                    O.AllowAnyHeader();
                    O.AllowAnyMethod();
                    O.AllowAnyOrigin();
                });
            });
            #endregion

            builder.Services.AddIdentityServices(builder.Configuration);
            builder.Services.AddServicesExctension();

            var app = builder.Build();

            #region Update Database
            using var Scope = app.Services.CreateScope();
            var Services = Scope.ServiceProvider;
            var loggerFactory = Services.GetService<ILoggerFactory>();
            try
            {
                var dbcontext = Services.GetService<AppDbContext>();
                await dbcontext.Database.MigrateAsync();

                var IdentityDbContext = Services.GetRequiredService<AppIdentityDbContext>();
                await IdentityDbContext.Database.MigrateAsync();

                var roleManger = Services.GetService<RoleManager<IdentityRole>>();
                await AppIdentitySeeding.Seed(roleManger);
            }
            catch (Exception ex)
            {
                var logger = loggerFactory.CreateLogger<Program>();
                logger.LogError(ex, "An Error Occured During Updating The Database");
            }
            #endregion

            #region Handling Error Messages
            app.UseMiddleware<ExceptionMiddleware>();
            app.UseStatusCodePagesWithRedirects("/errors/{0}");
            #endregion

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }
            app.AddSwagger();
            app.UseHttpsRedirection();
            app.UseCors("MyPolicy");
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapHub<ChatHub>("/chatHub");

            app.MapControllers();

            app.Run();
        }
    }
}
