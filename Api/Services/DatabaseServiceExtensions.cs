using Api.Data;
using Microsoft.EntityFrameworkCore;
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
                services.AddDbContext<DatabaseContext>(options =>
                    options.UseMySql(config.GetConnectionString("DefaultConnection"),
                    ServerVersion.AutoDetect(config.GetConnectionString("DefaultConnection"))));

                services.AddScoped<IRecipeService, RecipeService>();
            }
        }
    }
}
