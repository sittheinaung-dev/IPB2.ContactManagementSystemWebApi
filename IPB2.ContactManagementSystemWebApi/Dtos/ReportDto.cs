using System.Collections.Generic;

namespace IPB2.ContactManagementSystemWebApi.Dtos
{
    public class TotalContactsReportDto
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public int TotalContacts { get; set; }
        public int ActiveContacts { get; set; }
        public int DeletedContacts { get; set; }
    }

    public class ContactsByCategoryReportDto
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int ContactCount { get; set; }
        public List<ContactResponseDto> Contacts { get; set; }
    }

    public class ContactsByCategoryListReportDto
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public List<ContactsByCategoryReportDto> Categories { get; set; }
    }

    public class ContactSearchDto
    {
        public string SearchTerm { get; set; }
        public int? CategoryId { get; set; }
    }

    public class ContactSearchResponseDto
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public string Keyword { get; set; }
        public int TotalResults { get; set; }
        public List<ContactResponseDto> Results { get; set; }
    }

    public class ContactPaginationResponseDto
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public int TotalCount { get; set; }
        public int PageNo { get; set; }
        public int PageSize { get; set; }
        public List<ContactResponseDto> Contacts { get; set; }
    }
}
