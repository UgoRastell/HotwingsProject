using Models.MongoDB;
using Models;
using MongoDB.Driver;
using System.Text.Json;

namespace Api.Services
{
    public class MongoRecipeService : IRecipeService
    {
        private readonly IMongoCollection<MongoRecipe> _recipes;

        public MongoRecipeService(IConfiguration config)
        {
            var client = new MongoClient(config.GetConnectionString("MongoDbConnection"));
            var database = client.GetDatabase("HotWingsDB");
            _recipes = database.GetCollection<MongoRecipe>("Recipes");
        }

        public async Task<IEnumerable<Recipe>> GetAllRecipesAsync()
        {
            var recipes = await _recipes.Find(recipe => true).ToListAsync();
            return recipes.Select(r => new Recipe
            {
                RecipeId = Guid.NewGuid(),
                Title = r.Title,
                Description = r.Description,
                Ingredients = r.Ingredients.Select(i => new Ingredient
                {
                    IngredientId = Guid.NewGuid(),
                    Name = i.Name,
                    Quantity = i.Quantity
                }).ToList()
            });
        }

        public async Task<Recipe> GetRecipeByIdAsync(int recipeId)
        {
            return null;
        }

        public async Task<IEnumerable<Recipe>> SearchRecipesByNameAsync(string name)
        {
            var recipes = await _recipes.Find(recipe => recipe.Title.Contains(name)).ToListAsync();
            return recipes.Select(r => new Recipe
            {
                RecipeId = Guid.NewGuid(),
                Title = r.Title,
                Description = r.Description,
                Ingredients = r.Ingredients.Select(i => new Ingredient
                {
                    IngredientId = Guid.NewGuid(),
                    Name = i.Name,
                    Quantity = i.Quantity
                }).ToList()
            });
        }

        public async Task<IEnumerable<Recipe>> SearchRecipesByIngredientAsync(string ingredient)
        {
            var recipes = await _recipes.Find(recipe => recipe.Ingredients.Any(i => i.Name.Contains(ingredient))).ToListAsync();
            return recipes.Select(r => new Recipe
            {
                RecipeId = Guid.NewGuid(),
                Title = r.Title,
                Description = r.Description,
                Ingredients = r.Ingredients.Select(i => new Ingredient
                {
                    IngredientId = Guid.NewGuid(),
                    Name = i.Name,
                    Quantity = i.Quantity
                }).ToList()
            });
        }

        public async Task ImportRecipesAsync(string jsonFilePath)
        {
            using (var stream = new FileStream(jsonFilePath, FileMode.Open, FileAccess.Read))
            {
                var recipes = await JsonSerializer.DeserializeAsync<List<MongoRecipe>>(stream);
                await _recipes.InsertManyAsync(recipes);
            }
        }

        public async Task<IEnumerable<Recipe>> GetAllRecipesWithoutIngredientsAsync()
        {
            var recipes = await _recipes.Find(recipe => true).ToListAsync();
            return recipes.Select(r => new Recipe
            {
                RecipeId = Guid.NewGuid(),
                Title = r.Title,
                Description = r.Description,
                Ingredients = new List<Ingredient>()
            });
        }
    }
}
