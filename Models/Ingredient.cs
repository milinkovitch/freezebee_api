using System;
using System.Collections.Generic;

namespace freezebee_api.Models
{
    public class Ingredient : IEntityBase
    {
        public Guid Id { get; set; }
        public int Price { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public ICollection<IngredientModel> IngredientModels { get; set; }
    }
}