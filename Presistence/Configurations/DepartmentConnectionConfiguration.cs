using Departments.Presistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Departments.Presistence.Configurations
{
    public class DepartmentConnectionConfiguration : IEntityTypeConfiguration<DepartmentConnection>
    {
        public void Configure(EntityTypeBuilder<DepartmentConnection> builder)
        {
            builder.HasKey(dc => new { dc.ParentId, dc.ChildId });

            builder.HasOne(dc => dc.Parent)
                .WithMany(p => p.Children)
                .HasForeignKey(p => p.ParentId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(dc => dc.Child)
                .WithMany(p => p.Parents)
                .HasForeignKey(p => p.ChildId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.ToTable("DepartmentConnections", "dbo");
        }
    }
}
