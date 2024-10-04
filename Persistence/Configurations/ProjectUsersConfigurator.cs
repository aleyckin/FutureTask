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
    public class ProjectUsersConfigurator : IEntityTypeConfiguration<ProjectUsers>
    {
        public void Configure(EntityTypeBuilder<ProjectUsers> builder)
        {
            builder.HasKey(x => new { x.ProjectId, x.UserId  });

            builder.HasOne(x => x.Project)
                .WithMany(x => x.ProjectUsers)
                .HasForeignKey(x => x.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(x => x.User)
                .WithMany(x => x.ProjectUsers)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Property(x => x.RoleOnProject).IsRequired();
        }
    }
}
