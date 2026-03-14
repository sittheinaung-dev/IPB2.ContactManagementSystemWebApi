using System.Collections.Generic;

namespace IPB2.ContactManagementSystemWebApi.Dtos
{
    public class ContactCreateRequestDto
    {
        public string ContactName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public int CategoryId { get; set; }
    }

    public class ContactCreateResponseDto
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public int ContactId { get; set; }
    }

    public class ContactUpdateRequestDto
    {
        public int ContactId { get; set; }
        public string ContactName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public int CategoryId { get; set; }
    }

    public class ContactUpdateResponseDto
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
    }

    public class ContactDeleteResponseDto
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
    }

    public class ContactResponseDto
    {
        public int ContactId { get; set; }
        public string ContactName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public int? CategoryId { get; set; }
        public string? CategoryName { get; set; }
    }

    public class ContactListResponseDto
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public List<ContactResponseDto> Contacts { get; set; }
    }

    public class ContactSingleResponseDto
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public ContactResponseDto Contact { get; set; }
    }
}
