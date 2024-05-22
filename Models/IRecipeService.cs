using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public interface IRecipeService
    {
        Task<IEnumerable<Recipe>> GetAllRecipesAsync();
        Task<Recipe> GetRecipeByIdAsync(int recipeId);
        Task<IEnumerable<Recipe>> SearchRecipesByNameAsync(string name);
        Task<IEnumerable<Recipe>> SearchRecipesByIngredientAsync(string ingredient);
        Task ImportRecipesAsync(string jsonFilePath);
        Task<IEnumerable<Recipe>> GetAllRecipesWithoutIngredientsAsync();
    }

}
