using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MediatonicTest.Models
{
    /// <summary>
    /// Entity AnimalOwnership class which encapsulates default data relationship data between a user and an animal, as well 
    /// as functional behaviour that can be executed to change state, which is then persisted.
    /// </summary>
    public partial class AnimalOwnership
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public long AnimalId { get; set; }
        public string Name { get; set; }
        public int Hunger { get; private set; }
        public int Happiness { get; private set; }
        public DateTime LastUpdated { get; set; }
        public bool IsDeleted { get; set; }

        [ForeignKey("AnimalId")]
        public virtual Animal Animal { get; set; }
        public virtual User User { get; set; }

        /// <summary>
        /// Stroke method used as a means to increase an <seealso cref="Animal">s happiness.
        /// </summary>
        public void Stroke()
        {
            Happiness++;
        }

        /// <summary>
        /// Feed method used as a means to decrease an <seealso cref="Animal">s hunger.
        /// </summary>
        public void Feed()
        {
            Hunger--;
        }

        public void DegradeStats(int multiplier)
        {
            //Validate the multiplier is apporpriate, otherwise do not apply it
            if (multiplier > 0)
            {
                this.Happiness -= (Animal.HappinessDecrease * multiplier);
                this.Hunger += (Animal.HungerIncrease * multiplier);
            }
        }

        public void SetToDefaults(Animal animal)
        {
            this.Happiness = animal.HappinessDefault;
            this.Hunger = animal.HungerDefault;
        }
    }
}
