namespace ExpenseTracker.Models
{
    public class Category
    {
        public int CategoryId { get; set; }
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public required string CategoryName { get; set; }

    }

    public class CategoryPostDto
    {
        public required string CategoryName{ get; set; }
    }

    public class CategoryPutDto
    {
        public required string CategoryName { get; set; }
    }
}