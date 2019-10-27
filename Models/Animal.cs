using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MediatonicTest.Models
{
    /// <summary>
    /// Entity Animal class which encapsulates an Animal as it's default properites, type etc...
    /// </summary>
    public class Animal
    {
        public Animal()
        {
            AnimalOwnership = new HashSet<AnimalOwnership>();
        }

        public long Id { get; set; }
        public string Type { get; set; }
        public int HappinessDefault { get; set; }
        public int HappinessDecrease { get; set; }
        public int HungerDefault { get; set; }
        public int HungerIncrease { get; set; }
        public bool IsDeleted { get; set; }

        public virtual ICollection<AnimalOwnership> AnimalOwnership { get; set; }
    }
}
