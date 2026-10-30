using NossoLar.Domain.Exceptions;
using NossoLar.Domain.Finances.Enums;
using NossoLar.Domain.MonthlyBudgets.Entities;

namespace NossoLar.Domain.Test.MonthlyBudgets
{
    public class MonthlyBudgetTests
    {
        private static MonthlyBudget NewBudget(decimal income = 8790m, int reservePercentage = 10) =>
            new(income, reservePercentage);

        private static MonthlyBudget SpreadsheetBudget()
        {
            var budget = NewBudget();

            budget.AddExpense(ExpenseGroup.Housing, "Aluguel", 1950m);
            budget.AddExpense(ExpenseGroup.Bills, "Energia elétrica", 200m);
            budget.AddExpense(ExpenseGroup.Bills, "Internet", 150m);
            budget.AddExpense(ExpenseGroup.Bills, "Celular", 30m);
            budget.AddExpense(ExpenseGroup.Bills, "Streaming", 10m);
            budget.AddExpense(ExpenseGroup.GroceriesAndHome, "Supermercado", 2000m);
            budget.AddExpense(ExpenseGroup.GroceriesAndHome, "Produtos de limpeza", 150m);
            budget.AddExpense(ExpenseGroup.Transportation, "Combustível", 757.60m);
            budget.AddExpense(ExpenseGroup.Health, "Plano de saúde", 44m);
            budget.AddExpense(ExpenseGroup.Health, "Farmácia", 300m);
            budget.AddExpense(ExpenseGroup.LeisureAndPersonal, "Academia", 250m);
            budget.AddExpense(ExpenseGroup.LeisureAndPersonal, "Cuidados pessoais", 200m);
            budget.AddExpense(ExpenseGroup.LeisureAndPersonal, "Assinaturas", 100m);
            budget.AddExpense(ExpenseGroup.Other, "Imprevistos", 1700m);

            return budget;
        }

        [Fact]
        public void Constructor_WithValidData_SetsIncomeAndPercentage()
        {
            var budget = new MonthlyBudget(8790m, 15);

            Assert.Equal(8790m, budget.TotalIncome);
            Assert.Equal(15, budget.EmergencyReservePercentage);
        }

        [Fact]
        public void Constructor_WithoutPercentage_UsesDefaultOf10()
        {
            var budget = new MonthlyBudget(8790m);

            Assert.Equal(10, budget.EmergencyReservePercentage);
        }

        [Fact]
        public void Constructor_StartsWithoutExpenses()
        {
            var budget = NewBudget();

            Assert.Empty(budget.Expenses);
            Assert.Equal(0m, budget.TotalExpenses);
        }

        [Fact]
        public void Constructor_RoundsIncomeToTwoDecimals()
        {
            var budget = NewBudget(income: 1000.555m);

            Assert.Equal(1000.56m, budget.TotalIncome);
        }

        [Fact]
        public void Constructor_WithZeroIncome_IsAllowed()
        {
            var budget = NewBudget(income: 0m);

            Assert.Equal(0m, budget.TotalIncome);
        }

        [Fact]
        public void Constructor_WithNegativeIncome_Throws()
        {
            var ex = Assert.Throws<DomainException>(() => NewBudget(income: -1m));

            Assert.Equal("Total income cannot be negative.", ex.Message);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(101)]
        public void Constructor_WithPercentageOutOfRange_Throws(int percentage)
        {
            var ex = Assert.Throws<DomainException>(() => NewBudget(reservePercentage: percentage));

            Assert.Equal("Emergency reserve percentage must be between 0 and 100.", ex.Message);
        }

        [Fact]
        public void ChangeTotalIncome_WithValidValue_UpdatesIncome()
        {
            var budget = NewBudget();

            budget.ChangeTotalIncome(9400m);

            Assert.Equal(9400m, budget.TotalIncome);
        }

        [Fact]
        public void ChangeTotalIncome_WithNegativeValue_ThrowsAndKeepsCurrentIncome()
        {
            var budget = NewBudget();

            Assert.Throws<DomainException>(() => budget.ChangeTotalIncome(-100m));
            Assert.Equal(8790m, budget.TotalIncome);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(20)]
        [InlineData(100)]
        public void ChangeEmergencyReservePercentage_WithinRange_UpdatesPercentage(int percentage)
        {
            var budget = NewBudget();

            budget.ChangeEmergencyReservePercentage(percentage);

            Assert.Equal(percentage, budget.EmergencyReservePercentage);
        }

        [Fact]
        public void ChangeEmergencyReservePercentage_OutOfRange_ThrowsAndKeepsCurrentPercentage()
        {
            var budget = NewBudget(reservePercentage: 10);

            Assert.Throws<DomainException>(() => budget.ChangeEmergencyReservePercentage(150));
            Assert.Equal(10, budget.EmergencyReservePercentage);
        }

        [Fact]
        public void AddExpense_WithValidData_AddsAndReturnsExpense()
        {
            var budget = NewBudget();

            var expense = budget.AddExpense(ExpenseGroup.Housing, "Aluguel", 1950m);

            Assert.Equal(expense, Assert.Single(budget.Expenses));
            Assert.Equal(ExpenseGroup.Housing, expense.Group);
            Assert.Equal("Aluguel", expense.Description);
            Assert.Equal(1950m, expense.MonthlyAmount);
            Assert.NotEqual(Guid.Empty, expense.Id);
        }

        [Fact]
        public void AddExpense_TrimsDescriptionAndRoundsAmount()
        {
            var budget = NewBudget();

            var expense = budget.AddExpense(ExpenseGroup.Bills, "  Energia elétrica  ", 200.555m);

            Assert.Equal("Energia elétrica", expense.Description);
            Assert.Equal(200.56m, expense.MonthlyAmount);
        }

        [Fact]
        public void AddExpense_WithZeroAmount_IsAllowed()
        {
            var budget = NewBudget();

            var expense = budget.AddExpense(ExpenseGroup.Bills, "Gás", 0m);

            Assert.Equal(0m, expense.MonthlyAmount);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void AddExpense_WithoutDescription_ThrowsAndDoesNotAdd(string? description)
        {
            var budget = NewBudget();

            var ex = Assert.Throws<DomainException>(() =>
                budget.AddExpense(ExpenseGroup.Other, description!, 100m));

            Assert.Equal("Description is required.", ex.Message);
            Assert.Empty(budget.Expenses);
        }

        [Fact]
        public void AddExpense_WithNegativeAmount_ThrowsAndDoesNotAdd()
        {
            var budget = NewBudget();

            var ex = Assert.Throws<DomainException>(() =>
                budget.AddExpense(ExpenseGroup.Other, "Imprevistos", -1m));

            Assert.Equal("Monthly amount cannot be negative.", ex.Message);
            Assert.Empty(budget.Expenses);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(99)]
        public void AddExpense_WithInvalidGroup_ThrowsAndDoesNotAdd(int group)
        {
            var budget = NewBudget();

            var ex = Assert.Throws<DomainException>(() =>
                budget.AddExpense((ExpenseGroup)group, "Algo", 10m));

            Assert.Equal("Invalid expense group.", ex.Message);
            Assert.Empty(budget.Expenses);
        }

        [Fact]
        public void ChangeExpenseAmount_WithValidValue_UpdatesExpense()
        {
            var budget = NewBudget();
            var expense = budget.AddExpense(ExpenseGroup.Bills, "Energia elétrica", 200m);

            budget.ChangeExpenseAmount(expense.Id, 230m);

            Assert.Equal(230m, expense.MonthlyAmount);
        }

        [Fact]
        public void ChangeExpenseAmount_WithNegativeValue_ThrowsAndKeepsCurrentAmount()
        {
            var budget = NewBudget();
            var expense = budget.AddExpense(ExpenseGroup.Bills, "Energia elétrica", 200m);

            Assert.Throws<DomainException>(() => budget.ChangeExpenseAmount(expense.Id, -50m));
            Assert.Equal(200m, expense.MonthlyAmount);
        }

        [Fact]
        public void ChangeExpenseAmount_WithUnknownId_Throws()
        {
            var budget = NewBudget();

            var ex = Assert.Throws<DomainException>(() =>
                budget.ChangeExpenseAmount(Guid.NewGuid(), 100m));

            Assert.Equal("Expense not found.", ex.Message);
        }

        [Fact]
        public void ChangeExpenseDescription_WithValidText_UpdatesAndTrims()
        {
            var budget = NewBudget();
            var expense = budget.AddExpense(ExpenseGroup.Bills, "Luz", 200m);

            budget.ChangeExpenseDescription(expense.Id, "  Energia elétrica  ");

            Assert.Equal("Energia elétrica", expense.Description);
        }

        [Fact]
        public void ChangeExpenseDescription_WithEmptyText_ThrowsAndKeepsCurrentDescription()
        {
            var budget = NewBudget();
            var expense = budget.AddExpense(ExpenseGroup.Bills, "Luz", 200m);

            Assert.Throws<DomainException>(() => budget.ChangeExpenseDescription(expense.Id, "   "));
            Assert.Equal("Luz", expense.Description);
        }

        [Fact]
        public void ChangeExpenseDescription_WithUnknownId_Throws()
        {
            var budget = NewBudget();

            Assert.Throws<DomainException>(() =>
                budget.ChangeExpenseDescription(Guid.NewGuid(), "Qualquer"));
        }

        [Fact]
        public void RemoveExpense_WithExistingId_RemovesOnlyThatExpense()
        {
            var budget = NewBudget();
            var rent = budget.AddExpense(ExpenseGroup.Housing, "Aluguel", 1950m);
            var internet = budget.AddExpense(ExpenseGroup.Bills, "Internet", 150m);

            budget.RemoveExpense(rent.Id);

            Assert.Equal(internet, Assert.Single(budget.Expenses));
        }

        [Fact]
        public void RemoveExpense_WithUnknownId_Throws()
        {
            var budget = NewBudget();
            budget.AddExpense(ExpenseGroup.Housing, "Aluguel", 1950m);

            Assert.Throws<DomainException>(() => budget.RemoveExpense(Guid.NewGuid()));
            Assert.Single(budget.Expenses);
        }

        [Fact]
        public void Expenses_CannotBeModifiedFromOutside()
        {
            var budget = NewBudget();
            budget.AddExpense(ExpenseGroup.Housing, "Aluguel", 1950m);

            var asCollection = (ICollection<Expense>)budget.Expenses;

            Assert.Throws<NotSupportedException>(() => asCollection.Clear());
            Assert.Single(budget.Expenses);
        }

        [Fact]
        public void Spreadsheet_TotalsMatchTheSpreadsheet()
        {
            var budget = SpreadsheetBudget();

            Assert.Equal(7841.60m, budget.TotalExpenses);
            Assert.Equal(879.00m, budget.EmergencyReserveAmount);
            Assert.Equal(69.40m, budget.Balance);
            Assert.False(budget.HasNegativeBalance);
        }

        [Fact]
        public void EmergencyReserveAmount_RoundsToTwoDecimals()
        {
            var budget = NewBudget(income: 1234.56m, reservePercentage: 15);
            Assert.Equal(185.18m, budget.EmergencyReserveAmount);
        }

        [Fact]
        public void EmergencyReserveAmount_WithZeroPercentage_IsZero()
        {
            var budget = NewBudget(reservePercentage: 0);

            Assert.Equal(0m, budget.EmergencyReserveAmount);
        }

        [Fact]
        public void Balance_UpdatesWhenAnExpenseChanges()
        {
            var budget = SpreadsheetBudget();
            var energy = budget.Expenses.First(e => e.Description == "Energia elétrica");

            budget.ChangeExpenseAmount(energy.Id, 230m);

            Assert.Equal(39.40m, budget.Balance);
        }

        [Fact]
        public void Balance_UpdatesWhenIncomeChanges()
        {
            var budget = SpreadsheetBudget();

            budget.ChangeTotalIncome(9400m);

            Assert.Equal(940.00m, budget.EmergencyReserveAmount);
            Assert.Equal(618.40m, budget.Balance);
        }

        [Fact]
        public void Balance_UpdatesWhenAnExpenseIsRemoved()
        {
            var budget = SpreadsheetBudget();
            var rent = budget.Expenses.First(e => e.Description == "Aluguel");

            budget.RemoveExpense(rent.Id);

            Assert.Equal(2019.40m, budget.Balance);
        }

        [Fact]
        public void HasNegativeBalance_WhenExpensesExceedIncome_IsTrue()
        {
            var budget = NewBudget(income: 1000m, reservePercentage: 10);
            budget.AddExpense(ExpenseGroup.Housing, "Aluguel", 950m);

            Assert.Equal(-50m, budget.Balance);
            Assert.True(budget.HasNegativeBalance);
        }

        [Fact]
        public void PercentageOfIncome_ReturnsRoundedPercentage()
        {
            var budget = NewBudget(income: 8790m);

            Assert.Equal(22.18m, budget.PercentageOfIncome(1950m));
        }

        [Fact]
        public void PercentageOfIncome_WithZeroIncome_ReturnsZero()
        {
            var budget = NewBudget(income: 0m);

            Assert.Equal(0m, budget.PercentageOfIncome(100m));
        }

        [Fact]
        public void TotalsByGroup_SumsEachGroup()
        {
            var totals = SpreadsheetBudget().TotalsByGroup();

            Assert.Equal(1950m, totals[ExpenseGroup.Housing]);
            Assert.Equal(390m, totals[ExpenseGroup.Bills]);
            Assert.Equal(2150m, totals[ExpenseGroup.GroceriesAndHome]);
            Assert.Equal(757.60m, totals[ExpenseGroup.Transportation]);
            Assert.Equal(344m, totals[ExpenseGroup.Health]);
            Assert.Equal(550m, totals[ExpenseGroup.LeisureAndPersonal]);
            Assert.Equal(1700m, totals[ExpenseGroup.Other]);
        }

        [Fact]
        public void TotalsByGroup_IncludesEmptyGroupsWithZero()
        {
            var budget = NewBudget();
            budget.AddExpense(ExpenseGroup.Housing, "Aluguel", 1950m);

            var totals = budget.TotalsByGroup();

            Assert.Equal(Enum.GetValues<ExpenseGroup>().Length, totals.Count);
            Assert.Equal(0m, totals[ExpenseGroup.Health]);
        }
    }
}