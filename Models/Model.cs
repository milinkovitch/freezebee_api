using System;
using System.Collections.Generic;

namespace freezebee_api.Models
{
    public class Model : IModel
    {
        public Model()
        {
            this.Ingredients = new HashSet<Ingredient>();

        }
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }
        public string Range { get; set; }

        public virtual ICollection<Ingredient> Ingredients { get; set; }
    }

    public interface IModel : IEntityBase
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }
        public string Range { get; set; }
    }

    public class ModelCreate : IModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }
        public string Range { get; set; }
        public Guid Id { get; set; }
        public Guid[] Ingredients { get; set; }
    }
}