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
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Email).HasMaxLength(30).IsRequired();
            builder.Property(x => x.Password).HasMaxLength(30).IsRequired();

            builder.HasOne(x => x.Specialization)
                .WithMany(y => y.Users)
                .HasForeignKey(x => x.SpecializationId);               

            builder.HasMany(x => x.ProjectUsers)
                .WithOne(x => x.User)
                .HasForeignKey(x => x.UserId);

            builder.HasMany(user => user.Tasks)
                .WithOne(task => task.User)
                .HasForeignKey(task => task.UserId);
        }
    }
}
