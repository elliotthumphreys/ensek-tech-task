using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace DataAccess.MappingConfigurations;

internal class AccountMappings : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.ToTable("Accounts");

        builder.HasKey(x => x.Id).HasName("Accounts_PK");

        builder.Property(x => x.Id).IsRequired().ValueGeneratedNever();
        builder.Property(x => x.FirstName).IsRequired().ValueGeneratedNever();
        builder.Property(x => x.LastName).IsRequired().ValueGeneratedNever();
        builder.Property(x => x.Version).IsRequired().IsConcurrencyToken();

        builder.HasMany(x => x.MeterReadings)
               .WithOne(x => x.AssociatedAccount)
               .HasForeignKey(x => x.AssociatedAccountId)
               .HasPrincipalKey(x => x.Id)
               .HasConstraintName("Accounts_MeterReadings_FK");
    }
}