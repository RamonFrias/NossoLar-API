using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NossoLar.Domain.Residentials.Entities;
using NossoLar.Domain.Residentials.ValueObjects;

namespace NossoLar.Infraestructure.EntityConfiguration
{
    internal sealed class ResidentialConfiguration : IEntityTypeConfiguration<Residential>
    {
        public void Configure(EntityTypeBuilder<Residential> builder)
        {
            builder.ToTable("residentials");

            builder.HasKey(r => r.Id);
            builder.Property(r => r.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");

            builder.OwnsOne(r => r.Address, address => ConfigureAddress(address));
            builder.Navigation(r => r.Address).IsRequired();

            builder.OwnsOne(r => r.RentalCosts, costs =>
            {
                costs.Property(c => c.Rent)
                    .IsRequired()
                    .HasColumnName("rent")
                    .HasPrecision(12, 2);
                costs.Property(c => c.Condominium)
                    .IsRequired()
                    .HasColumnName("condominium")
                    .HasPrecision(12, 2);
                costs.Property(c => c.Iptu)
                    .IsRequired()
                    .HasColumnName("iptu")
                    .HasPrecision(12, 2);
            });
            builder.Navigation(r => r.RentalCosts).IsRequired();

            builder.Property(r => r.ResidentialType)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired()
                .HasColumnName("residential_type");

            builder.OwnsOne(r => r.RealEstateAgency, agency =>
            {
                agency.Property(a => a.Name)
                    .IsRequired()
                    .HasMaxLength(200)
                    .HasColumnName("agency_name");
                agency.OwnsOne(a => a.Address, address => ConfigureAddress(address, "agency_"));

                agency.OwnsOne(a => a.Cnpj, cnpj =>
                {
                    cnpj.Property(c => c.CNPJ)
                        .IsRequired()
                        .HasMaxLength(14)
                        .HasColumnName("cnpj");
                });
                agency.Property(a => a.Phone)
                    .IsRequired()
                    .HasMaxLength(11)
                    .HasColumnName("phone");
                agency.Navigation(a => a.Address).IsRequired();
                agency.Navigation(a => a.Cnpj).IsRequired();
            });
            builder.Property(r => r.Notes)
                .HasMaxLength(400)
                .HasColumnName("notes");
            builder.Property(r => r.Url)
                .HasColumnName("url")
                .HasConversion(url => url!.AbsoluteUri, value => new Uri(value))
                .HasMaxLength(500);
        }

        private static void ConfigureAddress<TOwner>(OwnedNavigationBuilder<TOwner, Address> address, string prefix = "") where TOwner : class
        {
            address.Property(a => a.Street)
                .IsRequired()
                .HasMaxLength(200)
                .HasColumnName($"{prefix}street");
            address.Property(a => a.Number)
                .IsRequired().HasMaxLength(10)
                .HasColumnName($"{prefix}number");
            address.Property(a => a.Neighborhood)
                .IsRequired()
                .HasMaxLength(200)
                .HasColumnName($"{prefix}neighborhood");
            address.Property(a => a.City)
                .IsRequired()
                .HasMaxLength(150)
                .HasColumnName($"{prefix}city");
            address.Property(a => a.State)
                .IsRequired()
                .HasMaxLength(2)
                .IsFixedLength()
                .HasColumnName($"{prefix}state");
            address.Property(a => a.ZipCode)
                .IsRequired()
                .HasMaxLength(8)
                .IsFixedLength()
                .HasColumnName($"{prefix}zip_code");
            address.Property(a => a.Complement)
                .HasMaxLength(200)
                .HasColumnName($"{prefix}complement");
            address.Property(a => a.ReferencePoint)
                .HasMaxLength(200)
                .HasColumnName($"{prefix}reference_point");
        }
    }
}
