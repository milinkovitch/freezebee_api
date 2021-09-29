using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace freezebee_api.Models
{
    public class FreezebeeContext : DbContext
    {
        public FreezebeeContext(DbContextOptions<FreezebeeContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<IngredientModel>()
                 .HasKey(im => new { im.IngredientId, im.ModelId });
        }

        public DbSet<Model> Models { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Process> Processes { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<IngredientModel> IngredientModels { get; set; }
        public DbSet<Step> Steps { get; set; }
    }
}
