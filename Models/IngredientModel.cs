using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace freezebee_api.Models
{
    public class IngredientModel
    {
        public int IngredientId { get; set; }
        public Ingredient Ingredient { get; set; }
        public int ModelId { get; set; }
        public Model Model { get; set; }

        public float Weight { get; set; }
    }

}
