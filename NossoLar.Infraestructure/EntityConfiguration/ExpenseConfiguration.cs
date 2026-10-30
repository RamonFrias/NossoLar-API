using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NossoLar.Domain.MonthlyBudgets.Entities;

namespace NossoLar.Infraestructure.EntityConfiguration
{
    internal sealed class ExpenseConfiguration : IEntityTypeConfiguration<Expense>
    {
        public void Configure(EntityTypeBuilder<Expense> builder)
        {
            builder.ToTable("expenses");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            builder.Property(e => e.Group)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired()
                .HasColumnName("expense_group");
            builder.Property(e => e.Description)
                .HasMaxLength(200)
                .IsRequired()
                .HasColumnName("description");
            builder.Property(e => e.MonthlyAmount)
                .HasPrecision(12, 2)
                .IsRequired()
                .HasColumnName("monthly_amount");
        }
    }
}
