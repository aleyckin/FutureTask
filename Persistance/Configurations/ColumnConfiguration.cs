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
    public class ColumnConfiguration : IEntityTypeConfiguration<Column>
    {
        public void Configure(EntityTypeBuilder<Column> builder)
        {
            builder.HasKey(column => column.Id);
            builder.Property(column => column.Title);

            builder.HasOne(column => column.Project)
                .WithMany(project => project.Columns)
                .HasForeignKey(column => column.ProjectId);

            builder.HasMany(column => column.Tasks)
                .WithOne(task => task.Column)
                .HasForeignKey(task => task.ColumnId);
        }
    }
}
