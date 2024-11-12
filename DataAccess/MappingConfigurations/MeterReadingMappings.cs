using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.MappingConfigurations;

internal class MeterReadingMappings : IEntityTypeConfiguration<MeterReading>
{
    public void Configure(EntityTypeBuilder<MeterReading> builder)
    {
        builder.ToTable("MeterReadings");

        builder.HasKey(x => new { x.AssociatedAccountId, x.ReadingDateTime }).HasName("MeterReadings_Composite_PK");

        builder.Property(x => x.ReadingValue).IsRequired().ValueGeneratedNever();
        builder.Property(x => x.ReadingDateTime).IsRequired().ValueGeneratedNever();
        builder.Property(x => x.AssociatedAccountId).IsRequired().ValueGeneratedNever();
    }
}