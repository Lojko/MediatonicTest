using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MediatonicTest.Models
{
    /// <summary>
    /// Encapsulative DTO class, containing derived data regarding an owned animal
    /// </summary>
    public class OwnedAnimal
    {
        public OwnedAnimal(long Id, string AnimalName, int Happiness, int Hunger)
        {
            this.Id = Id;
            this.AnimalName = AnimalName;
            this.Happiness = Happiness;
            this.Hunger = Hunger;
        }

        public long Id { get; }
        public String AnimalName { get; }
        public int Happiness { get; }
        public int Hunger { get; }
    }
}
