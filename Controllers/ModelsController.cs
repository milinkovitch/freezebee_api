using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using freezebee_api.Models;
using Microsoft.AspNetCore.Authorization;

namespace freezebee_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ModelsController : ControllerBase
    {
        private readonly FreezebeeContext _context;

        public ModelsController(FreezebeeContext context)
        {
            _context = context;
        }

        // GET: api/Models
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Model>>> GetModels(string search, string ingredient)
        {
            IQueryable<Model> query = _context.Models;

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(e => e.Name.Contains(search) || e.Description.Contains(search) || e.Price.ToString().Contains(search));
            }

            if (ingredient != null)
            {
                //query = query.Where(e => e.Ingredients.Contains.(ingredient));
            }

            return await query.Include(m => m.IngredientModels).ThenInclude(im => im.Ingredient).AsNoTracking().ToListAsync();
        }

        // GET: api/Models/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Model>> GetModel(Guid id)
        {
            var model = await _context.Models.FindAsync(id);

            if (model == null)
            {
                return NotFound();
            }

            return model;
        }

        // PUT: api/Models/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutModel(Guid id, ModelCreate mc)
        {
            if (id != mc.Id)
            {
                return BadRequest();
            }

            Model model = await _context.Models.Include(m => m.IngredientModels).FirstOrDefaultAsync(m => m.Id == id);
            model.Name = mc.Name;
            model.Description = mc.Description;
            model.Range = mc.Range;
            model.Price = mc.Price;

            _context.Entry(model).State = EntityState.Modified;

            ICollection<Ingredient> selectedIngredients = await _context.Ingredients.Where(i => mc.Ingredients.Contains(i.Id)).ToListAsync();
            ICollection<IngredientModel> existingIM = model.IngredientModels;

            foreach (Ingredient i in selectedIngredients)
            {
                IngredientModel selectedIM = existingIM.Where(im => im.IngredientId == i.Id && model.Id == im.ModelId).FirstOrDefault();
                if (selectedIM == null)
                {
                    model.IngredientModels.Add(new IngredientModel
                    { ModelId = model.Id, IngredientId = i.Id });
                }
            }

            foreach (IngredientModel im in existingIM)
            {
                if (!mc.Ingredients.Contains(im.IngredientId))
                {
                    model.IngredientModels.Remove(im);
                }
            }

            try
            {
                await _context.SaveChangesAsync();
            }

            catch (DbUpdateConcurrencyException)
            {
                if (!ModelExists(id))
                {
                    return NotFound();
                }
                else
                {   
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Models
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ModelCreate>> PostModel(ModelCreate mc)
        {
            Model model = new Model()
            {
                Name = mc.Name,
                Description = mc.Description,
                Price = mc.Price
            };

            IList<IngredientModel> IngredientModels = new List<IngredientModel>();
            if (mc.Ingredients != null)
            {
                List<Ingredient> ingredients = await _context.Ingredients.Where(i => mc.Ingredients.Contains(i.Id)).ToListAsync();
                foreach (Ingredient ingredient in ingredients)
                {
                    IngredientModel im = new IngredientModel { IngredientId = ingredient.Id };
                    IngredientModels.Add(im);
                }
            }
            model.IngredientModels = IngredientModels;

            _context.Models.Add(model);
            await _context.SaveChangesAsync();
            //test
            return CreatedAtAction("GetModel", new { id = model.Id }, mc);
        }

        // DELETE: api/Models/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteModel(Guid id)
        {
            var model = await _context.Models.FindAsync(id);
            if (model == null)
            {
                return NotFound();
            }

            _context.Models.Remove(model);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ModelExists(Guid id)
        {
            return _context.Models.Any(e => e.Id == id);
        }
    }
}
