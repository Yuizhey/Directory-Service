using System;
using DirectoryService.Domain;
using DirectoryService.Domain.Locations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DirectoryService.Infrastructure.Configurations;

public class LocationConfiguration : IEntityTypeConfiguration<Location>
{
    public void Configure(EntityTypeBuilder<Location> builder)
    {
        builder.ToTable("locations");

        builder.HasKey(l => l.Id);
        builder.Property(l => l.Id).HasColumnName("id");

        builder.OwnsOne(l => l.Name, n =>
        {
            n.Property(p => p.Value)
            .HasColumnName("name")
            .HasMaxLength(LengthConstants.MAX_LENGTH_120)
            .IsRequired();

            n.HasIndex(p => p.Value)
            .IsUnique()
            .HasDatabaseName("ix_locations_name");
        });

        builder.OwnsOne(l => l.Address, a =>
        {
            a.Property(p => p.Country).HasColumnName("country").IsRequired();
            a.Property(p => p.City).HasColumnName("city").IsRequired();
            a.Property(p => p.Street).HasColumnName("street").IsRequired();
            a.Property(p => p.HouseNumber).HasColumnName("house_number").IsRequired();

            a.HasIndex(p => new
            {
                p.Country,
                p.City,
                p.Street,
                p.HouseNumber,
            })
            .IsUnique()
            .HasDatabaseName("ux_locations_address");
        });

        builder.ComplexProperty(v => v.TimeZone, n =>
        {
            n.Property(p => p.Value).HasColumnName("time_zone").HasMaxLength(LengthConstants.MAX_LENGTH_120).IsRequired();
        });

        builder.Property(l => l.IsActive).HasColumnName("is_active").IsRequired().HasDefaultValue(true);

        builder.Property(x => x.CreatedAt).HasColumnName("created_at").IsRequired();

        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at").IsRequired(false);
    }
}
