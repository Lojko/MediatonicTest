using MediatonicTest.Models;
using Microsoft.EntityFrameworkCore;

namespace MediatonicTest.Data
{
    public class DataContext : DbContext
    {
        public DataContext()
        {
        }

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<Animal> Animal { get; set; }

        public DbSet<User> User { get; set; }

        public DbSet<AnimalOwnership> AnimalOwnership { get; set; }
    }
}
