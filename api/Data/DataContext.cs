using datingApp.api.Entities;
using Microsoft.EntityFrameworkCore;

namespace datingApp.api.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<AppUser> Users { get; set; }
    }
}