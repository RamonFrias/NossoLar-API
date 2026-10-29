using NossoLar.Domain.Common;
using NossoLar.Domain.Exceptions;
using NossoLar.Domain.Finances.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace NossoLar.Domain.MonthlyBudget.Entities
{
    public class Expense : Entity
    {
        public ExpenseGroup Group { get; private set; }
        public string Description { get; private set; } = null!;
        public decimal MonthlyAmount { get; private set; }

        internal Expense(ExpenseGroup group, string description, decimal monthlyAmount)
        {
            if (!Enum.IsDefined(group))
                throw new DomainException("Invalid expense group.");

            Group = group;
            ChangeDescription(description);
            ChangeMonthlyAmount(monthlyAmount);
        }

        internal void ChangeDescription(string description)
        {
            if (string.IsNullOrWhiteSpace(description))
                throw new DomainException("Description is required.");

            Description = description.Trim();
        }

        internal void ChangeMonthlyAmount(decimal monthlyAmount)
        {
            if (monthlyAmount < 0)
                throw new DomainException("Monthly amount cannot be negative.");

            MonthlyAmount = Round(monthlyAmount);
        }

        private static decimal Round(decimal value) =>
            decimal.Round(value, 2, MidpointRounding.AwayFromZero);
    }
}
