using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MediatonicTest.Data;
using MediatonicTest.Models;

namespace MediatonicTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    /// <summary>
    /// Core controller for CRUD related behaviour for Animal Ownership related administration
    /// </summary>
    public class AnimalOwnershipsController : ControllerBase
    {
        private readonly DataContext _context;

        public AnimalOwnershipsController(DataContext context)
        {
            _context = context;
        }

        // GET: api/AnimalOwnerships
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AnimalOwnership>>> GetAnimalOwnership()
        {
            return await _context.AnimalOwnership.ToListAsync();
        }

        // GET: api/AnimalOwnerships/5
        [HttpGet("{id}")]
        [ProducesResponseType(404)]
        public async Task<ActionResult<AnimalOwnership>> GetAnimalOwnership(long id)
        {
            var animalOwnership = await _context.AnimalOwnership.FindAsync(id);

            if (animalOwnership == null)
            {
                return NotFound();
            }

            return animalOwnership;
        }

        // PUT: api/AnimalOwnerships/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<ActionResult<AnimalOwnership>> PutAnimalOwnership(long id, AnimalOwnership animalOwnership)
        {
            if (id != animalOwnership.Id)
            {
                return BadRequest();
            }

            //Update the updated field
            animalOwnership.LastUpdated = DateTime.Now;

            _context.Entry(animalOwnership).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AnimalOwnershipExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return animalOwnership;
        }

        // POST: api/AnimalOwnerships
        [HttpPost]
        public async Task<ActionResult<AnimalOwnership>> PostAnimalOwnership(AnimalOwnership animalOwnership)
        {
            Animal animal = await _context.Animal.FindAsync(animalOwnership.AnimalId);

            //Check if the Animal exists
            if (animal == null)
            {
                return NotFound();
            }

            //Validate default values have been appropriately set
            if (animalOwnership.Happiness != animal.HappinessDefault || animalOwnership.Hunger != animal.HungerDefault)
            {
                //Override defaults
                animalOwnership.SetToDefaults(animal);
            }

            _context.AnimalOwnership.Add(animalOwnership);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAnimalOwnership", new { id = animalOwnership.Id }, animalOwnership);
        }

        // DELETE: api/AnimalOwnerships/5
        [Route("delete/{id:int}")]
        public async Task<ActionResult<AnimalOwnership>> DeleteAnimalOwnership(long id)
        {
            //Check if the ownership exists
            var animalOwnership = await _context.AnimalOwnership.FindAsync(id);
            if (animalOwnership == null)
            {
                return NotFound();
            }

            animalOwnership.IsDeleted = true;
            _context.Entry(animalOwnership).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AnimalOwnershipExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return animalOwnership;
        }

        private bool AnimalOwnershipExists(long id)
        {
            return _context.AnimalOwnership.Any(e => e.Id == id);
        }
    }
}
