using IPB2.ContactManagementSystemWebApi.Database.AppDbContextModels;
using Microsoft.EntityFrameworkCore;
using IPB2.ContactManagementSystemWebApi.Features.Contact;
using IPB2.ContactManagementSystemWebApi.Features.Category;
using IPB2.ContactManagementSystemWebApi.Features.Report;
using IPB2.ContactManagementSystemWebApi.Features.Contact.Models;
using IPB2.ContactManagementSystemWebApi.Features.Category.Models;
using IPB2.ContactManagementSystemWebApi.Features.Report.Models;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<ContactFeature>();
builder.Services.AddScoped<CategoryFeature>();
builder.Services.AddScoped<ReportFeature>();

var app = builder.Build();

// Contact Endpoints
app.MapGet("/api/contact", async (ContactFeature feature) => await feature.GetAllAsync());
app.MapGet("/api/contact/{id}", async (int id, ContactFeature feature) => {
    var response = await feature.GetByIdAsync(id);
    return response.IsSuccess ? Results.Ok(response) : Results.NotFound(response);
});
app.MapPost("/api/contact", async (CreateContactRequest request, ContactFeature feature) => {
    var response = await feature.CreateAsync(request);
    return response.IsSuccess ? Results.Ok(response) : Results.BadRequest(response);
});
app.MapPut("/api/contact", async (UpdateContactRequest request, ContactFeature feature) => {
    var response = await feature.UpdateAsync(request);
    return response.IsSuccess ? Results.Ok(response) : Results.BadRequest(response);
});
app.MapDelete("/api/contact/{id}", async (int id, ContactFeature feature) => {
    var response = await feature.DeleteAsync(id);
    return response.IsSuccess ? Results.Ok(response) : Results.BadRequest(response);
});

// Category Endpoints
app.MapGet("/api/category", async (CategoryFeature feature) => await feature.GetAllAsync());
app.MapGet("/api/category/WithContacts/{id}", async (int id, CategoryFeature feature) => {
    var response = await feature.GetWithContactsAsync(id);
    return response.IsSuccess ? Results.Ok(response) : Results.NotFound(response);
});
app.MapPost("/api/category", async (CreateCategoryRequest request, CategoryFeature feature) => {
    var response = await feature.CreateAsync(request);
    return response.IsSuccess ? Results.Ok(response) : Results.BadRequest(response);
});
app.MapPut("/api/category", async (UpdateCategoryRequest request, CategoryFeature feature) => {
    var response = await feature.UpdateAsync(request);
    if (!response.IsSuccess) return response.Message == "Category not found" ? Results.NotFound(response) : Results.BadRequest(response);
    return Results.Ok(response);
});
app.MapDelete("/api/category/{id}", async (int id, CategoryFeature feature) => {
    var response = await feature.DeleteAsync(id);
    if (!response.IsSuccess) return response.Message == "Category not found" ? Results.NotFound(response) : Results.BadRequest(response);
    return Results.Ok(response);
});

// Report Endpoints
app.MapGet("/api/report/paginated", async ([AsParameters] ContactPaginationRequest request, ReportFeature feature) => await feature.GetContactsPaginatedAsync(request));
app.MapGet("/api/report/by-category", async ([AsParameters] ContactsByCategoryReportRequest request, ReportFeature feature) => await feature.GetContactsByCategoryAsync(request));
app.MapGet("/api/report/all-categories", async (ReportFeature feature) => await feature.GetContactsByAllCategoriesAsync());
app.MapGet("/api/report/search", async ([AsParameters] ContactSearchRequest request, ReportFeature feature) => await feature.SearchContactsAsync(request));

app.Run();