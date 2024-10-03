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
    public class SpecializationConfiguration : IEntityTypeConfiguration<Specialization>
    {
        public void Configure(EntityTypeBuilder<Specialization> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name);
            builder.HasMany(x => x.Users)
                .WithOne(y => y.Specialization)
                .HasForeignKey(x => x.Id)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
