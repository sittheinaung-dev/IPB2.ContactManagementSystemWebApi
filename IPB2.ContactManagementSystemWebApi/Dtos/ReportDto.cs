namespace IPB2.ContactManagementSystemWebApi.Dtos
{
    public class TotalContactsReportDto
    {
        public int TotalContacts { get; set; }
        public int ActiveContacts { get; set; }
        public int DeletedContacts { get; set; }
    }

    public class ContactsByCategoryReportDto
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int ContactCount { get; set; }
        public List<ContactResponseDto> Contacts { get; set; }
    }

    public class ContactSearchDto
    {
        public string SearchTerm { get; set; }
        public int? CategoryId { get; set; }
    }
}
