using IPB2.ContactManagementSystemWebApi.Database.AppDbContextModels;
using IPB2.ContactManagementSystemWebApi.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IPB2.ContactManagementSystemWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly AppDbContext _db;

        public CategoryController(AppDbContext db)
        {
            _db = db;
        }

        [HttpPost("Add")]
        public async Task<IActionResult> AddCategory(CategoryCreateRequestDto dto)
        {
            var categoryExists = await _db.Categories
                .AnyAsync(x => x.CategoryName == dto.CategoryName && x.IsDelete == false);

            if (categoryExists)
                return BadRequest("Category name already exists");

            var category = new Category
            {
                CategoryName = dto.CategoryName,
                IsDelete = false
            };

            _db.Categories.Add(category);
            await _db.SaveChangesAsync();

            return Ok(new
            {
                CategoryId = category.CategoryId,
                Message = "Category added successfully"
            });
        }

        [HttpPut("Update/{id}")]
        public async Task<IActionResult> UpdateCategory(int id, CategoryUpdateRequestDto dto)
        {
            if (id != dto.CategoryId)
                return BadRequest("ID mismatch");

            var category = await _db.Categories
                .FirstOrDefaultAsync(x => x.CategoryId == id && x.IsDelete == false);

            if (category == null)
                return NotFound("Category not found");

            var categoryExists = await _db.Categories
                .AnyAsync(x => x.CategoryName == dto.CategoryName &&
                              x.CategoryId != id &&
                              x.IsDelete == false);

            if (categoryExists)
                return BadRequest("Category name already exists");

            category.CategoryName = dto.CategoryName;
            await _db.SaveChangesAsync();

            return Ok("Category updated successfully");
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _db.Categories
                .FirstOrDefaultAsync(x => x.CategoryId == id && x.IsDelete == false);

            if (category == null)
                return NotFound("Category not found");

            var hasContacts = await _db.Contacts
                .AnyAsync(x => x.CategoryId == id && x.IsDelete == false);

            if (hasContacts)
                return BadRequest("Cannot delete category that has contacts. Please reassign contacts first.");

            category.IsDelete = true;
            await _db.SaveChangesAsync();

            return Ok("Category deleted successfully");
        }

        [HttpGet("List")]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _db.Categories
                .Where(x => x.IsDelete == false)
                .Select(x => new CategoryResponseDto
                {
                    CategoryId = x.CategoryId,
                    CategoryName = x.CategoryName,
                    ContactCount = _db.Contacts
                        .Count(c => c.CategoryId == x.CategoryId && c.IsDelete == false)
                })
                .ToListAsync();

            return Ok(categories);
        }

        // Get Category with Contacts
        [HttpGet("WithContacts/{id}")]
        public async Task<IActionResult> GetCategoryWithContacts(int id)
        {
            var category = await _db.Categories
                .FirstOrDefaultAsync(x => x.CategoryId == id && x.IsDelete == false);

            if (category == null)
                return NotFound("Category not found");

            var contacts = await _db.Contacts
                .Where(x => x.CategoryId == id && x.IsDelete == false)
                .Select(x => new ContactResponseDto
                {
                    ContactId = x.ContactId,
                    ContactName = x.ContactName,
                    Email = x.Email,
                    Phone = x.Phone,
                    CategoryId = x.CategoryId ?? 0,
                    CategoryName = category.CategoryName
                })
                .ToListAsync();

            return Ok(new
            {
                CategoryId = category.CategoryId,
                CategoryName = category.CategoryName,
                ContactCount = contacts.Count,
                Contacts = contacts
            });
        }
    }
}