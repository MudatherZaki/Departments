using Departments.Presistence.Configurations;
using Departments.Presistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace Departments.Presistence
{
    public class ApplicationDbContext: DbContext
    {
        public ApplicationDbContext(DbContextOptions options): base(options)
        {
            
        }
        public DbSet<Department> Departments { get; set; }
        public DbSet<DepartmentConnection> DepartmentConnections { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new DepartmentConfiguration());
            modelBuilder.ApplyConfiguration(new DepartmentConnectionConfiguration());
            base.OnModelCreating(modelBuilder);
        }
    }
}
