using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecipesController : ControllerBase
    {
        private readonly IRecipeService _recipeService;

        public RecipesController(IRecipeService recipeService)
        {
            _recipeService = recipeService;
        }

        [HttpPost("import")]
        public async Task<IActionResult> ImportRecipes(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            var filePath = Path.GetTempFileName();

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            await _recipeService.ImportRecipesAsync(filePath);

            return Ok("File imported successfully.");
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Recipe>>> GetAllRecipesWithoutIngredients()
        {
            var recipes = await _recipeService.GetAllRecipesWithoutIngredientsAsync();
            return Ok(recipes);
        }

        [HttpGet("searchByIngredient")]
        public async Task<ActionResult<IEnumerable<Recipe>>> SearchRecipesByIngredient([FromQuery] string ingredient)
        {
            if (string.IsNullOrWhiteSpace(ingredient))
            {
                return BadRequest("Ingredient parameter is required.");
            }

            var recipes = await _recipeService.SearchRecipesByIngredientAsync(ingredient);
            return Ok(recipes);
        }

        [HttpGet("searchByName")]
        public async Task<ActionResult<IEnumerable<Recipe>>> SearchRecipesByName([FromQuery] string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return BadRequest("Name parameter is required.");
            }

            var recipes = await _recipeService.SearchRecipesByNameAsync(name);
            return Ok(recipes);
        }
    }
}
