using IPB2.ContactManagementSystemWebApi.Database.AppDbContextModels;
using Microsoft.EntityFrameworkCore;

namespace IPB2.ContactManagementSystemWebApi.Features.Contact;

public class ContactServices
{
    private readonly AppDbContext _db;

    public ContactServices(AppDbContext db)
    {
        _db = db;
    }

    public async Task<ContactListResponse> GetAllAsync()
    {
        var contacts = await _db.Contacts
            .Where(x => x.IsDelete == false)
            .Select(x => new ContactResponse
            {
                ContactId = x.ContactId,
                ContactName = x.ContactName,
                Email = x.Email,
                Phone = x.Phone,
                CategoryId = x.CategoryId ?? 0,
                CategoryName = x.Category.CategoryName
            })
            .ToListAsync();

        return new ContactListResponse { IsSuccess = true, Message = "Success", Contacts = contacts };
    }

    public async Task<ContactSingleResponse> GetByIdAsync(int id)
    {
        var contact = await _db.Contacts
            .Include(x => x.Category)
            .FirstOrDefaultAsync(x => x.ContactId == id && x.IsDelete == false);

        if (contact == null)
            return new ContactSingleResponse { IsSuccess = false, Message = "Contact not found" };

        return new ContactSingleResponse
        {
            IsSuccess = true,
            Message = "Success",
            Contact = new ContactResponse
            {
                ContactId = contact.ContactId,
                ContactName = contact.ContactName,
                Email = contact.Email,
                Phone = contact.Phone,
                CategoryId = contact.CategoryId ?? 0,
                CategoryName = contact.Category?.CategoryName
            }
        };
    }

    public async Task<CreateContactResponse> CreateAsync(CreateContactRequest request)
    {
        var contact = new IPB2.ContactManagementSystemWebApi.Database.AppDbContextModels.Contact
        {
            ContactName = request.ContactName,
            Email = request.Email,
            Phone = request.Phone,
            CategoryId = request.CategoryId,
            IsDelete = false
        };

        _db.Contacts.Add(contact);
        await _db.SaveChangesAsync();

        return new CreateContactResponse { IsSuccess = true, Message = "Contact created successfully", ContactId = contact.ContactId };
    }

    public async Task<UpdateContactResponse> UpdateAsync(UpdateContactRequest request)
    {
        var contact = await _db.Contacts.FirstOrDefaultAsync(x => x.ContactId == request.ContactId && x.IsDelete == false);
        if (contact == null)
            return new UpdateContactResponse { IsSuccess = false, Message = "Contact not found" };

        contact.ContactName = request.ContactName;
        contact.Email = request.Email;
        contact.Phone = request.Phone;
        contact.CategoryId = request.CategoryId;

        await _db.SaveChangesAsync();

        return new UpdateContactResponse { IsSuccess = true, Message = "Contact updated successfully" };
    }

    public async Task<DeleteContactResponse> DeleteAsync(int id)
    {
        var contact = await _db.Contacts.FirstOrDefaultAsync(x => x.ContactId == id && x.IsDelete == false);
        if (contact == null)
            return new DeleteContactResponse { IsSuccess = false, Message = "Contact not found" };

        contact.IsDelete = true;
        await _db.SaveChangesAsync();

        return new DeleteContactResponse { IsSuccess = true, Message = "Contact deleted successfully" };
    }
}
