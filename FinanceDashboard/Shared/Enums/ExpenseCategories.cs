namespace FinanceDashboard.Shared.Enums
{
    public enum ExpenseCategories
    {
        Groceries = 1,
        Utilities = 2,
        [DisplayAs("Rent or Mortgage")]
        RentOrMortgage = 3,
        Transportation = 4,
        [DisplayAs("Dining Out")]
        DiningOut = 5,
        Entertainment = 6,
        Healthcare = 7,
        Education = 8,
        Clothing = 9,
        Miscellaneous = 10,
    }
}
