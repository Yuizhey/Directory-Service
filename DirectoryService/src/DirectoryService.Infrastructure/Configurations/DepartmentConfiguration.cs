using System;
using DirectoryService.Domain;
using DirectoryService.Domain.Departments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DirectoryService.Infrastructure.Configurations;

public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        builder.ToTable("departments");

        builder.HasKey(d => d.Id);
        builder.Property(p => p.Id).HasColumnName("id");

        builder.ComplexProperty(d => d.Name, n => n.Property(p => p.Value).HasColumnName("name").HasMaxLength(LengthConstants.MAX_LENGTH_150).IsRequired());

        builder.OwnsOne(d => d.Identifier, i =>
        {
            i.Property(p => p.Value)
                .HasColumnName("identifier")
                .HasMaxLength(LengthConstants.MAX_LENGTH_150)
                .IsRequired();

            i.HasIndex(p => p.Value)
                .IsUnique()
                .HasDatabaseName("ix_departments_identifier");
        });

        builder.ComplexProperty(d => d.Path, p => p.Property(p => p.Value).HasColumnName("path").HasColumnType("ltree").HasMaxLength(LengthConstants.MAX_LENGTH_50).IsRequired());

        builder.Property(d => d.Depth).HasColumnName("depth").IsRequired();

        builder.Property(l => l.IsActive).HasColumnName("is_active").IsRequired().HasDefaultValue(true);

        builder.Property(x => x.CreatedAt).HasColumnName("created_at").IsRequired();

        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at").IsRequired(false);
    }
}
