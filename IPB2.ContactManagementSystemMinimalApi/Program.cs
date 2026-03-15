using IPB2.ContactManagementSystemWebApi.Database.AppDbContextModels;
using Microsoft.EntityFrameworkCore;
using IPB2.ContactManagementSystemWebApi.Features.Contact;
using IPB2.ContactManagementSystemWebApi.Features.Category;
using IPB2.ContactManagementSystemWebApi.Features.Report;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<ContactServices>();
builder.Services.AddScoped<CategoryServices>();
builder.Services.AddScoped<ReportServices>();

var app = builder.Build();

// Contact Endpoints
app.MapGet("/api/contact", async (ContactServices services) => await services.GetAllAsync());
app.MapGet("/api/contact/{id}", async (int id, ContactServices services) => {
    var response = await services.GetByIdAsync(id);
    return response.IsSuccess ? Results.Ok(response) : Results.NotFound(response);
});
app.MapPost("/api/contact", async (CreateContactRequest request, ContactServices services) => {
    var response = await services.CreateAsync(request);
    return response.IsSuccess ? Results.Ok(response) : Results.BadRequest(response);
});
app.MapPut("/api/contact", async (UpdateContactRequest request, ContactServices services) => {
    var response = await services.UpdateAsync(request);
    return response.IsSuccess ? Results.Ok(response) : Results.BadRequest(response);
});
app.MapDelete("/api/contact/{id}", async (int id, ContactServices services) => {
    var response = await services.DeleteAsync(id);
    return response.IsSuccess ? Results.Ok(response) : Results.BadRequest(response);
});

// Category Endpoints
app.MapGet("/api/category", async (CategoryServices services) => await services.GetAllAsync());
app.MapGet("/api/category/WithContacts/{id}", async (int id, CategoryServices services) => {
    var response = await services.GetWithContactsAsync(id);
    return response.IsSuccess ? Results.Ok(response) : Results.NotFound(response);
});
app.MapPost("/api/category", async (CreateCategoryRequest request, CategoryServices services) => {
    var response = await services.CreateAsync(request);
    return response.IsSuccess ? Results.Ok(response) : Results.BadRequest(response);
});
app.MapPut("/api/category", async (UpdateCategoryRequest request, CategoryServices services) => {
    var response = await services.UpdateAsync(request);
    if (!response.IsSuccess) return response.Message == "Category not found" ? Results.NotFound(response) : Results.BadRequest(response);
    return Results.Ok(response);
});
app.MapDelete("/api/category/{id}", async (int id, CategoryServices services) => {
    var response = await services.DeleteAsync(id);
    if (!response.IsSuccess) return response.Message == "Category not found" ? Results.NotFound(response) : Results.BadRequest(response);
    return Results.Ok(response);
});

// Report Endpoints
app.MapGet("/api/report/paginated", async ([AsParameters] ContactPaginationRequest request, ReportServices services) => await services.GetContactsPaginatedAsync(request));
app.MapGet("/api/report/by-category", async ([AsParameters] ContactsByCategoryReportRequest request, ReportServices services) => await services.GetContactsByCategoryAsync(request));
app.MapGet("/api/report/all-categories", async (ReportServices services) => await services.GetContactsByAllCategoriesAsync());
app.MapGet("/api/report/search", async ([AsParameters] ContactSearchRequest request, ReportServices services) => await services.SearchContactsAsync(request));

app.Run();