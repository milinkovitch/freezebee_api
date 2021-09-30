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
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProcessesController : ControllerBase
    {
        private readonly FreezebeeContext _context;

        public ProcessesController(FreezebeeContext context)
        {
            _context = context;
        }

        // GET: api/Processes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Process>>> GetProcesses()
        {
            return await _context.Processes.Include(p => p.Steps).ToListAsync();
        }

        // GET: api/Processes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Process>> GetProcess(Guid id)
        {
            var process = await _context.Processes.FindAsync(id);

            if (process == null)
            {
                return NotFound();
            }

            return process;
        }

        // PUT: api/Processes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProcess(Guid id, Process process)
        {
            if (id != process.Id)
            {
                return BadRequest();
            }

            Process currentProcess = await _context.Processes.Include(m => m.Steps).FirstOrDefaultAsync(i => i.Id == id);
            currentProcess.Name = process.Name;
            currentProcess.Description = process.Description;

            _context.Entry(currentProcess).State = EntityState.Modified;

            ICollection<Step> selectedSteps = process.Steps;
            ICollection<Step> existingSteps = currentProcess.Steps;
            ICollection<Step> toAdd = selectedSteps.Where(s => s.Id == Guid.Empty).ToList();
            ICollection<Step> toEdit = selectedSteps.Where(s => existingSteps.Select(e => e.Id).Contains(s.Id)).ToList();
            ICollection<Step> toRemove = existingSteps.Where(e => !selectedSteps.Select(s => s.Id).Contains(e.Id)).ToList();
            foreach (Step step in currentProcess.Steps)
            {
                Step oldStep = toEdit.Where(t => t.Id == step.Id).First();
                step.Name = oldStep.Name;
                step.Description = oldStep.Description;
                step.Order = oldStep.Order;
            }

            foreach (Step step in toAdd)
            {
                currentProcess.Steps.Add(step);
            }

            foreach (Step step in toRemove)
            {
                currentProcess.Steps.Remove(step);
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProcessExists(id))
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

        // POST: api/Processes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Process>> PostProcess(Process process)
        {
            _context.Processes.Add(process);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProcess", new { id = process.Id }, process);
        }

        // DELETE: api/Processes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProcess(Guid id)
        {
            var process = await _context.Processes.Include(p => p.Steps).FirstOrDefaultAsync(i => i.Id == id);
            foreach (Step step in process.Steps)
            {
                _context.Steps.Remove(step);
            }
            if (process == null)
            {
                return NotFound();
            }

            _context.Processes.Remove(process);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProcessExists(Guid id)
        {
            return _context.Processes.Any(e => e.Id == id);
        }
    }
}
