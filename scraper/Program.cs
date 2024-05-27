using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;
using Models;
using System.Text.Json;
using System.IO;

class Program
{
    static async Task Main(string[] args)
    {
        var scraper = new Scraper();
        var categories = await scraper.GetIngredientCategoriesAsync("https://www.allrecipes.com/ingredients-a-z-6740416");
        var allRecipes = new List<Recipe>();

        var recipeTasks = new List<Task<List<Recipe>>>();
        foreach (var categoryUrl in categories)
        {
            recipeTasks.Add(ProcessCategory(scraper, categoryUrl));
        }
        var recipesFromAllCategories = await Task.WhenAll(recipeTasks);

        foreach (var categoryRecipes in recipesFromAllCategories) // Renommé 'recipes' en 'categoryRecipes'
        {
            allRecipes.AddRange(categoryRecipes);
        }

        // Écrire toutes les recettes dans un fichier JSON sans filtrer les doublons
        var initialJson = JsonSerializer.Serialize(allRecipes);
        await File.WriteAllTextAsync("C:/Users/ugora/source/repos/UgoRastell/HotwingsProject/scraper/data.json", initialJson);

        // Lire le fichier JSON
        var jsonData = await File.ReadAllTextAsync("C:/Users/ugora/source/repos/UgoRastell/HotwingsProject/scraper/data.json");
        var deserializedRecipes = JsonSerializer.Deserialize<List<Recipe>>(jsonData); // Renommé 'recipes' en 'deserializedRecipes'

        // Filtrer les doublons
        var uniqueRecipes = deserializedRecipes.DistinctBy(r => r.Title).ToList();

        // Réécrire les données filtrées dans le fichier JSON
        var filteredJson = JsonSerializer.Serialize(uniqueRecipes);
        await File.WriteAllTextAsync("C:/Users/ugora/source/repos/UgoRastell/HotwingsProject/scraper/data.json", filteredJson);
        Console.WriteLine($"Saved {uniqueRecipes.Count} unique recipes to data.json");
    }

    static async Task<List<Recipe>> ProcessCategory(Scraper scraper, string categoryUrl)
    {
        var recipes = new List<Recipe>();
        var recipeUrls = await scraper.GetRecipesFromCategoryAsync(categoryUrl);
        foreach (var recipeUrl in recipeUrls)
        {
            var recipeDetails = await scraper.GetRecipeDetailsAsync(recipeUrl);
            if (recipeDetails != null)
            {
                recipes.Add(recipeDetails);
            }
        }
        return recipes;
    }
}
