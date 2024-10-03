using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Configurations
{
    public class AdministratorConfiguration : IEntityTypeConfiguration<Administrator>
    {
        public void Configure(EntityTypeBuilder<Administrator> builder)
        {
            builder.HasKey(administrator => administrator.Id);
            builder.Property(administrator => administrator.Id).ValueGeneratedOnAdd();
            builder.Property(administrator => administrator.Email).HasMaxLength(30).IsRequired();
            builder.Property(administrator => administrator.Password).HasMaxLength(30).IsRequired();
            builder.HasMany(administrator => administrator.Projects)
                .WithOne()
                .HasForeignKey(project => project.AdministratorId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
