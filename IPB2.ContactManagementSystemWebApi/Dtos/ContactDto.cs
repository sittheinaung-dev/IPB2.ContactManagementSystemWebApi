namespace IPB2.ContactManagementSystemWebApi.Dtos
{
    public class ContactCreateRequestDto
    {
        public string ContactName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public int CategoryId { get; set; }
    }

    public class ContactUpdateRequestDto
    {
        public int ContactId { get; set; }
        public string ContactName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public int CategoryId { get; set; }
    }

    public class ContactResponseDto
    {
        public int ContactId { get; set; }
        public string ContactName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public int? CategoryId { get; set; }
        public string ?CategoryName { get; set; }
    }

    public class ContactCreateResponseDto
    {
        public int ContactId { get; set; }
        public string Message { get; set; }
    }
}
