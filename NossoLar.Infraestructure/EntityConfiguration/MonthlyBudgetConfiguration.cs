using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NossoLar.Domain.MonthlyBudgets.Entities;

namespace NossoLar.Infraestructure.EntityConfiguration
{
    internal sealed class MonthlyBudgetConfiguration : IEntityTypeConfiguration<MonthlyBudget>
    {
        public void Configure(EntityTypeBuilder<MonthlyBudget> builder)
        {
            builder.ToTable("monthly_budgets", t => t.HasCheckConstraint(
                "CK_MonthlyBudgets_EmergencyReservePercentage",
                "[emergency_reserve_percentage] BETWEEN 0 AND 100"));
            builder.HasKey(m => m.Id);
            builder.Property(m => m.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            builder.Property(m => m.TotalIncome)
                .IsRequired()
                .HasPrecision(12, 2)
                .HasColumnName("total_income");
            builder.Property(m => m.EmergencyReservePercentage)
                .IsRequired()
                .HasColumnName("emergency_reserve_percentage");
            builder.HasMany(m => m.Expenses)
                .WithOne()
                .HasForeignKey("monthly_budget_id")
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
            builder.Navigation(m => m.Expenses).UsePropertyAccessMode(PropertyAccessMode.Field);
            builder.Ignore(b => b.TotalExpenses);
            builder.Ignore(b => b.EmergencyReserveAmount);
            builder.Ignore(b => b.Balance);
            builder.Ignore(b => b.HasNegativeBalance);
        }
    }
}
