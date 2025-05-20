namespace TalabatAPIs.Exctensions
{
    public static class Swagger
    {
        public static IServiceCollection AddSwagger(this IServiceCollection Services)
        {
            /* 1 => Install-Package Swashbuckle.AspNetCore (in PMC) */
            /* 2 => */
            Services.AddEndpointsApiExplorer();
            Services.AddSwaggerGen();

            return Services;
        }
        public static WebApplication AddSwagger(this WebApplication app)
        {
            /* 3 => "launchBrowser": true in launchSettings.json */
            /* 4 => */
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                c.RoutePrefix = string.Empty; // Open Swagger at root URL
            });
            return app;
        }
    }
}
