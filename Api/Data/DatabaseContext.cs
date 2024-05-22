using Microsoft.EntityFrameworkCore;
using Models;
using System.Collections.Generic;

namespace Api.Data
{
    public class DatabaseContext : DbContext
    {
        public DbSet<Recipe> Recipes { get; set; }

        public DatabaseContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuration pour Recipe
            modelBuilder.Entity<Recipe>()
                .HasKey(r => r.RecipeId);

            modelBuilder.Entity<Recipe>()
                .Property(r => r.Title)
                .IsRequired()
                .HasMaxLength(200);

            modelBuilder.Entity<Recipe>()
                .Property(r => r.Description)
                .IsRequired()
                .HasMaxLength(1000);

            // Configuration pour Ingredient
            modelBuilder.Entity<Ingredient>()
                .HasKey(i => i.IngredientId);

            modelBuilder.Entity<Ingredient>()
                .Property(i => i.Name)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<Ingredient>()
                .Property(i => i.Quantity)
                .IsRequired()
                .HasMaxLength(50);

            // Relation entre Recipe et Ingredient
            modelBuilder.Entity<Recipe>()
                .HasMany(r => r.Ingredients)
                .WithOne() // Si vous n'avez pas une propriété de navigation de retour dans Ingredient.
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete pour simplifier la gestion des dépendances.
        }
    }
}
