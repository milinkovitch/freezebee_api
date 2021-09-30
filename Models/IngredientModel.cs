using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace freezebee_api.Models
{
    public class IngredientModel
    {
        public Guid IngredientId { get; set; }
        public Ingredient Ingredient { get; set; }
        public Guid ModelId { get; set; }
        public Model Model { get; set; }

        public float Weight { get; set; }
    }

}
