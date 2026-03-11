using System;
using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Locations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DirectoryService.Infrastructure.Configurations;

public sealed class DepartmentLocationConfiguration : IEntityTypeConfiguration<DepartmentLocation>
{
    public void Configure(EntityTypeBuilder<DepartmentLocation> builder)
    {
        builder.ToTable("departments_locations");

        builder.HasKey(dl => dl.Id);
        builder.Property(dl => dl.Id).HasColumnName("id");

        builder.HasOne<Department>().WithMany(x => x.Locations).HasForeignKey(x => x.DepartmentId).IsRequired();
        builder.Property(dl => dl.DepartmentId).HasColumnName("department_id").IsRequired();

        builder.HasOne<Location>().WithMany(x => x.Departments).HasForeignKey(x => x.LocationId).IsRequired();
        builder.Property(dl => dl.LocationId).HasColumnName("location_id").IsRequired();
    }
}
