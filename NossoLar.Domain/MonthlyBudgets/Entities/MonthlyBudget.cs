using NossoLar.Domain.Common;
using NossoLar.Domain.Exceptions;
using NossoLar.Domain.Finances.Enums;
using NossoLar.Domain.MonthlyBudgets.Entities;

namespace NossoLar.Domain.MonthlyBudgets.Entities
{
    public class MonthlyBudget : Entity
    {
        private readonly List<Expense> _expenses = new();
        public decimal TotalIncome { get; private set; }
        public int EmergencyReservePercentage { get; private set; }
        public IReadOnlyCollection<Expense> Expenses => _expenses.AsReadOnly();
        public decimal TotalExpenses => _expenses.Sum(e => e.MonthlyAmount);
        public decimal EmergencyReserveAmount => Round(TotalIncome * EmergencyReservePercentage / 100m);
        public decimal Balance => TotalIncome - TotalExpenses - EmergencyReserveAmount;
        public bool HasNegativeBalance => Balance < 0;

        public MonthlyBudget(decimal totalIncome, int emergencyReservePercentage = 10)
        {
            ChangeTotalIncome(totalIncome);
            ChangeEmergencyReservePercentage(emergencyReservePercentage);
        }

        public void ChangeTotalIncome(decimal totalIncome)
        {
            if (totalIncome < 0)
                throw new DomainException("Total income cannot be negative.");

            TotalIncome = Round(totalIncome);
        }

        public void ChangeEmergencyReservePercentage(int percentage)
        {
            if (percentage is < 0 or > 100)
                throw new DomainException("Emergency reserve percentage must be between 0 and 100.");

            EmergencyReservePercentage = percentage;
        }

        public Expense AddExpense(ExpenseGroup group, string description, decimal monthlyAmount)
        {
            var expense = new Expense(group, description, monthlyAmount);
            _expenses.Add(expense);
            return expense;
        }

        public void ChangeExpenseDescription(Guid expenseId, string description) =>
            GetExpense(expenseId).ChangeDescription(description);

        public void ChangeExpenseAmount(Guid expenseId, decimal monthlyAmount) =>
            GetExpense(expenseId).ChangeMonthlyAmount(monthlyAmount);

        public void RemoveExpense(Guid expenseId) =>
            _expenses.Remove(GetExpense(expenseId));

        public decimal PercentageOfIncome(decimal amount) =>
            TotalIncome == 0 ? 0 : Round(amount / TotalIncome * 100m);

        public IReadOnlyDictionary<ExpenseGroup, decimal> TotalsByGroup() =>
            Enum.GetValues<ExpenseGroup>().ToDictionary(
                group => group,
                group => _expenses.Where(e => e.Group == group).Sum(e => e.MonthlyAmount));

        private Expense GetExpense(Guid expenseId) =>
            _expenses.FirstOrDefault(e => e.Id == expenseId)
            ?? throw new DomainException("Expense not found.");

        private static decimal Round(decimal value) =>
            decimal.Round(value, 2, MidpointRounding.AwayFromZero);
    }
}