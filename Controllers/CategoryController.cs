using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ExpenseTracker.Models;
using ExpenseTracker.Data;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;


namespace ExpenseTracker.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CategoryController : ControllerBase
    {
        private readonly AppDbContext _context;
        public CategoryController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategory()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            return await _context.Category.Where(c => c.UserId == userId).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Category>> GetCategory(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var category = await _context.Category.FirstOrDefaultAsync(c => c.CategoryId == id && c.UserId == userId);
            if (category == null) return NotFound();

            return category;
        }

        [HttpPost]
        public async Task<ActionResult<Category>> PostCategory(CategoryPostDto dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var category = new Category
            {
                UserId = userId,
                CategoryName = dto.CategoryName
            };
            _context.Category.Add(category);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCategory), new { id = category.CategoryId }, category);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Category>> PutCategory(int id, CategoryPutDto cat)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var category = await _context.Category.FirstOrDefaultAsync(c => c.CategoryId == id && c.UserId == userId);

            if (category == null) return NotFound();

            category.CategoryName = cat.CategoryName;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Category>> DeleteCategory(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var category = await _context.Category.FirstOrDefaultAsync(c => c.CategoryId == id && c.UserId == userId);
            if (category == null) return NotFound();

            _context.Category.Remove(category);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}