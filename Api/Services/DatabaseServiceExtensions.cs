using Api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Models;
using Services;

namespace Api.Services
{
    public static class DatabaseServiceExtensions
    {
        public static void AddDatabaseServices(this IServiceCollection services, IConfiguration config)
        {
            var useMongoDb = config.GetValue<bool>("UseMongoDb");

            if (useMongoDb)
            {
                services.AddScoped<IRecipeService, MongoRecipeService>();
            }
            else
            {
                var connectionString = config.GetConnectionString("DefaultConnection");
                Console.WriteLine($"Attempting to connect to MySQL with connection string: {connectionString}");

                services.AddDbContext<DatabaseContext>(options =>
                    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
                           .LogTo(Console.WriteLine, LogLevel.Information)); // Logging to console for debugging

                services.AddScoped<IRecipeService, RecipeService>();
            }
        }
    }
}
