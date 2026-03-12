using IPB2.ContactManagementSystemWebApi;
using IPB2.ContactManagementSystemWebApi.Database.AppDbContextModels;
using IPB2.ContactManagementSystemWebApi.Dtos;
using IPB2.ContactManagementSystemWebApi.IPB2.ContactManagementSystemWebApi;
using Microsoft.EntityFrameworkCore;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register DbContext with SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Map all endpoints

app.MapContactEndpoints();
app.MapReportEndpoints();
app.MapCategoryEndpoints();


app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}



namespace IPB2.ContactManagementSystemWebApi
{
    public static class ContactEndpoints
    {
        public static void MapContactEndpoints(this WebApplication app)
        {
            var contactGroup = app.MapGroup("/api/contact");

            // Add Contact
            contactGroup.MapPost("/Add", async (AppDbContext db, ContactCreateRequestDto dto) =>
            {
                var categoryExists = await db.Categories
                    .AnyAsync(x => x.CategoryId == dto.CategoryId && x.IsDelete == false);

                if (!categoryExists)
                    return Results.BadRequest("Category not found");

                var emailExists = await db.Contacts
                    .AnyAsync(x => x.Email == dto.Email && x.IsDelete == false);

                if (emailExists)
                    return Results.BadRequest("Email already exists");

                var contact = new Contact
                {
                    ContactName = dto.ContactName,
                    Email = dto.Email,
                    Phone = dto.Phone,
                    CategoryId = dto.CategoryId,
                    IsDelete = false
                };

                db.Contacts.Add(contact);
                await db.SaveChangesAsync();

                return Results.Ok(new ContactCreateResponseDto
                {
                    ContactId = contact.ContactId,
                    Message = "Contact added successfully"
                });
            });

            // Update Contact
            contactGroup.MapPut("/Update/{id}", async (int id, AppDbContext db, ContactUpdateRequestDto dto) =>
            {
                if (id != dto.ContactId)
                    return Results.BadRequest("ID mismatch");

                var contact = await db.Contacts
                    .FirstOrDefaultAsync(x => x.ContactId == id && x.IsDelete == false);

                if (contact == null)
                    return Results.NotFound("Contact not found");

                var categoryExists = await db.Categories
                    .AnyAsync(x => x.CategoryId == dto.CategoryId && x.IsDelete == false);

                if (!categoryExists)
                    return Results.BadRequest("Category not found");

                var emailExists = await db.Contacts
                    .AnyAsync(x => x.Email == dto.Email &&
                                   x.ContactId != id &&
                                   x.IsDelete == false);

                if (emailExists)
                    return Results.BadRequest("Email already used by another contact");

                contact.ContactName = dto.ContactName;
                contact.Email = dto.Email;
                contact.Phone = dto.Phone;
                contact.CategoryId = dto.CategoryId;

                await db.SaveChangesAsync();

                return Results.Ok("Contact updated successfully");
            });

            // Delete Contact
            contactGroup.MapDelete("/Delete/{id}", async (int id, AppDbContext db) =>
            {
                var contact = await db.Contacts
                    .FirstOrDefaultAsync(x => x.ContactId == id && x.IsDelete == false);

                if (contact == null)
                    return Results.NotFound("Contact not found");

                contact.IsDelete = true;
                await db.SaveChangesAsync();

                return Results.Ok("Contact deleted successfully");
            });

            // Get All Contacts
            contactGroup.MapGet("/AllContacts", async (AppDbContext db) =>
            {
                var contacts = await db.Contacts
                    .Where(x => x.IsDelete == false)
                    .Join(db.Categories,
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

                return Results.Ok(contacts);
            });

            // Get Contact By Id
            contactGroup.MapGet("/View/{id}", async (int id, AppDbContext db) =>
            {
                var contact = await db.Contacts
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
                    return Results.NotFound("Contact not found");

                return Results.Ok(contact);
            });
        }
    }

    public static class ReportEndpoints
    {
        public static void MapReportEndpoints(this WebApplication app)
        {
            var reportGroup = app.MapGroup("/api/report");

            // View Contact Pagination
            reportGroup.MapGet("/View Contact Pagination", async (AppDbContext db, int pageNo = 1, int pageSize = 10) =>
            {
                var query = db.Contacts
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

                return Results.Ok(new
                {
                    TotalCount = totalCount,
                    PageNo = pageNo,
                    PageSize = pageSize,
                    Contacts = contacts
                });
            });

            // Contacts By Category
            reportGroup.MapGet("/ContactsByCategory", async (AppDbContext db, int? categoryId = null) =>
            {
                if (categoryId.HasValue)
                {
                    var category = await db.Categories
                        .FirstOrDefaultAsync(x => x.CategoryId == categoryId && x.IsDelete == false);

                    if (category == null)
                        return Results.NotFound("Category not found");

                    var contacts = await db.Contacts
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

                    return Results.Ok(new
                    {
                        CategoryId = category.CategoryId,
                        CategoryName = category.CategoryName,
                        TotalContacts = contacts.Count,
                        Contacts = contacts
                    });
                }
                else
                {
                    var categories = await db.Categories
                        .Where(x => x.IsDelete == false)
                        .Select(category => new
                        {
                            CategoryId = category.CategoryId,
                            CategoryName = category.CategoryName,
                            Contacts = db.Contacts
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

                    return Results.Ok(categories);
                }
            });

            // Search Contacts
            reportGroup.MapGet("/Search", async (AppDbContext db, string keyword) =>
            {
                if (string.IsNullOrWhiteSpace(keyword))
                    return Results.BadRequest("Keyword is required");

                var results = await db.Contacts
                    .Where(x => x.IsDelete == false &&
                        (EF.Functions.Like(x.ContactName, $"%{keyword}%") ||
                         EF.Functions.Like(x.Email, $"%{keyword}%") ||
                         EF.Functions.Like(x.Phone, $"%{keyword}%")))
                    .Join(db.Categories.Where(c => c.IsDelete == false),
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

                return Results.Ok(new
                {
                    Keyword = keyword,
                    TotalResults = results.Count,
                    Results = results
                });
            });
        }

    }

namespace IPB2.ContactManagementSystemWebApi
    {
        public static class CategoryEndpoints
        {
            public static void MapCategoryEndpoints(this WebApplication app)
            {
                var categoryGroup = app.MapGroup("/api/category");

                // Add Category
                categoryGroup.MapPost("/Add", async (AppDbContext db, CategoryCreateRequestDto dto) =>
                {
                    var categoryExists = await db.Categories
                        .AnyAsync(x => x.CategoryName == dto.CategoryName && x.IsDelete == false);

                    if (categoryExists)
                        return Results.BadRequest("Category name already exists");

                    var category = new Category
                    {
                        CategoryName = dto.CategoryName,
                        IsDelete = false
                    };

                    db.Categories.Add(category);
                    await db.SaveChangesAsync();

                    return Results.Ok(new
                    {
                        CategoryId = category.CategoryId,
                        Message = "Category added successfully"
                    });
                });

                // Update Category
                categoryGroup.MapPut("/Update/{id}", async (int id, AppDbContext db, CategoryUpdateRequestDto dto) =>
                {
                    if (id != dto.CategoryId)
                        return Results.BadRequest("ID mismatch");

                    var category = await db.Categories
                        .FirstOrDefaultAsync(x => x.CategoryId == id && x.IsDelete == false);

                    if (category == null)
                        return Results.NotFound("Category not found");

                    var categoryExists = await db.Categories
                        .AnyAsync(x => x.CategoryName == dto.CategoryName &&
                                      x.CategoryId != id &&
                                      x.IsDelete == false);

                    if (categoryExists)
                        return Results.BadRequest("Category name already exists");

                    category.CategoryName = dto.CategoryName;
                    await db.SaveChangesAsync();

                    return Results.Ok("Category updated successfully");
                });

                // Delete Category
                categoryGroup.MapDelete("/Delete/{id}", async (int id, AppDbContext db) =>
                {
                    var category = await db.Categories
                        .FirstOrDefaultAsync(x => x.CategoryId == id && x.IsDelete == false);

                    if (category == null)
                        return Results.NotFound("Category not found");

                    var hasContacts = await db.Contacts
                        .AnyAsync(x => x.CategoryId == id && x.IsDelete == false);

                    if (hasContacts)
                        return Results.BadRequest("Cannot delete category that has contacts. Please reassign contacts first.");

                    category.IsDelete = true;
                    await db.SaveChangesAsync();

                    return Results.Ok("Category deleted successfully");
                });

                // Get Categories List with Contact Count
                categoryGroup.MapGet("/List", async (AppDbContext db) =>
                {
                    var categories = await db.Categories
                        .Where(x => x.IsDelete == false)
                        .Select(x => new
                        {
                            CategoryId = x.CategoryId,
                            CategoryName = x.CategoryName,
                            ContactCount = db.Contacts
                                .Count(c => c.CategoryId == x.CategoryId && c.IsDelete == false)
                        })
                        .ToListAsync();

                    return Results.Ok(categories);
                });

                // Get Category with Contacts
                categoryGroup.MapGet("/WithContacts/{id}", async (int id, AppDbContext db) =>
                {
                    var category = await db.Categories
                        .FirstOrDefaultAsync(x => x.CategoryId == id && x.IsDelete == false);

                    if (category == null)
                        return Results.NotFound("Category not found");

                    var contacts = await db.Contacts
                        .Where(x => x.CategoryId == id && x.IsDelete == false)
                        .Select(x => new
                        {
                            ContactId = x.ContactId,
                            ContactName = x.ContactName,
                            Email = x.Email,
                            Phone = x.Phone,
                            CategoryId = x.CategoryId ?? 0,
                            CategoryName = category.CategoryName
                        })
                        .ToListAsync();

                    return Results.Ok(new
                    {
                        CategoryId = category.CategoryId,
                        CategoryName = category.CategoryName,
                        ContactCount = contacts.Count,
                        Contacts = contacts
                    });
                });
            }
        }
    }
}