using IPB2.ContactManagementSystemWebApi.Database.AppDbContextModels;
using Microsoft.EntityFrameworkCore;
using IPB2.ContactManagementSystemWebApi.Features.Report.Models;
using ContactModels = IPB2.ContactManagementSystemWebApi.Features.Contact.Models;

namespace IPB2.ContactManagementSystemWebApi.Features.Report;

public class ReportFeature
{
    private readonly AppDbContext _db;

    public ReportFeature(AppDbContext db)
    {
        _db = db;
    }

    public async Task<ContactPaginationResponse> GetContactsPaginatedAsync(ContactPaginationRequest request)
    {
        var query = _db.Contacts
            .Where(x => x.IsDelete == false)
            .Include(x => x.Category);

        var totalCount = await query.CountAsync();

        var contacts = await query
            .Skip((request.PageNo - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new ContactModels.ContactResponse
            {
                ContactId = x.ContactId,
                ContactName = x.ContactName,
                Email = x.Email,
                Phone = x.Phone,
                CategoryId = x.CategoryId ?? 0,
                CategoryName = x.Category.CategoryName
            })
            .ToListAsync();

        return new ContactPaginationResponse
        {
            IsSuccess = true,
            Message = "Contacts retrieved successfully",
            TotalCount = totalCount,
            PageNo = request.PageNo,
            PageSize = request.PageSize,
            Contacts = contacts
        };
    }

    public async Task<ContactsByCategoryReportResponse> GetContactsByCategoryAsync(ContactsByCategoryReportRequest request)
    {
        if (request.CategoryId.HasValue)
        {
            var category = await _db.Categories
                .FirstOrDefaultAsync(x => x.CategoryId == request.CategoryId && x.IsDelete == false);

            if (category == null)
                return new ContactsByCategoryReportResponse { IsSuccess = false, Message = "Category not found" };

            var contacts = await _db.Contacts
                .Where(x => x.CategoryId == request.CategoryId && x.IsDelete == false)
                .Select(x => new ContactModels.ContactResponse
                {
                    ContactId = x.ContactId,
                    ContactName = x.ContactName,
                    Email = x.Email,
                    Phone = x.Phone,
                    CategoryId = x.CategoryId ?? 0,
                    CategoryName = category.CategoryName
                })
                .OrderBy(x => x.ContactId)
                .ToListAsync();

            return new ContactsByCategoryReportResponse
            {
                IsSuccess = true,
                Message = "Contacts by category retrieved successfully",
                CategoryId = category.CategoryId,
                CategoryName = category.CategoryName,
                ContactCount = contacts.Count(),
                Contacts = contacts
            };
        }
        else
        {
            // The existing logic seems to return a list of reports if no categoryId is provided.
            // I'll adapt the response model to handle this or keep it consistent.
            // For now, I'll return an error or handle it as "All Categories" if I adjust the DTO.
            return new ContactsByCategoryReportResponse { IsSuccess = false, Message = "CategoryId is required for this report." };
        }
    }

    public async Task<ContactsByCategoryListReportResponse> GetContactsByAllCategoriesAsync()
    {
        var categories = await _db.Categories
            .Where(x => x.IsDelete == false)
            .Select(category => new ContactsByCategoryReportResponse
            {
                CategoryId = category.CategoryId,
                CategoryName = category.CategoryName,
                ContactCount = _db.Contacts.Count(x => x.CategoryId == category.CategoryId && x.IsDelete == false),
                Contacts = _db.Contacts
                    .Where(x => x.CategoryId == category.CategoryId && x.IsDelete == false)
                    .Select(x => new ContactModels.ContactResponse
                    {
                        ContactId = x.ContactId,
                        ContactName = x.ContactName,
                        Email = x.Email,
                        Phone = x.Phone,
                        CategoryId = x.CategoryId ?? 0,
                        CategoryName = category.CategoryName
                    })
                    .OrderBy(x => x.ContactId)
                    .ToList()
            })
            .ToListAsync();

        return new ContactsByCategoryListReportResponse
        {
            IsSuccess = true,
            Message = "All categories with contacts retrieved successfully",
            Categories = categories
        };
    }

    public async Task<ContactSearchResponse> SearchContactsAsync(ContactSearchRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Keyword))
            return new ContactSearchResponse { IsSuccess = false, Message = "Keyword is required" };

        var results = await _db.Contacts
            .Where(x => x.IsDelete == false &&
                (EF.Functions.Like(x.ContactName, $"%{request.Keyword}%") ||
                 EF.Functions.Like(x.Email, $"%{request.Keyword}%") ||
                 EF.Functions.Like(x.Phone, $"%{request.Keyword}%")))
            .Join(_db.Categories.Where(c => c.IsDelete == false),
                contact => contact.CategoryId,
                category => category.CategoryId,
                (contact, category) => new ContactModels.ContactResponse
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

        return new ContactSearchResponse
        {
            IsSuccess = true,
            Message = "Search executed successfully",
            Keyword = request.Keyword,
            TotalResults = results.Count(),
            Results = results
        };
    }
}
