using backendCourse1.api.Entities;
using Microsoft.EntityFrameworkCore;

namespace backendCourse1.api.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<AppUser> Users { get; set; }
    }
}