using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Configurations
{
    public class TaskConfiguration : IEntityTypeConfiguration<Domain.Entities.Task>
    {
        public void Configure(EntityTypeBuilder<Domain.Entities.Task> builder)
        {
            builder.HasKey(task => task.Id);
            builder.Property(task => task.Title).HasMaxLength(20).IsRequired();
            builder.Property(task => task.Description).HasMaxLength(20).IsRequired();
            builder.Property(task => task.Priority).IsRequired();
            builder.Property(task => task.Status).IsRequired();
            builder.Property(task => task.DateCreated).IsRequired();
            builder.Property(task => task.DateEnd).IsRequired();

            builder.HasOne(task => task.Column)
                .WithMany(column => column.Tasks)
                .HasForeignKey(task => task.ColumnId);

            builder.HasOne(task => task.User)
                .WithMany(user => user.Tasks)
                .HasForeignKey(task => task.UserId);
        }
    }
}
