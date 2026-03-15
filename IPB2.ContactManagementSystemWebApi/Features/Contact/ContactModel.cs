namespace IPB2.ContactManagementSystemWebApi.Features.Contact;

public class CreateContactRequest
{
    public string ContactName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public int CategoryId { get; set; }
}

public class CreateContactResponse
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public int ContactId { get; set; }
}

public class UpdateContactRequest
{
    public int ContactId { get; set; }
    public string ContactName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public int CategoryId { get; set; }
}

public class UpdateContactResponse
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
}

public class DeleteContactResponse
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
}

public class ContactResponse
{
    public int ContactId { get; set; }
    public string ContactName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public string? CategoryName { get; set; }
}

public class ContactListResponse
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<ContactResponse> Contacts { get; set; } = new();
}

public class ContactSingleResponse
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public ContactResponse? Contact { get; set; }
}
