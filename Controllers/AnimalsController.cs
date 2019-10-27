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
    /// Core controller for CRUD related behaviour for Animal related administration
    /// </summary>
    public class AnimalsController : ControllerBase
    {
        private readonly DataContext _context;

        public AnimalsController(DataContext context)
        {
            _context = context;
        }

        // GET: api/Animals
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Animal>>> GetAnimals()
        {
            return await _context.Animal.ToListAsync();
        }

        // GET: api/Animals/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Animal>> GetAnimal(long id)
        {
            var animal = await _context.Animal.FindAsync(id);

            //Check if the animal exists OR has been soft deleted
            if (animal == null || animal.IsDeleted == true)
            {
                return NotFound();
            }

            return animal;
        }

        // PUT: api/Animals/5
        [HttpPut("{id}")]
        public async Task<ActionResult<Animal>> PutAnimal(long id, Animal animal)
        {
            //Check if the animal id's are correct OR has been soft deleted
            if (id != animal.Id || animal.IsDeleted == true)
            {
                return BadRequest();
            }

            _context.Entry(animal).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AnimalExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return animal;
        }

        // POST: api/Animals
        [HttpPost]
        public async Task<ActionResult<Animal>> PostAnimal(Animal animal)
        {
            _context.Animal.Add(animal);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAnimal", new { id = animal.Id }, animal);
        }

        // Route: api/Animals/delete/0 to allow for soft deletion & auditing
        [Route("delete/{id:int}")]
        public async Task<ActionResult<Animal>> DeleteAnimal(long id)
        {
            //Check if the animal exists
            var animal = await _context.Animal.FindAsync(id);
            if (animal == null)
            {
                return NotFound();
            }

            //Set "Soft Delete" to true
            animal.IsDeleted = true;

            _context.Entry(animal).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AnimalExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return animal;
        }

        private bool AnimalExists(long id)
        {
            return _context.Animal.Any(e => e.Id == id);
        }
    }
}
