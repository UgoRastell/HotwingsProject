using System.ComponentModel.DataAnnotations;

namespace Models
{

    public class Recipe
    {
        [Key]
        public Guid RecipeId { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [Required]
        [StringLength(1000)]
        public string Description { get; set; }

        public List<Ingredient> Ingredients { get; set; }
    }

}
