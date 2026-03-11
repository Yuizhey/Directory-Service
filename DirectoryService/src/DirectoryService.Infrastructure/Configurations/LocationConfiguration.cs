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

        builder.ComplexProperty(v => v.Name, n =>
        {
            n.Property(p => p.Value).HasColumnName("name").HasMaxLength(LengthConstants.MAX_LENGTH_120).IsRequired();
        });

        builder.ComplexProperty(v => v.Address, a =>
        {
            a.Property(p => p.Country).HasColumnName("country").HasMaxLength(LengthConstants.MAX_LENGTH_50).IsRequired();
            a.Property(p => p.City).HasColumnName("city").HasMaxLength(LengthConstants.MAX_LENGTH_50).IsRequired();
            a.Property(p => p.Street).HasColumnName("street").HasMaxLength(LengthConstants.MAX_LENGTH_50).IsRequired();
            a.Property(p => p.HouseNumber).HasColumnName("house_number").IsRequired();
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
