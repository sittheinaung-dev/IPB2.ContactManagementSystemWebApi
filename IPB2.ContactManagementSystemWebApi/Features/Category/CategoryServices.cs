using IPB2.ContactManagementSystemWebApi.Database.AppDbContextModels;
using Microsoft.EntityFrameworkCore;

namespace IPB2.ContactManagementSystemWebApi.Features.Category;

public class CategoryServices
{
    private readonly AppDbContext _db;

    public CategoryServices(AppDbContext db)
    {
        _db = db;
    }

    public async Task<CategoryListResponse> GetAllAsync()
    {
        var categories = await _db.Categories
            .Where(x => x.IsDelete == false)
            .Select(x => new CategoryResponse
            {
                CategoryId = x.CategoryId,
                CategoryName = x.CategoryName,
                ContactCount = _db.Contacts.Count(c => c.CategoryId == x.CategoryId && c.IsDelete == false)
            })
            .ToListAsync();

        return new CategoryListResponse { IsSuccess = true, Message = "Success", Categories = categories };
    }

    public async Task<CategoryWithContactsResponse> GetWithContactsAsync(int id)
    {
        var category = await _db.Categories.FirstOrDefaultAsync(x => x.CategoryId == id && x.IsDelete == false);
        if (category == null)
            return new CategoryWithContactsResponse { IsSuccess = false, Message = "Category not found" };

        var contacts = await _db.Contacts
            .Where(x => x.CategoryId == id && x.IsDelete == false)
            .Select(x => new Contact.ContactResponse
            {
                ContactId = x.ContactId,
                ContactName = x.ContactName,
                Email = x.Email ?? string.Empty,
                Phone = x.Phone ?? string.Empty,
                CategoryId = x.CategoryId ?? 0,
                CategoryName = category.CategoryName ?? string.Empty
            })
            .ToListAsync();

        return new CategoryWithContactsResponse
        {
            IsSuccess = true,
            Message = "Success",
            CategoryId = category.CategoryId,
            CategoryName = category.CategoryName,
            ContactCount = contacts.Count,
            Contacts = contacts
        };
    }

    public async Task<CreateCategoryResponse> CreateAsync(CreateCategoryRequest request)
    {
        var exists = await _db.Categories.AnyAsync(x => x.CategoryName == request.CategoryName && x.IsDelete == false);
        if (exists)
            return new CreateCategoryResponse { IsSuccess = false, Message = "Category name already exists" };

        var category = new IPB2.ContactManagementSystemWebApi.Database.AppDbContextModels.Category
        {
            CategoryName = request.CategoryName,
            IsDelete = false
        };

        _db.Categories.Add(category);
        await _db.SaveChangesAsync();

        return new CreateCategoryResponse { IsSuccess = true, Message = "Category created successfully", CategoryId = category.CategoryId };
    }

    public async Task<UpdateCategoryResponse> UpdateAsync(UpdateCategoryRequest request)
    {
        var category = await _db.Categories.FirstOrDefaultAsync(x => x.CategoryId == request.CategoryId && x.IsDelete == false);
        if (category == null)
            return new UpdateCategoryResponse { IsSuccess = false, Message = "Category not found" };

        var exists = await _db.Categories.AnyAsync(x => x.CategoryName == request.CategoryName && x.CategoryId != request.CategoryId && x.IsDelete == false);
        if (exists)
            return new UpdateCategoryResponse { IsSuccess = false, Message = "Category name already exists" };

        category.CategoryName = request.CategoryName;
        await _db.SaveChangesAsync();

        return new UpdateCategoryResponse { IsSuccess = true, Message = "Category updated successfully" };
    }

    public async Task<DeleteCategoryResponse> DeleteAsync(int id)
    {
        var category = await _db.Categories.FirstOrDefaultAsync(x => x.CategoryId == id && x.IsDelete == false);
        if (category == null)
            return new DeleteCategoryResponse { IsSuccess = false, Message = "Category not found" };

        var hasContacts = await _db.Contacts.AnyAsync(x => x.CategoryId == id && x.IsDelete == false);
        if (hasContacts)
            return new DeleteCategoryResponse { IsSuccess = false, Message = "Cannot delete category with contacts." };

        category.IsDelete = true;
        await _db.SaveChangesAsync();

        return new DeleteCategoryResponse { IsSuccess = true, Message = "Category deleted successfully" };
    }
}
