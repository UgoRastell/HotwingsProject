using Api.Data;
using Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.IO;

namespace Services
{
    public class RecipeService : IRecipeService
    {
        private readonly DatabaseContext _context;

        public RecipeService(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Recipe>> GetAllRecipesAsync()
        {
            return await _context.Recipes.ToListAsync();
        }

        public async Task<Recipe> GetRecipeByIdAsync(int recipeId)
        {
            return await _context.Recipes.FindAsync(recipeId);
        }

        public async Task<IEnumerable<Recipe>> SearchRecipesByNameAsync(string name)
        {
            return await _context.Recipes
                .Include(r => r.Ingredients)
                .Where(r => r.Title.Contains(name))
                .ToListAsync();
        }

        public async Task<IEnumerable<Recipe>> SearchRecipesByIngredientAsync(string ingredient)
        {
            return await _context.Recipes
                .Include(r => r.Ingredients)
                .Where(r => r.Ingredients.Any(i => i.Name.Contains(ingredient)))
                .ToListAsync();
        }

        public async Task ImportRecipesAsync(string jsonFilePath)
        {
            using (var stream = new FileStream(jsonFilePath, FileMode.Open, FileAccess.Read))
            {
                var recipes = await JsonSerializer.DeserializeAsync<List<Recipe>>(stream);
                _context.Recipes.AddRange(recipes);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Recipe>> GetAllRecipesWithoutIngredientsAsync()
        {
            return await _context.Recipes
                .Select(r => new Recipe
                {
                    RecipeId = r.RecipeId,
                    Title = r.Title,
                    Description = r.Description,
                })
                .ToListAsync();
        }
    }
}
