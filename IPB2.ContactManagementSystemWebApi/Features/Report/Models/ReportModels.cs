using IPB2.ContactManagementSystemWebApi.Features.Contact.Models;

namespace IPB2.ContactManagementSystemWebApi.Features.Report.Models;

public class ContactPaginationRequest
{
    public int PageNo { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class ContactPaginationResponse
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public int TotalCount { get; set; }
    public int PageNo { get; set; }
    public int PageSize { get; set; }
    public List<ContactResponse> Contacts { get; set; } = new();
}

public class ContactsByCategoryReportRequest
{
    public int? CategoryId { get; set; }
}

public class ContactsByCategoryReportResponse
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public int? CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public int ContactCount { get; set; }
    public List<ContactResponse> Contacts { get; set; } = new();
}

public class ContactsByCategoryListReportResponse
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<ContactsByCategoryReportResponse> Categories { get; set; } = new();
}

public class ContactSearchRequest
{
    public string Keyword { get; set; } = string.Empty;
}

public class ContactSearchResponse
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public string Keyword { get; set; } = string.Empty;
    public int TotalResults { get; set; }
    public List<ContactResponse> Results { get; set; } = new();
}
