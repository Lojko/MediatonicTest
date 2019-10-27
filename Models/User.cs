using System;
using System.Collections.Generic;

namespace MediatonicTest.Models
{
    /// <summary>
    /// Entity User class which encapsulates data regarding a specific user.
    /// </summary>
    public partial class User
    {
        public User()
        {
            AnimalOwnership = new HashSet<AnimalOwnership>();
        }

        public long Id { get; set; }
        public string Username { get; set; }
        public bool IsDeleted { get; set; }

        public virtual ICollection<AnimalOwnership> AnimalOwnership { get; set; }
    }
}
