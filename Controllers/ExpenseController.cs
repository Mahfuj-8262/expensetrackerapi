using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ExpenseTracker.Data;
using ExpenseTracker.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace ExpenseTracker.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ExpenseController : ControllerBase
    {
        private readonly AppDbContext _context;
        public ExpenseController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Expense>>> GetExpense()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            return await _context.Expense.Where(e => e.UserId == userId).Include(e => e.Category).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Expense>> GetExpense(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var expense = await _context.Expense.Include(e => e.Category).FirstOrDefaultAsync(e => e.ExpenseId == id && e.UserId == userId);
            if (expense == null) return NotFound();
            return expense;
        }

        [HttpPost]
        public async Task<ActionResult<Expense>> PostExpense(ExpensePostDto exp)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var expense = new Expense
            {
                UserId = userId,
                CategoryId = exp.CategoryId,
                Amount = exp.Amount,
                Date = exp.Date,
                Description = exp.Description
            };
            _context.Expense.Add(expense);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetExpense), new { id = expense.ExpenseId }, expense);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Expense>> PutExpense(int id, ExpensePutDto exp)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var expense = await _context.Expense.FirstOrDefaultAsync(e => e.ExpenseId == id && e.UserId == userId);
            if (expense == null) return NotFound();

            expense.CategoryId = exp.CategoryId;
            expense.Amount = exp.Amount;
            expense.Date = exp.Date;
            expense.Description = exp.Description;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Expense>> DeleteExpense(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var expense = await _context.Expense.FirstOrDefaultAsync(e => e.ExpenseId == id && e.UserId == userId);
            if (expense == null) return NotFound();

            _context.Expense.Remove(expense);
            await _context.SaveChangesAsync();
            return NoContent();
        }


    }
}