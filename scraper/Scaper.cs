using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;
using Models;

public class Scraper
{
    private HttpClient _client;

    public Scraper()
    {
        var handler = new HttpClientHandler()
        {
            ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
        };
        _client = new HttpClient(handler);
    }

    // Récupérer les catégories d'ingrédients
    public async Task<List<string>> GetIngredientCategoriesAsync(string url)
    {
        var ingredientLinks = new List<string>();
        var html = await _client.GetStringAsync(url);
        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        var links = doc.DocumentNode.SelectNodes("//ul[@class='loc mntl-link-list']//a");
        if (links != null)
        {
            foreach (var node in links)
            {
                ingredientLinks.Add(node.GetAttributeValue("href", string.Empty));
            }
        }
        return ingredientLinks;
    }


    public async Task<List<string>> GetRecipesFromCategoryAsync(string categoryUrl)
    {
        var recipeLinks = new List<string>();
        var html = await _client.GetStringAsync(categoryUrl);
        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        var links = doc.DocumentNode.SelectNodes("//a[@data-doc-id]");
        if (links != null)
        {
            foreach (var node in links)
            {
                recipeLinks.Add(node.GetAttributeValue("href", string.Empty));
            }
        }
        return recipeLinks;
    }

    // Récupérer les détails pour chaque recette
    public async Task<Recipe> GetRecipeDetailsAsync(string recipeUrl)
    {
        var html = await _client.GetStringAsync(recipeUrl);
        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        var titleNode = doc.DocumentNode.SelectSingleNode("//h1[contains(@class, 'article-heading')]");
        var descriptionNode = doc.DocumentNode.SelectSingleNode("//p[contains(@class, 'article-subheading')]");
        var ingredientNodes = doc.DocumentNode.SelectNodes("//li[contains(@class, 'mntl-structured-ingredients__list-item')]");

        if (ingredientNodes == null || !ingredientNodes.Any()) 
        {
            return null;
        }

        var recipe = new Recipe
        {
            Title = titleNode?.InnerText.Trim(),
            Description = descriptionNode?.InnerText.Trim(),
            Ingredients = ingredientNodes.Select(node => new Ingredient
            {
                Name = node.SelectSingleNode(".//span[@data-ingredient-name='true']")?.InnerText.Trim(),
                Quantity = node.SelectSingleNode(".//span[@data-ingredient-quantity='true']")?.InnerText.Trim() + " " +
                           node.SelectSingleNode(".//span[@data-ingredient-unit='true']")?.InnerText.Trim()
            }).ToList()
        };

        return recipe;
    }



}

