using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Models
{
    public class Expense
    {
        public int ExpenseId { get; set; }
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public int? CategoryId { get; set; }
        public Category? Category { get; set; }
        [Precision(12,4)]
        public decimal Amount { get; set; }
        public DateOnly Date { get; set; }
        public string Description { get; set; } = string.Empty;
    }

    public class ExpensePutDto
    {
        public int? CategoryId { get; set; }
        public decimal Amount { get; set; }
        public DateOnly Date { get; set; }
        public string Description { get; set; } = string.Empty;
    }

    public class ExpensePostDto
    {
        public int? CategoryId { get; set; }
        public decimal Amount { get; set; }
        public DateOnly Date { get; set; }
        public string Description { get; set; } = string.Empty;
    }

}