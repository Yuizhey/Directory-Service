using System;
using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Positions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DirectoryService.Infrastructure.Configurations;

public sealed class DepartmentPositionConfiguration : IEntityTypeConfiguration<DepartmentPosition>
{
    public void Configure(EntityTypeBuilder<DepartmentPosition> builder)
    {
        builder.ToTable("departments_positions");

        builder.HasKey(dp => dp.Id);
        builder.Property(dp => dp.Id).HasColumnName("id");

        builder.HasOne<Department>().WithMany(x => x.Positions).HasForeignKey(x => x.DepartmentId).IsRequired();
        builder.Property(dp => dp.DepartmentId).HasColumnName("department_id").IsRequired();

        builder.HasOne<Position>().WithMany(x => x.Departments).HasForeignKey(x => x.PositionId).IsRequired();
        builder.Property(dp => dp.PositionId).HasColumnName("position_id").IsRequired();
    }
}
