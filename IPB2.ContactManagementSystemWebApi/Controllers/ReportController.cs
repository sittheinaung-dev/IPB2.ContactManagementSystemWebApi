using IPB2.ContactManagementSystemWebApi.Database.AppDbContextModels;
using IPB2.ContactManagementSystemWebApi.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IPB2.ContactManagementSystemWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly AppDbContext _db;

        public ReportController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet("View Contact Pagination")]
        public async Task<IActionResult> GetContacts(int pageNo = 1, int pageSize = 10)
        {
            var query = _db.Contacts
                .Where(x => x.IsDelete == false)
                .Include(x => x.Category);

            var totalCount = await query.CountAsync();

            var contacts = await query
                .Skip((pageNo - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new ContactResponseDto
                {
                    ContactId = x.ContactId,
                    ContactName = x.ContactName,
                    Email = x.Email,
                    Phone = x.Phone,
                    CategoryId = x.CategoryId ?? 0,
                    CategoryName = x.Category.CategoryName
                })
                .ToListAsync();

            return Ok(new
            {
                TotalCount = totalCount,
                PageNo = pageNo,
                PageSize = pageSize,
                Contacts = contacts
            });
        }

        [HttpGet("ContactsByCategory")]
        public async Task<IActionResult> GetContactsByCategory([FromQuery] int? categoryId = null)
        {
            if (categoryId.HasValue)
            {
                var category = await _db.Categories
                    .FirstOrDefaultAsync(x => x.CategoryId == categoryId && x.IsDelete == false);

                if (category == null)
                    return NotFound("Category not found");

                var contacts = await _db.Contacts
                    .Where(x => x.CategoryId == categoryId && x.IsDelete == false)
                    .Select(x => new ContactResponseDto
                    {
                        ContactId = x.ContactId,
                        ContactName = x.ContactName,
                        Email = x.Email,
                        Phone = x.Phone,
                        CategoryId = x.CategoryId ?? 0,
                        CategoryName = category.CategoryName
                    })
                    .OrderBy(x => x.ContactName)
                    .ToListAsync();

                return Ok(new
                {
                    CategoryId = category.CategoryId,
                    CategoryName = category.CategoryName,
                    TotalContacts = contacts.Count,
                    Contacts = contacts
                });
            }
            else
            {
                var categories = await _db.Categories
                    .Where(x => x.IsDelete == false)
                    .Select(category => new
                    {
                        CategoryId = category.CategoryId,
                        CategoryName = category.CategoryName,
                        Contacts = _db.Contacts
                            .Where(x => x.CategoryId == category.CategoryId && x.IsDelete == false)
                            .Select(x => new ContactResponseDto
                            {
                                ContactId = x.ContactId,
                                ContactName = x.ContactName,
                                Email = x.Email,
                                Phone = x.Phone,
                                CategoryId = x.CategoryId ?? 0,
                                CategoryName = category.CategoryName
                            })
                            .OrderBy(x => x.ContactName)
                            .ToList()
                    })
                    .ToListAsync();

                return Ok(categories);
            }
        }

        [HttpGet("Search")]
        public async Task<IActionResult> SearchContacts([FromQuery] string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return BadRequest("Keyword is required");

            var results = await _db.Contacts
                .Where(x => x.IsDelete == false &&
                    (EF.Functions.Like(x.ContactName, $"%{keyword}%") ||
                     EF.Functions.Like(x.Email, $"%{keyword}%") ||
                     EF.Functions.Like(x.Phone, $"%{keyword}%")))
                .Join(_db.Categories.Where(c => c.IsDelete == false),
                    contact => contact.CategoryId,
                    category => category.CategoryId,
                    (contact, category) => new ContactResponseDto
                    {
                        ContactId = contact.ContactId,
                        ContactName = contact.ContactName,
                        Email = contact.Email,
                        Phone = contact.Phone,
                        CategoryId = contact.CategoryId ?? 0,
                        CategoryName = category.CategoryName
                    })
                .OrderBy(x => x.ContactName)
                .ToListAsync();

            return Ok(new
            {
                Keyword = keyword,
                TotalResults = results.Count,
                Results = results
            });
        }
    }
}