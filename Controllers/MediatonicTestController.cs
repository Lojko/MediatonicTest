using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatonicTest.Data;
using MediatonicTest.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MediatonicTest.Controllers
{
    /**
     * Core controller, main get is loaded by default, routing the "service" related functionality for interaction with animals.
     */
    [ApiController]
    [Route("service")]
    public class MediatonicTestController : ControllerBase
    {
        // Total amount of time to assess OwnedAnimal degredation whenever an OwnedAnimal is retrieved
        private const double DEGRADING_TIMEFRAME_IN_MILLISECONDS = 60 * 60 * 1000;

        // Max value a degradation can be set (Can an Animal die?)
        private const int MAX_DEGRADATION = 100;

        // Year of production, as some default values for last updated default to before this time
        private const int YEAR_OF_PRODUCTION = 2019;

        // DataContext access to the underlying persistance storage
        private readonly DataContext _context;

        public MediatonicTestController(DataContext context)
        {
            _context = context;
        }

        // GET: service
        [HttpGet]
        public IEnumerable<OwnedAnimal> Get()
        {
            var ownedAnimals = _context.AnimalOwnership.Include(a => a.Animal).ToList();

            return ownedAnimals.Select(x => new OwnedAnimal(x.Id, x.Name, x.Happiness, x.Hunger));
        }

        // Routed: e.g. service/getAnimalsForUser/5
        [Route("getAnimalsForUser/{id:int}")]
        public IEnumerable<OwnedAnimal> GetAnimalsForUser(long id)
        {
            var ownedAnimals = _context.AnimalOwnership
                .Where(i => i.UserId == id)
                .Include(a => a.Animal)
                .ToList();

            //Validate nothing has been returned (or the user doesn't own any animals)
            if (ownedAnimals.Count == 0)
            {
                //Return empty list
                return new List<OwnedAnimal>();
            }

            //This is configured to Eagerly load the foreign data, couldn't make lazy loading functional :@
            return ownedAnimals.Select(x => new OwnedAnimal(x.Id, x.Name, x.Happiness, x.Hunger));

        }

        // Routed: e.g. service/getAnimal/5
        [Route("getAnimal/{id:int}")]
        public async Task<ActionResult<OwnedAnimal>> GetAnimal(long id)
        {
            //This is configured to Eagerly load the foreign data, couldn't make lazy loading functional :@
            AnimalOwnership ownedAnimal = await _context.AnimalOwnership.Include(a => a.Animal).SingleOrDefaultAsync(i => i.Id == id);

            //Null check if the animal doesn't exist
            if (ownedAnimal == null)
            {
                return NotFound();
            }


            //On retrieval validate and degrade appropriately
            if (!ValidateAndDegradeAnimal(ownedAnimal))
            {
                //If validating state failed, return bad request
                return BadRequest();
            }

            return new OwnedAnimal(ownedAnimal.Id, ownedAnimal.Name, ownedAnimal.Happiness, ownedAnimal.Hunger);
        }

        // Routed: e.g. service/stroke/5
        [Route("stroke/{id:int}")]
        public async Task<ActionResult<OwnedAnimal>> StrokeAnimal(long id) 
        {
            //This is configured to Eagerly load the foreign data, couldn't make lazy loading functional :@
            AnimalOwnership ownedAnimal = await _context.AnimalOwnership.Include(a => a.Animal).SingleOrDefaultAsync(i => i.Id == id);

            if (ownedAnimal == null)
            {
                //If validating state failed, return bad request
                return BadRequest();
            }

            //Validate current state of animal
            if (!ValidateAndDegradeAnimal(ownedAnimal))
            {
                //If validating state failed, return bad request
                return BadRequest();
            }

            //Stroke Animal
            ownedAnimal.Stroke();

            //Persist state
            _context.Entry(ownedAnimal).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return new OwnedAnimal(ownedAnimal.Id, ownedAnimal.Name, ownedAnimal.Happiness, ownedAnimal.Hunger);
        }

        // Routed: e.g. service/feed/5
        [Route("feed/{id:int}")]
        public async Task<ActionResult<OwnedAnimal>> FeedAnimal(long id)
        {
            //This is configured to Eagerly load the foreign data, couldn't make lazy loading functional :@
            AnimalOwnership ownedAnimal = await _context.AnimalOwnership.Include(a => a.Animal).SingleOrDefaultAsync(i => i.Id == id);

            if (ownedAnimal == null)
            {
                //If validating state failed, return bad request
                return BadRequest();
            }

            //Validate current state of animal
            if (!ValidateAndDegradeAnimal(ownedAnimal))
            {
                //If validating state failed, return bad request
                return BadRequest();
            }

            //Stroke Animal
            ownedAnimal.Feed();

            //Persist state
            _context.Entry(ownedAnimal).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return new OwnedAnimal(ownedAnimal.Id, ownedAnimal.Name, ownedAnimal.Happiness, ownedAnimal.Hunger);
        }

        /// <summary> Validate Owned Animal state, whenever the animal is requested via the API validate the Animal doesn't need degradation on it's stats </summary>
        /// <returns> bool value representing whether it successfully validated and or degraded the <seealso cref="AnimalOwnership"> object </returns>
        private bool ValidateAndDegradeAnimal(AnimalOwnership ownedAnimal)
        {
            //New Animals with a default timestamp which is less than this year could be generated via auto creation, so set their update time to now.
            if (ownedAnimal.LastUpdated.Year < YEAR_OF_PRODUCTION)
            {
                //Animal is freshly made, set the last updated time to now and continue
                return UpdateAnimal(ownedAnimal);
            }

            //Calculate the degradation multipler
            int degradationMultiplier = (int)Math.Floor((DateTime.Now - ownedAnimal.LastUpdated).TotalMilliseconds / DEGRADING_TIMEFRAME_IN_MILLISECONDS);

            //Min the degradation multiplier against a defined max value
            degradationMultiplier = Math.Min(MAX_DEGRADATION, degradationMultiplier);

            //Check if we need to apply stat degradation
            if (degradationMultiplier > 0)
            {
                //Degrade the animals stats providing the multiplier (time degradation times).
                ownedAnimal.DegradeStats(degradationMultiplier);

                //Return updated animal
                return UpdateAnimal(ownedAnimal);
            }

            //Validation complete without any modifications to Animal status
            return true;
        }

        /// <summary> Private function which updates a provided <seealso cref="AnimalOwnership">. </summary>
        /// <returns> bool value representing whether it successfully updated the <seealso cref="AnimalOwnership"> object </returns>
        private bool UpdateAnimal(AnimalOwnership ownedAnimal)
        {
            //Update the updated field to now
            ownedAnimal.LastUpdated = DateTime.Now;

            _context.Entry(ownedAnimal).State = EntityState.Modified;

            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                //Relationship doesn't exist, fail
                if (!AnimalOwnershipExists(ownedAnimal.Id))
                {
                    return false;
                }
                else
                {
                    throw;
                }
            }

            //Successfully updated the OwnedAnimal
            return true;
        }

        /// <summary> Private function which checks an <seealso cref="AnimalOwnership"> exists </summary>
        /// <returns> bool value representing whether it exists </returns>
        private bool AnimalOwnershipExists(long id)
        {
            return _context.AnimalOwnership.Any(e => e.Id == id);
        }
    }
}
