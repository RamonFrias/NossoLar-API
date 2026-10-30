using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NossoLar.Domain.Residentials.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace NossoLar.Infraestructure.EntityConfiguration
{
    internal class ResidentialConfiguration : IEntityTypeConfiguration<Residential>
    {
        public void Configure(EntityTypeBuilder<Residential> builder)
        {
            builder.ToTable("residentials");

            builder.HasKey(r => r.Id);
            builder.Property(r => r.Id).ValueGeneratedNever();

            builder.OwnsOne(r => r.Address, address =>
            {
                address.Property(a => a.Street)
                    .IsRequired()
                    .HasMaxLength(200)
                    .HasColumnName("street");
                address.Property(a => a.Number)
                    .IsRequired()
                    .HasMaxLength(5)
                    .HasColumnName("number");
                address.Property(a => a.Neighborhood)
                    .IsRequired()
                    .HasMaxLength(200)
                    .HasColumnName("neighborhood");
                address.Property(a => a.City)
                    .IsRequired()
                    .HasMaxLength(150)
                    .HasColumnName("city");
                address.Property(a => a.State)  
                    .IsRequired()
                    .HasMaxLength(2)
                    .HasColumnName("state");
                address.Property(a => a.ZipCode)
                    .IsRequired()
                    .HasMaxLength(8)
                    .HasColumnName("zip_code");
                address.Property(a => a.Complement)
                    .HasMaxLength(200)
                    .HasColumnName("complement");
                address.Property(a => a.ReferencePoint).HasMaxLength(200).HasColumnName("reference_point");

            });
            builder.Navigation(r => r.Address).IsRequired();
        }
    }
}
