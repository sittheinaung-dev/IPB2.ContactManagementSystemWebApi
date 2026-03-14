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

        public ContactController()
        {
            _db = new AppDbContext();
        }

        [HttpPost("Add")]
        public async Task<IActionResult> AddContact(ContactCreateRequestDto dto)
        {
            var categoryExists = await _db.Categories
                .AnyAsync(x => x.CategoryId == dto.CategoryId && x.IsDelete == false);

            if (!categoryExists)
                return BadRequest(new ContactCreateResponseDto
                {
                    IsSuccess = false,
                    Message = "Category not found"
                });

            var emailExists = await _db.Contacts
                .AnyAsync(x => x.Email == dto.Email && x.IsDelete == false);

            if (emailExists)
                return BadRequest(new ContactCreateResponseDto
                {
                    IsSuccess = false,
                    Message = "Email already exists"
                });

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
                IsSuccess = true,
                Message = "Contact added successfully"
            });
        }

        [HttpPut("Update/{id}")]
        public async Task<IActionResult> UpdateContact(int id, ContactUpdateRequestDto dto)
        {
            if (id != dto.ContactId)
                return BadRequest(new ContactUpdateResponseDto { IsSuccess = false, Message = "ID mismatch" });

            var contact = await _db.Contacts
                .FirstOrDefaultAsync(x => x.ContactId == id && x.IsDelete == false);

            if (contact == null)
                return NotFound(new ContactUpdateResponseDto { IsSuccess = false, Message = "Contact not found" });

            var categoryExists = await _db.Categories
                .AnyAsync(x => x.CategoryId == dto.CategoryId && x.IsDelete == false);

            if (!categoryExists)
                return BadRequest(new ContactUpdateResponseDto { IsSuccess = false, Message = "Category not found" });

            var emailExists = await _db.Contacts
                .AnyAsync(x => x.Email == dto.Email &&
                               x.ContactId != id &&
                               x.IsDelete == false);

            if (emailExists)
                return BadRequest(new ContactUpdateResponseDto { IsSuccess = false, Message = "Email already used by another contact" });

            contact.ContactName = dto.ContactName;
            contact.Email = dto.Email;
            contact.Phone = dto.Phone;
            contact.CategoryId = dto.CategoryId;

            await _db.SaveChangesAsync();

            return Ok(new ContactUpdateResponseDto { IsSuccess = true, Message = "Contact updated successfully" });
        }

        [HttpDelete("Delete{id}")]
        public async Task<IActionResult> DeleteContact(int id)
        {
            var contact = await _db.Contacts
                .FirstOrDefaultAsync(x => x.ContactId == id && x.IsDelete == false);

            if (contact == null)
                return NotFound(new ContactDeleteResponseDto { IsSuccess = false, Message = "Contact not found" });

            contact.IsDelete = true;

            await _db.SaveChangesAsync();

            return Ok(new ContactDeleteResponseDto { IsSuccess = true, Message = "Contact deleted successfully" });
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
                .OrderBy(x => x.ContactId)
                .ToListAsync();

            return Ok(new ContactListResponseDto
            {
                IsSuccess = true,
                Message = "Contacts retrieved successfully",
                Contacts = contacts
            });
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
                return NotFound(new ContactSingleResponseDto { IsSuccess = false, Message = "Contact not found" });

            return Ok(new ContactSingleResponseDto
            {
                IsSuccess = true,
                Message = "Contact retrieved successfully",
                Contact = contact
            });
        }
    }
}