using IPB2.ContactManagementSystemWebApi.Database.AppDbContextModels;
using IPB2.ContactManagementSystemWebApi.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IPB2.ContactManagementSystemWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactController : ControllerBase
    {
        private readonly AppDbContext _db;

        public ContactController(AppDbContext db)
        {
            _db = db;
        }

        [HttpPost("Add")]
        public async Task<IActionResult> AddContact(ContactCreateRequestDto dto)
        {
            var categoryExists = await _db.Categories
                .AnyAsync(x => x.CategoryId == dto.CategoryId && x.IsDelete == false);

            if (!categoryExists)
                return BadRequest("Category not found");

            var emailExists = await _db.Contacts
                .AnyAsync(x => x.Email == dto.Email && x.IsDelete == false);

            if (emailExists)
                return BadRequest("Email already exists");

            var contact = new Contact
            {
                ContactName = dto.ContactName,
                Email = dto.Email,
                Phone = dto.Phone,
                CategoryId = dto.CategoryId,
                IsDelete = false
            };

            _db.Contacts.Add(contact);
            await _db.SaveChangesAsync();

            return Ok(new ContactCreateResponseDto
            {
                ContactId = contact.ContactId,
                Message = "Contact added successfully"
            });
        }

        [HttpPut("Update/{id}")]
        public async Task<IActionResult> UpdateContact(int id, ContactUpdateRequestDto dto)
        {
            if (id != dto.ContactId)
                return BadRequest("ID mismatch");

            var contact = await _db.Contacts
                .FirstOrDefaultAsync(x => x.ContactId == id && x.IsDelete == false);

            if (contact == null)
                return NotFound("Contact not found");

            var categoryExists = await _db.Categories
                .AnyAsync(x => x.CategoryId == dto.CategoryId && x.IsDelete == false);

            if (!categoryExists)
                return BadRequest("Category not found");

            var emailExists = await _db.Contacts
                .AnyAsync(x => x.Email == dto.Email &&
                               x.ContactId != id &&
                               x.IsDelete == false);

            if (emailExists)
                return BadRequest("Email already used by another contact");

            contact.ContactName = dto.ContactName;
            contact.Email = dto.Email;
            contact.Phone = dto.Phone;
            contact.CategoryId = dto.CategoryId;

            await _db.SaveChangesAsync();

            return Ok("Contact updated successfully");
        }

        [HttpDelete("Delete{id}")]
        public async Task<IActionResult> DeleteContact(int id)
        {
            var contact = await _db.Contacts
                .FirstOrDefaultAsync(x => x.ContactId == id && x.IsDelete == false);

            if (contact == null)
                return NotFound("Contact not found");

            contact.IsDelete = true;

            await _db.SaveChangesAsync();

            return Ok("Contact deleted successfully");
        }

        [HttpGet("AllContacts")]
        public async Task<IActionResult> GetAllContacts()
        {
            var contacts = await _db.Contacts
                .Where(x => x.IsDelete == false)
                .Join(_db.Categories,
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

            return Ok(contacts);
        }

        // Get Contact By Id
        [HttpGet("View/{id}")]
        public async Task<IActionResult> GetContact(int id)
        {
            var contact = await _db.Contacts
                .Where(x => x.ContactId == id && x.IsDelete == false)
                .Include(x => x.Category)
                .Select(x => new ContactResponseDto
                {
                    ContactId = x.ContactId,
                    ContactName = x.ContactName,
                    Email = x.Email,
                    Phone = x.Phone,
                    CategoryId = x.CategoryId ?? 0,
                    CategoryName = x.Category.CategoryName
                })
                .FirstOrDefaultAsync();

            if (contact == null)
                return NotFound("Contact not found");

            return Ok(contact);
        }
    }
}