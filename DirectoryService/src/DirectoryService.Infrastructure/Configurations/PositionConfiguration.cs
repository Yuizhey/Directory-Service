using System;
using DirectoryService.Domain;
using DirectoryService.Domain.Positions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DirectoryService.Infrastructure.Configurations;

public class PositionConfiguration : IEntityTypeConfiguration<Position>
{
    public void Configure(EntityTypeBuilder<Position> builder)
    {
        builder.ToTable("positions");

        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).HasColumnName("id");

        builder.OwnsOne(p => p.Name, n =>
        {
            n.Property(p => p.Value)
                .HasColumnName("name")
                .HasMaxLength(LengthConstants.MAX_LENGTH_100)
                .IsRequired();

            n.HasIndex(p => p.Value)
                .IsUnique()
                .HasFilter("is_active = true")
                .HasDatabaseName("ix_active_positions_name");
        });

        builder.ComplexProperty(p => p.Description, n =>
        {
            n.Property(p => p.Value)
                .HasColumnName("description")
                .HasMaxLength(LengthConstants.MAX_LENGTH_1000)
                .IsRequired();
        });

        builder.Property(l => l.IsActive).HasColumnName("is_active").IsRequired().HasDefaultValue(true);

        builder.Property(x => x.CreatedAt).HasColumnName("created_at").IsRequired();

        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at").IsRequired(false);
    }
}
